using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using UsaloYa.Dto;
using UsaloYa.Dto.Enums;
using UsaloYa.Dto.Utils;
using UsaloYa.Library.Models;
using UsaloYa.Library.Config;
using UsaloYa.Services.interfaces;
using Microsoft.SqlServer.Server;

namespace UsaloYa.Services
{
    public class ProductService : IProductService
    {
        private readonly DBContext _dBContext;
        private readonly IProductCategoryService _productCategoryService;
        private readonly AppConfig _settings;
        int NORMAL = 3;
        int WARNING = 2;
        int CRITICAL = 1;

        public ProductService(DBContext dBContext, IProductCategoryService productCategoryService, AppConfig settings)
        {
            _dBContext = dBContext;
            _productCategoryService = productCategoryService;
            _settings = settings;
            
          
        }

        public async Task<IEnumerable<Product4ListDto>> FilterProducts(int pageNumber, int categoryId, int companyId)
        {
            int pageSize = pageNumber == -1 ? 500 : 30;
            if (pageNumber == -1) pageNumber = 1;

            return categoryId == 0
                ? await _dBContext.Products
                    .Where(p => p.CompanyId == companyId)
                    .OrderBy(p => p.Name)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new Product4ListDto
                    {
                        ProductId = p.ProductId,
                        Discontinued = p.Discontinued,
                        Sku = p.Sku,
                        Description = p.Description,
                        Name = p.Name,
                        CompanyId = p.CompanyId
                    }).ToListAsync()
                : await _dBContext.Products
                    .Where(p => p.CompanyId == companyId && p.CategoryId == categoryId)
                    .OrderBy(p => p.Name)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new Product4ListDto
                    {
                        ProductId = p.ProductId,
                        Discontinued = p.Discontinued,
                        Sku = p.Sku,
                        Description = p.Description,
                        Name = p.Name,
                        CompanyId = p.CompanyId,
                        CategoryId = p.CategoryId ?? 0
                    }).ToListAsync();
        }

        public async Task<IEnumerable<Product4ListDto>> SearchProduct4List(int pageNumber, string keyword, int companyId)
        {
            int pageSize = pageNumber == -1 ? 500 : 30;
            if (pageNumber == -1) pageNumber = 1;

            keyword = keyword?.Trim() ?? "-1";
            return string.Equals(keyword, "-1", StringComparison.OrdinalIgnoreCase)
                ? await _dBContext.Products
                    .Where(p => p.CompanyId == companyId)
                    .OrderBy(p => p.Name)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new Product4ListDto
                    {
                        ProductId = p.ProductId,
                        Discontinued = p.Discontinued,
                        Sku = p.Sku,
                        Description = p.Description,
                        Name = p.Name,
                        CompanyId = p.CompanyId
                    }).ToListAsync()
                : await _dBContext.Products
                    .Where(p => p.CompanyId == companyId &&
                                (p.Name.Contains(keyword) ||
                                 p.Description != null && p.Description.Contains(keyword) ||
                                 p.Sku != null && p.Sku.Contains(keyword)))
                    .OrderBy(p => p.Name)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new Product4ListDto
                    {
                        ProductId = p.ProductId,
                        Discontinued = p.Discontinued,
                        Sku = p.Sku,
                        Description = p.Description,
                        Name = p.Name,
                        CompanyId = p.CompanyId
                    }).ToListAsync();
        }

        public async Task<IEnumerable<ProductDto>> SearchProductFull(int pageNumber, string keyword, int companyId)
        {
            keyword = keyword?.Trim() ?? "-1";

            var products = string.Equals(keyword, "-1", StringComparison.OrdinalIgnoreCase)
                ? await _dBContext.Products
                    .Where(p => p.CompanyId == companyId)
                    .OrderBy(p => p.Name)
                    .ToListAsync()
                : await _dBContext.Products
                    .Where(p => p.CompanyId == companyId &&
                                (p.Name.Contains(keyword) ||
                                 p.Description != null && p.Description.Contains(keyword) ||
                                 p.Sku != null && p.Sku.Contains(keyword)))
                    .OrderBy(p => p.Name)
                    .ToListAsync();

            return products.Select(p => new ProductDto
            {
                Barcode = p.Barcode,
                Name = p.Name,
                BuyPrice = p.BuyPrice,
                CompanyId = companyId,
                Description = p.Description,
                Discontinued = p.Discontinued,
                ProductId = p.ProductId,
                SKU = p.Sku,
                UnitPrice = p.UnitPrice,
                UnitPrice1 = p.UnitPrice1,
                UnitPrice2 = p.UnitPrice2,
                UnitPrice3 = p.UnitPrice3,
                UnitsInStock = p.UnitsInStock
            }).ToList();
        }

        public async Task<ProductDto?> GetProduct(int productId, int companyId)
        {
            var product = await _dBContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId && p.CompanyId == companyId);
            if (product == null) return null;

            return new ProductDto
            {
                Barcode = product.Barcode ?? "",
                CategoryId = product.CategoryId ?? 0,
                BuyPrice = product.BuyPrice,
                CompanyId = product.CompanyId,
                Description = product.Description ?? "",
                Discontinued = product.Discontinued,
                Name = product.Name,
                ProductId = product.ProductId,
                SKU = product.Sku,
                UnitPrice = product.UnitPrice,
                UnitPrice1 = product.UnitPrice1,
                UnitPrice2 = product.UnitPrice2,
                UnitPrice3 = product.UnitPrice3,
                UnitsInStock = product.UnitsInStock,
                LowInventoryStart = product.AlertaStockNumProducts,
                IsInventarioUpdated = product.IsInVentarioUpdated
            };
        }

        public async Task<bool> ImportProduct(string requestorId, ProductDto productDto, int companyId)
        {
            var user = await HeaderValidatorService.ValidateRequestor(requestorId, Role.Admin, _dBContext);
            if (user.UserId <= 0) return false;

            var productWithSameBarcodeAndSku = await _dBContext.Products
                .Where(p => (p.Barcode == productDto.Barcode || p.Sku == productDto.SKU) && p.CompanyId == companyId)
                .ToListAsync();

            var userWantsUpdateTheProduct = productDto.UpdateProduct ?? false;
            if (user.CompanyStatusId == (int)CompanyStatus.Free) userWantsUpdateTheProduct = false;

            Product? firstProduct = productWithSameBarcodeAndSku.FirstOrDefault();

            if (productWithSameBarcodeAndSku.Count > 1 && !userWantsUpdateTheProduct) return false;
            if (productWithSameBarcodeAndSku.Count > 0 && !userWantsUpdateTheProduct && firstProduct?.ProductId != productDto.ProductId) return false;

            var categoryId = await GetOrCreateProductCategory(productDto.Categoria, companyId);

            if (userWantsUpdateTheProduct && firstProduct != null)
            {
                var prodUpdateSettings = productDto.UpdateSettings;
                if (prodUpdateSettings != null)
                {
                    if (prodUpdateSettings.UpdateNombre) firstProduct.Name = productDto.Name.Trim();
                    if (prodUpdateSettings.UpdateAlertaExistencias) firstProduct.AlertaStockNumProducts = productDto.LowInventoryStart ?? 0;
                    if (prodUpdateSettings.UpdatePrecioUnitario) firstProduct.UnitPrice = productDto.UnitPrice;
                    if (prodUpdateSettings.UpdatePrecio1) firstProduct.UnitPrice1 = productDto.UnitPrice1;
                    if (prodUpdateSettings.UpdatePrecio2) firstProduct.UnitPrice2 = productDto.UnitPrice2;
                    if (prodUpdateSettings.UpdatePrecio3) firstProduct.UnitPrice3 = productDto.UnitPrice3;
                    if (prodUpdateSettings.UpdateCategoria) firstProduct.CategoryId = categoryId == 0 ? null : categoryId;
                    if (prodUpdateSettings.UpdateStock) firstProduct.UnitsInStock = productDto.UnitsInStock;
                }
                _dBContext.Entry(firstProduct).State = EntityState.Modified;
            }
            else if (productDto.ProductId == 0)
            {
                var newProduct = new Product
                {
                    Name = productDto.Name.Trim(),
                    Description = productDto.Description,
                    CategoryId = categoryId == 0 ? null : categoryId,
                    BuyPrice = productDto.BuyPrice == 1 ? null : productDto.BuyPrice,
                    UnitPrice = productDto.UnitPrice == 0 ? 1 : productDto.UnitPrice,
                    UnitPrice1 = productDto.UnitPrice1 == 0 ? null : productDto.UnitPrice1,
                    UnitPrice2 = productDto.UnitPrice2 == 0 ? null : productDto.UnitPrice2,
                    UnitPrice3 = productDto.UnitPrice3 == 0 ? null : productDto.UnitPrice3,
                    UnitsInStock = productDto.UnitsInStock,
                    Discontinued = productDto.Discontinued,
                    DateModified = Utils.GetMxDateTime(),
                    Sku = string.IsNullOrEmpty(productDto.SKU) ? null : productDto.SKU,
                    Barcode = productDto.Barcode,
                    DateAdded = Utils.GetMxDateTime(),
                    CompanyId = companyId,
                    AlertaStockNumProducts = productDto.LowInventoryStart ?? 0,
                    IsInVentarioUpdated = productDto.IsInventarioUpdated
                };

                _dBContext.Products.Add(newProduct);
            }
            else
            {
                return false;
            }

            await _dBContext.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetOrCreateProductCategory(string? categoria, int companyId)
        {
            if (string.IsNullOrEmpty(categoria?.Trim())) return 0;

            try
            {
                var catInfo = await _productCategoryService.GetCategoryByName(categoria, companyId);
                return catInfo.CategoryId;
            }
            catch (Exception ex)
            {
                // Log error at service level.
                return 0;
            }
        }

        public async Task<Product?> AddProduct(string requestorId, ProductDto productDto, int companyId)
        {
            var user = await HeaderValidatorService.ValidateRequestor(requestorId, Role.Admin, _dBContext);
            if (user.UserId <= 0) return null;

            if (productDto.Equals(default(ProductDto)) || companyId <= 0) return null;

            var existingProduct = await _dBContext.Products
                .FirstOrDefaultAsync(p => p.ProductId == productDto.ProductId && p.CompanyId == companyId);

            var productWithSameBarcodeAndSku = _dBContext.Products
                .Where(p => (p.Barcode == productDto.Barcode || p.Sku == productDto.SKU) && p.CompanyId == companyId);

            var numOfProducts = await productWithSameBarcodeAndSku.CountAsync();
            if (numOfProducts > 1 || (numOfProducts > 0 && productWithSameBarcodeAndSku.First().ProductId != productDto.ProductId)) return null;

            if (existingProduct == null && productDto.ProductId == 0)
            {
                if (user.CompanyStatusId == (int)CompanyStatus.Free)
                {
                    var numExistingRecords = await _dBContext.Products.CountAsync(c => c.CompanyId == companyId);
                    if (numExistingRecords >= _settings.FreeRoleMaxProducts) return null;
                }

                existingProduct = new Product
                {
                    Name = productDto.Name.Trim(),
                    Description = productDto.Description,
                    CategoryId = productDto.CategoryId == 0 ? null : productDto.CategoryId,
                    BuyPrice = productDto.BuyPrice,
                    UnitPrice = productDto.UnitPrice,
                    UnitPrice1 = productDto.UnitPrice1,
                    UnitPrice2 = productDto.UnitPrice2,
                    UnitPrice3 = productDto.UnitPrice3,
                    UnitsInStock = productDto.UnitsInStock,
                    Discontinued = productDto.Discontinued,
                    DateModified = Utils.GetMxDateTime(),
                    Sku = string.IsNullOrEmpty(productDto.SKU) ? null : productDto.SKU,
                    Barcode = productDto.Barcode,
                    DateAdded = Utils.GetMxDateTime(),
                    CompanyId = companyId,
                    AlertaStockNumProducts = productDto.LowInventoryStart ?? 0,
                    IsInVentarioUpdated = productDto.IsInventarioUpdated
                };

                _dBContext.Products.Add(existingProduct);
            }
            else if (existingProduct != null)
            {
                existingProduct.Name = productDto.Name.Trim();
                existingProduct.Description = productDto.Description;
                existingProduct.CategoryId = productDto.CategoryId == 0 ? null : productDto.CategoryId;
                existingProduct.BuyPrice = productDto.BuyPrice;
                existingProduct.UnitPrice = productDto.UnitPrice;
                existingProduct.UnitPrice1 = productDto.UnitPrice1;
                existingProduct.UnitPrice2 = productDto.UnitPrice2;
                existingProduct.UnitPrice3 = productDto.UnitPrice3;
                existingProduct.UnitsInStock = productDto.UnitsInStock;
                existingProduct.Discontinued = productDto.Discontinued;
                existingProduct.DateModified = Utils.GetMxDateTime();
                existingProduct.Sku = string.IsNullOrEmpty(productDto.SKU) ? null : productDto.SKU;
                existingProduct.Barcode = productDto.Barcode;
                existingProduct.AlertaStockNumProducts = productDto.LowInventoryStart ?? 0;
                existingProduct.IsInVentarioUpdated = productDto.IsInventarioUpdated;

                _dBContext.Entry(existingProduct).State = EntityState.Modified;
            }
            else
            {
                return null;
            }

            await _dBContext.SaveChangesAsync();
            return existingProduct;
        }


        public async Task<IEnumerable<Product4InventariotDto>> GetInventarioByCategoryId(int categoryId, int companyId, int pageNumber)
        {
            int pageSize = 50;
            

            return await _dBContext.Products
                .Include(c => c.Category)
                .Where(p => p.CompanyId == companyId && !p.Discontinued && p.CategoryId == categoryId)
                .OrderBy(p => p.UnitsInStock <= 0 ? CRITICAL : p.UnitsInStock <= p.AlertaStockNumProducts ? WARNING : NORMAL)
                .ThenBy(p => p.UnitsInStock)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new Product4InventariotDto
                {
                    ProductId = p.ProductId,
                    Discontinued = p.Discontinued,
                    Sku = p.Sku ?? "",
                    Barcode = p.Barcode ?? "",
                    Name = p.Name,
                    CompanyId = p.CompanyId,
                    UnitsInStock = p.UnitsInStock,
                    TotalCashStock = p.UnitsInStock * p.UnitPrice,
                    UnitsInVentario = p.InVentario ?? 0,
                    AlertaStockNumProducts = p.AlertaStockNumProducts,
                    InVentarioAlertLevel = p.UnitsInStock <= 0 ? CRITICAL : p.UnitsInStock <= p.AlertaStockNumProducts ? WARNING : NORMAL,
                    UnitPrice = p.UnitPrice ?? 0,
                    CategoryId = p.CategoryId ?? -1,
                    CategoryName = p.Category.Name ?? "",
                    IsInVentarioUpdated = p.IsInVentarioUpdated ?? false
                })
                .ToListAsync();
        }

        public async Task<List<Product4InventariotDto>> GetInventarioByAlertId(int alertLevel, int companyId, int pageNumber)
        {
            int pageSize = 50;

            var products = await _dBContext.Products
                .Include(c => c.Category)
                .Select(p => new Product4InventariotDto()
                {
                    ProductId = p.ProductId,
                    Discontinued = p.Discontinued,
                    Sku = p.Sku ?? "",
                    Barcode = p.Barcode ?? "",
                    Name = p.Name,
                    CompanyId = p.CompanyId,
                    UnitsInStock = p.UnitsInStock,
                    TotalCashStock = p.UnitsInStock * p.UnitPrice,
                    UnitsInVentario = p.InVentario ?? 0,
                    AlertaStockNumProducts = p.AlertaStockNumProducts,
                    InVentarioAlertLevel = p.UnitsInStock <= 0 ? CRITICAL : p.UnitsInStock <= p.AlertaStockNumProducts ? WARNING : NORMAL,
                    UnitPrice = p.UnitPrice ?? 0,
                    CategoryId = p.CategoryId ?? 0,
                    CategoryName = p.Category.Name ?? "",
                    IsInVentarioUpdated = p.IsInVentarioUpdated ?? false
                })
                .Where(p => p.CompanyId == companyId && !p.Discontinued && p.InVentarioAlertLevel == alertLevel)
                .OrderBy(p => p.InVentarioAlertLevel)
                .ThenBy(p => p.UnitsInStock)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return products;
        }

        public async Task<List<Product4InventariotDto>> GetInventarioWithDiscrepancias(int companyId, int pageNumber)
        {
            int pageSize = 50;

            var products = await _dBContext.Products
                .Include(c => c.Category)
                .Select(p => new Product4InventariotDto()
                {
                    ProductId = p.ProductId,
                    Discontinued = p.Discontinued,
                    Sku = p.Sku ?? "",
                    Barcode = p.Barcode ?? "",
                    Name = p.Name,
                    CompanyId = p.CompanyId,
                    UnitsInStock = p.UnitsInStock,
                    TotalCashStock = p.UnitsInStock * p.UnitPrice,
                    UnitsInVentario = p.InVentario ?? 0,
                    AlertaStockNumProducts = p.AlertaStockNumProducts,
                    InVentarioAlertLevel = p.UnitsInStock <= 0 ? CRITICAL : p.UnitsInStock <= p.AlertaStockNumProducts ? WARNING : NORMAL,
                    UnitPrice = p.UnitPrice ?? 0,
                    CategoryId = p.CategoryId ?? 0,
                    CategoryName = p.Category.Name ?? "",
                    IsInVentarioUpdated = p.IsInVentarioUpdated ?? false
                })
                .Where(p => p.CompanyId == companyId && !p.Discontinued && p.UnitsInVentario != p.UnitsInStock)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return products.OrderBy(p => p.InVentarioAlertLevel).ThenBy(p => p.UnitsInStock).ToList();
        }

        public async Task<InventoryDto> GetInventarioTop50(string keyword, int companyId, int pageNumber)
        {
            int pageSize = 50;
            int totalInventoryProds = 0;
            decimal totalInventoryCash = 0;

            keyword = keyword.Trim();
            var products = string.Equals(keyword, "-1", StringComparison.OrdinalIgnoreCase)
                ? await _dBContext.Products
                    .Include(c => c.Category)
                    .Select(p => new Product4InventariotDto()
                    {
                        ProductId = p.ProductId,
                        Discontinued = p.Discontinued,
                        Sku = p.Sku ?? "",
                        Barcode = p.Barcode ?? "",
                        Name = p.Name,
                        CompanyId = p.CompanyId,
                        UnitsInStock = p.UnitsInStock,
                        TotalCashStock = p.UnitsInStock * p.UnitPrice,
                        UnitsInVentario = p.InVentario ?? 0,
                        AlertaStockNumProducts = p.AlertaStockNumProducts,
                        InVentarioAlertLevel = p.UnitsInStock <= 0 ? CRITICAL : p.UnitsInStock <= p.AlertaStockNumProducts ? WARNING : NORMAL,
                        CategoryId = p.CategoryId ?? 0,
                        CategoryName = p.Category.Name ?? "",
                        IsInVentarioUpdated = p.IsInVentarioUpdated ?? false
                    })
                    .Where(p => p.CompanyId == companyId && !p.Discontinued)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync()
                : await _dBContext.Products
                    .Include(c => c.Category)
                    .Select(p => new Product4InventariotDto()
                    {
                        ProductId = p.ProductId,
                        Discontinued = p.Discontinued,
                        Barcode = p.Barcode ?? "",
                        Sku = p.Sku ?? "",
                        Name = p.Name,
                        CompanyId = p.CompanyId,
                        UnitsInStock = p.UnitsInStock,
                        TotalCashStock = p.UnitsInStock * p.UnitPrice,
                        UnitsInVentario = p.InVentario ?? 0,
                        AlertaStockNumProducts = p.AlertaStockNumProducts,
                        InVentarioAlertLevel = p.UnitsInStock <= 0 ? CRITICAL : p.UnitsInStock <= p.AlertaStockNumProducts ? WARNING : NORMAL,
                        CategoryId = p.CategoryId ?? 0,
                        CategoryName = p.Category.Name ?? "",
                        IsInVentarioUpdated = p.IsInVentarioUpdated ?? false
                    })
                    .Where(p => p.CompanyId == companyId && !p.Discontinued &&
                        (p.Name.Contains(keyword) || keyword.Contains(p.Name) || p.Barcode == keyword || p.Sku == keyword))
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

            var activeProducts = await _dBContext.Products
                .Select(p => new StockTotalDto()
                {
                    CompanyId = p.CompanyId,
                    UnitPrice = p.UnitPrice ?? 0,
                    ProductId = p.ProductId,
                    UnitsInStock = p.UnitsInStock,
                    Discontinued = p.Discontinued
                })
                .Where(p => p.CompanyId == companyId && !p.Discontinued)
                .ToListAsync();

            if (activeProducts.Any())
            {
                totalInventoryProds = activeProducts.Sum(p => Math.Max(0, p.UnitsInStock));
                totalInventoryCash = activeProducts.Sum(p => Math.Max(0, p.UnitsInStock) * p.UnitPrice);
            }

            return new InventoryDto
            {
                Products = products.OrderBy(p => p.InVentarioAlertLevel).ThenBy(p => p.UnitsInStock).ToList(),
                TotalProductUnits = totalInventoryProds,
                TotalProducts = activeProducts.Count,
                TotalCash = totalInventoryCash
            };
        }


        public async Task<List<Product4InventariotDto>> GetInventarioItemsUpdated(int companyId, int pageNumber)
        {
            int pageSize = 50;

            var products = await _dBContext.Products
                .Include(c => c.Category)
                .Select(p => new Product4InventariotDto()
                {
                    ProductId = p.ProductId,
                    Discontinued = p.Discontinued,
                    Sku = p.Sku ?? "",
                    Barcode = p.Barcode ?? "",
                    Name = p.Name,
                    CompanyId = p.CompanyId,
                    UnitsInStock = p.UnitsInStock,
                    TotalCashStock = p.UnitsInStock * p.UnitPrice,
                    UnitsInVentario = p.InVentario ?? 0,
                    AlertaStockNumProducts = p.AlertaStockNumProducts,
                    InVentarioAlertLevel = p.UnitsInStock <= 0 ? CRITICAL : p.UnitsInStock <= p.AlertaStockNumProducts ? WARNING : NORMAL,
                    CategoryId = p.CategoryId ?? 0,
                    CategoryName = p.Category.Name ?? "",
                    IsInVentarioUpdated = p.IsInVentarioUpdated ?? false
                })
                .Where(p => p.CompanyId == companyId && !p.Discontinued && p.IsInVentarioUpdated)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return products.OrderBy(p => p.InVentarioAlertLevel).ThenBy(p => p.UnitsInStock).ToList();
        }

        public async Task<Product4InventariotDto> SetInVentario(string barcode, int quantity, int companyId)
        {
            var product = await _dBContext.Products
                .Include(c => c.Category)
                .FirstOrDefaultAsync(p => p.Barcode == barcode.Trim() && p.CompanyId == companyId);

            if (product != null)
            {
                product.InVentario = quantity > 1 ? quantity : (product.InVentario ?? 0) + 1;
                product.IsInVentarioUpdated = true;

                _dBContext.Entry(product).State = EntityState.Modified;
                await _dBContext.SaveChangesAsync();

                return new Product4InventariotDto()
                {
                    ProductId = product.ProductId,
                    Discontinued = product.Discontinued,
                    Barcode = product.Barcode ?? "",
                    Sku = product.Sku ?? "",
                    Name = product.Name,
                    CompanyId = product.CompanyId,
                    UnitsInStock = product.UnitsInStock,
                    TotalCashStock = product.UnitsInStock * product.UnitPrice,
                    UnitsInVentario = product.InVentario ?? 0,
                    AlertaStockNumProducts = product.AlertaStockNumProducts,
                    InVentarioAlertLevel = product.UnitsInStock <= 0 ? CRITICAL : product.UnitsInStock <= product.AlertaStockNumProducts ? WARNING : NORMAL,
                    CategoryName = product.Category?.Name ?? ""
                };
            }
            return null;
        }

        public async Task<bool> SetAllUnitsStock(int companyId)
        {
            string query = $"UPDATE [Products] SET UnitsInStock = InVentario, IsInVentarioUpdated=0 WHERE Discontinued = 0 AND IsInVentarioUpdated=1 AND CompanyId={companyId}";
            await _dBContext.Database.ExecuteSqlRawAsync(query);
            return true;
        }

        public async Task<bool> ResetAllInVentario(int companyId)
        {
            string query = $"UPDATE [Products] SET InVentario = 0 WHERE CompanyId={companyId}";
            await _dBContext.Database.ExecuteSqlRawAsync(query);
            return true;
        }

        public async Task<int> SetUnitsInStockByProductId(int productId, int companyId)
        {
            var product = await _dBContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId && p.CompanyId == companyId);
            if (product != null)
            {
                product.UnitsInStock = product.InVentario ?? 0;
                product.IsInVentarioUpdated = false;

                _dBContext.Entry(product).State = EntityState.Modified;
                await _dBContext.SaveChangesAsync();
                return product.UnitsInStock;
            }
            return -1;
        }

        public async Task<int> SetUnitsInStock(int productId, int unitsInStock, bool isHardReset, int companyId)
        {
            var product = await _dBContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId && p.CompanyId == companyId);

            if (product != null)
            {
                product.UnitsInStock = isHardReset ? unitsInStock : product.UnitsInStock + unitsInStock;

                _dBContext.Entry(product).State = EntityState.Modified;
                await _dBContext.SaveChangesAsync();
                return product.UnitsInStock;
            }

            return -1;
        }

    }
}
