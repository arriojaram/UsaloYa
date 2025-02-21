using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UsaloYa.API.Config;
using UsaloYa.API.DTO;
using UsaloYa.API.Enums;
using UsaloYa.API.Models;
using UsaloYa.API.Security;
using UsaloYa.API.Services;
using UsaloYa.API.Utils;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace UsaloYa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(AccessValidationFilter))]
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly DBContext _dBContext;
        private readonly ProductCategoryService _productCategoryService;

        int NORMAL = 3;
        int WARNING = 2;
        int CRITICAL = 1;

        public ProductController(DBContext dBContext, ILogger<ProductController> logger, ProductCategoryService prodCatService)
        {
            _logger = logger;
            _dBContext = dBContext;
            _productCategoryService = prodCatService;
        }

        [HttpGet("FilterProducts")]
        public async Task<IActionResult> FilterProducts(int pageNumber, int categoryId, int companyId)
        {
            int pageSize = 30;
            if (pageNumber == -1) //Used to manage the cache products function
            {
                pageSize = 500;
                pageNumber = 1;
            }
            try
            {
                if (companyId <= 0)
                {
                    return BadRequest(new { Message = "$_Empresa_O_Producto_Invalido" });
                }

                var products = categoryId == 0
                    ? await _dBContext.Products
                    .Select(p => new Product4ListDto() { ProductId = p.ProductId, Discontinued = p.Discontinued, Sku = p.Sku, Description = p.Description, Name = p.Name, CompanyId = p.CompanyId })
                    .Where(p => p.CompanyId == companyId)
                    .OrderBy(p => p.Name)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync()

                    : await _dBContext.Products
                    .Select(p => new Product4ListDto() 
                        { ProductId = p.ProductId, Discontinued = p.Discontinued, 
                            Sku = p.Sku, Description = p.Description, Name = p.Name, 
                            CompanyId = p.CompanyId, CategoryId = p.CategoryId?? 0 })
                    .Where(p => p.CompanyId == companyId && p.CategoryId == categoryId )
                    .OrderBy(p => p.Name)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                if (products == null || products.Count == 0)
                {
                    return NotFound();
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "FilterProducts.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpGet("SearchProduct4List")]
        public async Task<IActionResult> SearchProduct4List(int pageNumber, string keyword, int companyId)
        {
            int pageSize = 30;
            if (pageNumber == -1) //Used to manage the cache products function
            {
                pageSize = 500;
                pageNumber = 1;
            }
            try
            {
                if (string.IsNullOrEmpty(keyword) || companyId <= 0)
                {
                    return BadRequest(new { Message = "$_Empresa_O_Producto_Invalido" });
                }

                keyword = keyword.Trim();
                var products = string.Equals(keyword, "-1", StringComparison.OrdinalIgnoreCase)
                    ? await _dBContext.Products
                    .Select(p => new Product4ListDto() { ProductId = p.ProductId, Discontinued = p.Discontinued, Sku = p.Sku, Description = p.Description, Name = p.Name, CompanyId = p.CompanyId })
                    .Where(p => p.CompanyId == companyId)
                    .OrderBy(p => p.Name)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync()

                    : await _dBContext.Products
                    .Select(p => new Product4ListDto() { ProductId = p.ProductId, Discontinued = p.Discontinued, Sku = p.Sku, Description = p.Description, Name = p.Name, CompanyId = p.CompanyId })
                    .Where(p => p.CompanyId == companyId &&
                                (p.Name.Contains(keyword) || keyword.Contains(p.Name)
                                    || p.Description != null && p.Description.Contains(keyword)
                                    || p.Sku != null && p.Sku.Contains(keyword)))
                    .OrderBy(p => p.Name)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                if (products == null || products.Count == 0)
                {
                    return NotFound();
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SearchProduct.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpGet("SearchProductFull")]
        public async Task<IActionResult> SearchProductFull(int pageNumber, string keyword, int companyId)
        {
            try
            {
                if (string.IsNullOrEmpty(keyword) || companyId <= 0)
                {
                    return BadRequest(new { Message = "$_Empresa_O_Producto_Invalido" });
                }

                keyword = keyword.Trim();
                var products = string.Equals(keyword, "-1", StringComparison.OrdinalIgnoreCase)
                    ? await _dBContext.Products
                    .Where(p => p.CompanyId == companyId)
                    .OrderBy(p => p.Name)
                    .ToListAsync()

                    : await _dBContext.Products
                    .Where(p => p.CompanyId == companyId &&
                                (p.Name.Contains(keyword) || keyword.Contains(p.Name)
                                    || p.Description != null && p.Description.Contains(keyword)
                                    || p.Sku != null && p.Sku.Contains(keyword)))
                     .OrderBy(p => p.Name)
                    .ToListAsync();

                if (products == null || products.Count == 0)
                {
                    return NotFound();
                }

                return Ok(products.Select(p => new ProductDto()
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
                    UnitsInStock = p.UnitsInStock,
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SearchProduct.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpGet("GetProduct")]
        public async Task<IActionResult> GetProduct(int productId, int companyId)
        {
            try
            {
                if (productId <= 0 || companyId <= 0)
                {
                    return BadRequest(new { Message = "$_Empresa_O_Producto_Invalido" });
                }

                var product = await _dBContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId && p.CompanyId == companyId);

                if (product == null)
                {
                    return NotFound();
                }
                var p = new ProductDto() { 
                    
                    Barcode = product.Barcode??"",
                    CategoryId = product.CategoryId?? 0,
                    BuyPrice = product.BuyPrice,
                    CompanyId = product.CompanyId,
                    Description = product.Description??"",
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
                return Ok(p);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetProduct.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpPost("ImportProduct")]
        public async Task<IActionResult> ImportProduct([FromHeader] string RequestorId, [FromBody] ProductDto productDto, int companyId)
        {
            try
            {
                var user = await Util.ValidateRequestor(RequestorId, Role.Admin, _dBContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                if (productDto.Equals(default(ProductDto)) || companyId <= 0)
                {
                    return BadRequest(new { Message = "$_Empresa_O_Producto_Invalido" });
                }

                // Check if a product with the same Barcode and SKU in the company already exists
                var productWithSameBarcodeAndSku = await _dBContext.Products
                    .Where(p => (p.Barcode.Equals(productDto.Barcode) || p.Sku.Equals(productDto.SKU ?? "$SKU"))
                                                    && p.CompanyId == companyId).ToListAsync();
                
                var userWantsUpdateTheProduct = productDto.UpdateProduct ?? false;
                
                var numOfProducts = productWithSameBarcodeAndSku.Count;
                
                Product firstProduct = null;
                if (productWithSameBarcodeAndSku.Any())
                    firstProduct = productWithSameBarcodeAndSku[0];

                if (numOfProducts > 1 && !userWantsUpdateTheProduct)
                {
                    return Conflict(new { Message = "$_Producto_Con_Mismo_Codigo_De_Barras_Ya_Existe" });
                }

                if (numOfProducts > 0 && !userWantsUpdateTheProduct
                    && firstProduct?.ProductId != productDto.ProductId)
                {
                    return Conflict(new { Message = "$_Producto_Con_Mismo_Codigo_De_Barras_Ya_Existe" });
                }

                // Generar la categoria y traer el ID
                var categoryId = await GetOrCreateProductCategory(productDto.Categoria, companyId);
                if (userWantsUpdateTheProduct && firstProduct != null) 
                {
                    var prodUpdateSettings = productDto.UpdateSettings;
                    if (prodUpdateSettings != null)
                    {
                        if(prodUpdateSettings.UpdateNombre)
                            firstProduct.Name = productDto.Name.Trim();
                        if (prodUpdateSettings.UpdateAlertaExistencias)
                            firstProduct.AlertaStockNumProducts = productDto.LowInventoryStart ?? 0;
                        if(prodUpdateSettings.UpdatePrecioUnitario)
                            firstProduct.UnitPrice = productDto.UnitPrice;
                        if (prodUpdateSettings.UpdatePrecio1)
                            firstProduct.UnitPrice1 = productDto.UnitPrice1;
                        if (prodUpdateSettings.UpdatePrecio2)
                            firstProduct.UnitPrice2 = productDto.UnitPrice2;
                        if (prodUpdateSettings.UpdatePrecio3)
                            firstProduct.UnitPrice3 = productDto.UnitPrice3;
                        if (prodUpdateSettings.UpdateCategoria)
                            firstProduct.CategoryId = categoryId == 0 ? null : categoryId;
                        if(prodUpdateSettings.UpdateStock)
                            firstProduct.UnitsInStock = productDto.UnitsInStock;

                    }
                    _dBContext.Entry(firstProduct).State = EntityState.Modified;
                }
                else if (productDto.ProductId == 0)
                {
                    var existingProduct = new Product
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
                        DateModified = Utils.Util.GetMxDateTime(),

                        Sku = (string.IsNullOrEmpty(productDto.SKU) ? null : productDto.SKU),
                        Barcode = productDto.Barcode,

                        DateAdded = Utils.Util.GetMxDateTime(),
                        CompanyId = companyId,
                        AlertaStockNumProducts = productDto.LowInventoryStart ?? 0,
                        IsInVentarioUpdated = productDto.IsInventarioUpdated
                    };

                    _dBContext.Products.Add(existingProduct);

                }
                else
                {
                    return NotFound("$_Empresa_O_Producto_Invalido");
                }
                await _dBContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddProduct.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        private async Task<int> GetOrCreateProductCategory(string? categoria, int companyId)
        {
            if (string.IsNullOrEmpty((categoria ?? "").Trim()))
                return 0;

            int categoryId;
            try
            {
                var catInfo = await _productCategoryService.GetCategoryByName(categoria, companyId);
                categoryId = catInfo.CategoryId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetOrCreateProductCategory.ApiError");
                categoryId = 0;
            }
            return categoryId;
        }

        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromHeader] string RequestorId, [FromBody] ProductDto productDto, int companyId)
        {
            try
            {
                var user = await Util.ValidateRequestor(RequestorId, Role.Admin, _dBContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                if (productDto.Equals(default(ProductDto)) || companyId <= 0)
                {
                    return BadRequest(new { Message = "$_Empresa_O_Producto_Invalido" });
                }

                var existingProduct = await _dBContext.Products
                    .FirstOrDefaultAsync(p => p.ProductId == productDto.ProductId && p.CompanyId == companyId);

                // Check if a product with the same Barcode and SKU already exists
                var productWithSameBarcodeAndSku = _dBContext.Products
                    .Where(p => (p.Barcode.Equals(productDto.Barcode) || p.Sku.Equals(productDto.SKU ?? "$SKU"))
                                                    && p.CompanyId == companyId);

                var numOfProducts = await productWithSameBarcodeAndSku.CountAsync();
                if (numOfProducts > 1)
                {
                    return Conflict(new { Message = "$_Producto_Con_Mismo_Codigo_De_Barras_Ya_Existe" });
                }

                if (numOfProducts > 0
                    && productWithSameBarcodeAndSku.First().ProductId != productDto.ProductId)
                {
                    return Conflict(new { Message = "$_Producto_Con_Mismo_Codigo_De_Barras_Ya_Existe" });
                }

                if (existingProduct == null && productDto.ProductId == 0)
                {
                    
                    existingProduct = new Product
                    {
                        Name = productDto.Name.Trim(),
                        Description = productDto.Description,
                        CategoryId = (productDto.CategoryId?? 0) == 0 ? null : productDto.CategoryId,
                        BuyPrice = productDto.BuyPrice == 1 ? null : productDto.BuyPrice,
                        UnitPrice = productDto.UnitPrice == 0 ? 1 : productDto.UnitPrice,
                        UnitPrice1 = productDto.UnitPrice1 == 0 ? null : productDto.UnitPrice1,
                        UnitPrice2 = productDto.UnitPrice2 == 0 ? null : productDto.UnitPrice2,
                        UnitPrice3 = productDto.UnitPrice3 == 0 ? null : productDto.UnitPrice3,

                        UnitsInStock = productDto.UnitsInStock,
                        Discontinued = productDto.Discontinued,
                        DateModified = Utils.Util.GetMxDateTime(),

                        Sku = (string.IsNullOrEmpty(productDto.SKU) ? null : productDto.SKU),
                        Barcode = productDto.Barcode,
                        
                        DateAdded = Utils.Util.GetMxDateTime(),
                        CompanyId = companyId,
                        AlertaStockNumProducts = productDto.LowInventoryStart?? 0,
                        IsInVentarioUpdated = productDto.IsInventarioUpdated
                    };

                    _dBContext.Products.Add(existingProduct);

                }
                else if (existingProduct != null)
                {
                    existingProduct.Name = productDto.Name.Trim();
                    existingProduct.Description = productDto.Description;
                    existingProduct.CategoryId = (productDto.CategoryId ?? 0) == 0 ? null : productDto.CategoryId;

                    existingProduct.BuyPrice = productDto.BuyPrice;
                    existingProduct.UnitPrice = productDto.UnitPrice;
                    existingProduct.UnitPrice1 = productDto.UnitPrice1;
                    existingProduct.UnitPrice2 = productDto.UnitPrice2;
                    existingProduct.UnitPrice3 = productDto.UnitPrice3;

                    existingProduct.UnitsInStock = productDto.UnitsInStock;
                    existingProduct.Discontinued = productDto.Discontinued;

                    existingProduct.DateModified = Utils.Util.GetMxDateTime();
            
                    existingProduct.Sku = (string.IsNullOrEmpty(productDto.SKU) ? null : productDto.SKU);
                    existingProduct.Barcode = productDto.Barcode;
                    

                    existingProduct.AlertaStockNumProducts = productDto.LowInventoryStart ?? 0;
                    existingProduct.IsInVentarioUpdated = productDto.IsInventarioUpdated;

                    _dBContext.Entry(existingProduct).State = EntityState.Modified;
                }
                else
                {
                    return NotFound("$_Empresa_O_Producto_Invalido");
                }
                await _dBContext.SaveChangesAsync();
                return Ok(existingProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddProduct.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpGet("GetInventarioByCategoryId")]
        public async Task<IActionResult> GetInventarioByCategoryId([FromHeader] string RequestorId, int categoryId, int companyId, int pageNumber)
        {
            int totalInventoryProds = 0;
            decimal totalInventoryCash = 0;
            int pageSize = 50;
            try
            {
                var user = await Util.ValidateRequestorSameCompany(RequestorId, Role.Admin, companyId, _dBContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                if (companyId <= 0)
                {
                    return BadRequest(new { Message = "$_Empresa_O_Producto_Invalido" });
                }
                var products = await _dBContext.Products
                    .Include(c=> c.Category)
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
                    CategoryId= p.CategoryId?? -1,
                    CategoryName = p.Category != null ? p.Category.Name : "",
                    IsInVentarioUpdated = p.IsInVentarioUpdated?? false
                })
                .Where(p => p.CompanyId == companyId && !p.Discontinued && p.CategoryId == categoryId)
                .OrderBy(p => p.InVentarioAlertLevel).ThenBy(p => p.UnitsInStock)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SearchProduct.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpGet("GetInventarioByAlertId")]
        public async Task<IActionResult> GetInventarioByAlertId([FromHeader] string RequestorId, int alertLevel, int companyId, int pageNumber)
        {
            int totalInventoryProds = 0;
            decimal totalInventoryCash = 0;
            int pageSize = 50;
            try
            {
                var user = await Util.ValidateRequestorSameCompany(RequestorId, Role.Admin, companyId, _dBContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                if (companyId <= 0)
                {
                    return BadRequest(new { Message = "$_Empresa_O_Producto_Invalido" });
                }
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
                    UnitsInVentario = p.InVentario?? 0,
                    AlertaStockNumProducts = p.AlertaStockNumProducts,
                    InVentarioAlertLevel = p.UnitsInStock <= 0 ? CRITICAL : p.UnitsInStock <= p.AlertaStockNumProducts ? WARNING : NORMAL,
                    UnitPrice = p.UnitPrice ?? 0,
                    CategoryId = p.CategoryId ?? 0,
                    CategoryName = p.Category != null ? p.Category.Name : "",
                    IsInVentarioUpdated = p.IsInVentarioUpdated?? false
                })
                .Where(p => p.CompanyId == companyId && !p.Discontinued
                        && p.InVentarioAlertLevel == alertLevel)
                .OrderBy(p => p.InVentarioAlertLevel).ThenBy(p => p.UnitsInStock)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

                

                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SearchProduct.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpGet("GetInventarioWithDiscrepancias")]
        public async Task<IActionResult> GetInventarioWithDiscrepancias([FromHeader] string RequestorId, int companyId, int pageNumber)
        {
            int pageSize = 50;
           
            try
            {
                var user = await Util.ValidateRequestorSameCompany(RequestorId, Role.Admin, companyId, _dBContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                if (companyId <= 0)
                {
                    return BadRequest(new { Message = "$_Empresa_O_Producto_Invalido" });
                }
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
                            UnitPrice = p.UnitPrice?? 0,
                            CategoryId = p.CategoryId ?? 0,
                    CategoryName = p.Category != null ? p.Category.Name : "",
                    IsInVentarioUpdated = p.IsInVentarioUpdated?? false
                })
                        .Where(p => p.CompanyId == companyId 
                                    && !p.Discontinued
                                    && p.UnitsInVentario != p.UnitsInStock)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();


                var sortedProds = products.OrderBy(p => p.InVentarioAlertLevel).ThenBy(p => p.UnitsInStock).ToList();
                        

                if (products == null || products.Count == 0)
                {
                    return NotFound();
                }

                return Ok(sortedProds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SearchProduct.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpGet("GetInventarioTop50")]
        public async Task<IActionResult> GetInventarioTop50([FromHeader] string RequestorId, int pageNumber, string keyword, int companyId)
        {
            int pageSize = 50;
            int totalInventoryProds = 0;
            decimal totalInventoryCash = 0;
            try
            {
                var user = await Util.ValidateRequestorSameCompany(RequestorId, Role.Admin, companyId, _dBContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                if (string.IsNullOrEmpty(keyword) || companyId <= 0)
                {
                    return BadRequest(new { Message = "$_Empresa_O_Producto_Invalido" });
                }

                keyword = keyword.Trim();
                var products = string.Equals(keyword, "-1", StringComparison.OrdinalIgnoreCase)
                    ? await _dBContext.Products
                        .Include(c => c.Category)
                        .Select(p => new Product4InventariotDto()
                        {
                            ProductId = p.ProductId,
                            Discontinued = p.Discontinued,
                            Sku = p.Sku ?? "",
                            Barcode = p.Barcode?? "",
                            Name = p.Name,
                            CompanyId = p.CompanyId,
                            UnitsInStock = p.UnitsInStock,
                            TotalCashStock = p.UnitsInStock * p.UnitPrice,
                            UnitsInVentario = p.InVentario??0,
                            AlertaStockNumProducts = p.AlertaStockNumProducts,
                            InVentarioAlertLevel = p.UnitsInStock <= 0 ? CRITICAL : p.UnitsInStock <= p.AlertaStockNumProducts ? WARNING : NORMAL,
                            CategoryId = p.CategoryId ?? 0,
                            CategoryName = p.Category != null ? p.Category.Name : "",
                            IsInVentarioUpdated = p.IsInVentarioUpdated?? false

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
                             Barcode = p.Barcode?? "",
                             Sku = p.Sku ?? "",
                             Name = p.Name,
                             CompanyId = p.CompanyId,
                             UnitsInStock = p.UnitsInStock,
                             TotalCashStock = p.UnitsInStock * p.UnitPrice,
                             UnitsInVentario = p.InVentario,
                             AlertaStockNumProducts = p.AlertaStockNumProducts,
                             InVentarioAlertLevel = p.UnitsInStock <= 0 ? CRITICAL : p.UnitsInStock <= p.AlertaStockNumProducts ? WARNING : NORMAL,
                             CategoryId = p.CategoryId ?? 0,
                             CategoryName = p.Category != null ? p.Category.Name : ""
                         })
                         .Where(p => p.CompanyId == companyId && !p.Discontinued
                                    && (p.Name.Contains(keyword) || keyword.Contains(p.Name)
                                        || p.Barcode != null && p.Barcode == keyword)
                                        || p.Sku != null && p.Sku == keyword)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();

                var activeProducts = await _dBContext.Products
                                            .Select(p => new StockTotalDto() { 
                                                    CompanyId = p.CompanyId,
                                                    UnitPrice = p.UnitPrice?? 0,
                                                    ProductId = p.ProductId,
                                                    UnitsInStock = p.UnitsInStock,
                                                    Discontinued = p.Discontinued
                                                })
                                            .Where(p => p.CompanyId == companyId && !p.Discontinued).ToListAsync();

                if (activeProducts.Any())
                { 
                    totalInventoryProds = activeProducts.Sum(p => p.UnitsInStock < 0 ? 0 : p.UnitsInStock);
                    totalInventoryCash = activeProducts.Sum(p => (p.UnitsInStock < 0 ? 0 : p.UnitsInStock) * p.UnitPrice);
                }
                InventoryDto inventory = new InventoryDto();
                inventory.Products = products.OrderBy(p => p.InVentarioAlertLevel).ThenBy(p=> p.UnitsInStock).ToList();
                
                inventory.TotalProductUnits = totalInventoryProds;
                inventory.TotalProducts = activeProducts.Count;
                inventory.TotalCash = totalInventoryCash;

                if (products == null || products.Count == 0)
                {
                    return NotFound();
                }

                return Ok(inventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SearchProduct.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpGet("GetInventarioItemsUpdated")]
        public async Task<IActionResult> GetInventarioItemsUpdated([FromHeader] string RequestorId, int pageNumber, int companyId)
        {
            int pageSize = 50;
            try
            {
                var user = await Util.ValidateRequestorSameCompany(RequestorId, Role.Admin, companyId, _dBContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                if (companyId <= 0)
                {
                    return BadRequest(new { Message = "$_Empresa_O_Producto_Invalido" });
                }

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
                            CategoryName = p.Category != null ? p.Category.Name : "",
                            IsInVentarioUpdated = p.IsInVentarioUpdated?? false

                        })
                        .Where(p => p.CompanyId == companyId 
                                            && !p.Discontinued
                                            && p.IsInVentarioUpdated)
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();

              
                var sortedProds = products.OrderBy(p => p.InVentarioAlertLevel).ThenBy(p => p.UnitsInStock).ToList();

                if (products == null || products.Count == 0)
                {
                    return NotFound();
                }

                return Ok(sortedProds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SearchProduct.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }


        [HttpPost("SetInVentario")]
        public async Task<IActionResult> SetInVentario([FromHeader] string RequestorId, [FromBody] BarcodeDto barcodeDto, int companyId)
        {
            var existignInventario = 0;
            try
            {
                var user = await Util.ValidateRequestorSameCompany(RequestorId, Role.Admin, companyId, _dBContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                if (string.IsNullOrEmpty(barcodeDto.Code) || companyId <= 0)
                {
                    return BadRequest(new { Message = "$_Empresa_O_Producto_Invalido" });
                }

                var p = await _dBContext
                    .Products
                    .Include(c=> c.Category)
                    .FirstOrDefaultAsync(p => p.Barcode == barcodeDto.Code.Trim() && p.CompanyId == companyId);

                if (p != null)
                {
                    if (barcodeDto.Quantity > 1)
                    {
                        p.InVentario = barcodeDto.Quantity;
                    }
                    else
                    {
                        existignInventario = p.InVentario ?? 0;
                        p.InVentario = ++existignInventario;
                    }
                    p.IsInVentarioUpdated = true;
                    _dBContext.Entry(p).State = EntityState.Modified;
                    await _dBContext.SaveChangesAsync();
                }
                else
                {
                    return NotFound("$Producto_Invalido");
                }
                var inventarioProd = new Product4InventariotDto() 
                {
                    ProductId = p.ProductId,
                    Discontinued=p.Discontinued,
                    Barcode = p.Barcode ?? "",
                    Sku = p.Sku ?? "",
                    Name = p.Name,
                    CompanyId = p.CompanyId,
                    UnitsInStock = p.UnitsInStock,
                    TotalCashStock = p.UnitsInStock * p.UnitPrice,
                    UnitsInVentario = p.InVentario,
                    AlertaStockNumProducts = p.AlertaStockNumProducts,
                    InVentarioAlertLevel = p.UnitsInStock <= 0 ? CRITICAL : p.UnitsInStock <= p.AlertaStockNumProducts ? WARNING : NORMAL,
                    CategoryName = p.Category != null ? p.Category.Name : ""

                };
                
                return Ok(inventarioProd);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SetInVentario.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }
        
        /// <summary>
        /// Toma los valores capturados en la columna inventario y sincroniza la columna Stock (Existencias).
        /// Afecta a los productos modificados.
        /// </summary>
        /// <param name="RequestorId"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpPost("SetAllUnitsStock")]
        public async Task<IActionResult> SetAllUnitsStock([FromHeader] string RequestorId, [FromBody] IdDto company)
        {
            try
            {
                var user = await Util.ValidateRequestorSameCompany(RequestorId, Role.Admin, company.Id, _dBContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                if (company.Id <= 0)
                {
                    return BadRequest(new { Message = "$_Empresa_Invalida" });
                }
                string query = "UPDATE [Products] SET UnitsInStock = InVentario, IsInVentarioUpdated=0 WHERE Discontinued = 0 AND IsInVentarioUpdated=1 AND CompanyId=" + company.Id;
                await _dBContext.Database.ExecuteSqlRawAsync(query);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SetAllUnitsStock.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpPost("ResetAllInVentario")]
        public async Task<IActionResult> ResetAllInVentario([FromHeader] string RequestorId, [FromBody] IdDto company)
        {
            try
            {
                var user = await Util.ValidateRequestorSameCompany(RequestorId, Role.Admin, company.Id, _dBContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                if (company.Id <= 0)
                {
                    return BadRequest(new { Message = "$_Empresa_Invalida" });
                }
                string query = "UPDATE [Products] SET InVentario = 0 WHERE CompanyId = " + company.Id;
                await _dBContext.Database.ExecuteSqlRawAsync(query);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ResetAllInVentario.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpPost("SetUnitsInStockByProductId")]
        public async Task<IActionResult> SetUnitsInStockByProductId([FromHeader] string RequestorId, [FromBody] IdDto product, int companyId)
        {
            var newStockVal = 0;
            try
            {
                var user = await Util.ValidateRequestorSameCompany(RequestorId, Role.Admin, companyId, _dBContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                if (product.Id == 0 || companyId <= 0)
                {
                    return BadRequest(new { Message = "$_Empresa_O_Producto_Invalido" });
                }

                var existingProduct = await _dBContext.Products
                    .FirstOrDefaultAsync(p => p.ProductId == product.Id && p.CompanyId == companyId);

                if (existingProduct != null)
                {
                    existingProduct.UnitsInStock = existingProduct.InVentario?? 0;
                    existingProduct.IsInVentarioUpdated = false;

                    _dBContext.Entry(existingProduct).State = EntityState.Modified;
                    await _dBContext.SaveChangesAsync();
                    newStockVal = existingProduct.UnitsInStock;
                }
                else
                {
                    return NotFound("$_Empresa_O_Producto_Invalido");
                }

                return Ok(newStockVal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SetUnitsInStock.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpPost("SetUnitsInStock")]
        public async Task<IActionResult> SetUnitsInStock([FromHeader] string RequestorId, [FromBody] SetStockDto stock, int companyId)
        {
            var newStockVal = 0;
            try
            {
                var user = await Util.ValidateRequestorSameCompany(RequestorId, Role.Admin, companyId, _dBContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                if (stock.ProductId == 0 || companyId <= 0)
                {
                    return BadRequest(new { Message = "$_Empresa_O_Producto_Invalido" });
                }

                var existingProduct = await _dBContext.Products
                    .FirstOrDefaultAsync(p => p.ProductId == stock.ProductId && p.CompanyId == companyId);

                if (existingProduct != null)
                {
                    if (stock.IsHardReset)
                        existingProduct.UnitsInStock = stock.UnitsInStock;
                    else
                        existingProduct.UnitsInStock = existingProduct.UnitsInStock + stock.UnitsInStock;

                    _dBContext.Entry(existingProduct).State = EntityState.Modified;
                    await _dBContext.SaveChangesAsync();
                    newStockVal = existingProduct.UnitsInStock;
                }
                else
                {
                    return NotFound("$_Empresa_O_Producto_Invalido");
                }

                return Ok(newStockVal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SetUnitsInStock.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }
    }
}
