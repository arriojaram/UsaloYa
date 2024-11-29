using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsaloYa.API.DTO;
using UsaloYa.API.Models;

namespace UsaloYa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly DBContext _dBContext;

        public ProductController(DBContext dBContext, ILogger<ProductController> logger)
        {
            _logger = logger;
            _dBContext = dBContext;
        }

        [HttpGet("SearchProduct")]
        public async Task<IActionResult> SearchProduct(string keyword, int companyId)
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
                    .Where(p => p.CompanyId == companyId).Take(500)
                    .ToListAsync()

                    : await _dBContext.Products
                    .Where(p => p.CompanyId == companyId &&
                                (p.Name.Contains(keyword) || keyword.Contains(p.Name)
                                    || p.Description != null && p.Description.Contains(keyword)
                                    || p.Sku != null && p.Sku.Contains(keyword)))
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

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetProduct.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromBody] ProductDto productDto, int companyId)
        {
            try
            {
                if (productDto.Equals(default(ProductDto)) || companyId <= 0)
                {
                    return BadRequest(new { Message = "$_Empresa_O_Producto_Invalido" });
                }

                var existingProduct = await _dBContext.Products
                    .FirstOrDefaultAsync(p => p.ProductId == productDto.ProductId && p.CompanyId == companyId);

                // Check if a product with the same Barcode and SKU already exists
                var productWithSameBarcodeAndSku = _dBContext.Products
                    .Where(p => (p.Barcode.Equals(productDto.Barcode) || p.Sku.Equals(productDto.SKU?? "$SKU"))
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
                        CategoryId = productDto.CategoryId == 0? null: productDto.CategoryId,
                        SupplierId = productDto.SupplierId == 0? null: productDto.SupplierId,
                        BuyPrice = productDto.BuyPrice == 1 ? null : productDto.BuyPrice,
                        UnitPrice = productDto.UnitPrice == 0 ? 1 : productDto.UnitPrice,
                        UnitPrice1 = productDto.UnitPrice1 == 0 ? null : productDto.UnitPrice1,
                        UnitPrice2 = productDto.UnitPrice2 == 0 ? null : productDto.UnitPrice2,
                        UnitPrice3 = productDto.UnitPrice3 == 0 ? null : productDto.UnitPrice3,

                        UnitsInStock = productDto.UnitsInStock,
                        Discontinued = productDto.Discontinued,
                        ImgUrl = string.IsNullOrEmpty(productDto.ImgUrl) ? null : productDto.ImgUrl,
                        DateModified = Utils.Util.GetMxDateTime(),
                        Weight = productDto.Weight,
                        Sku = (string.IsNullOrEmpty(productDto.SKU) ? null: productDto.SKU),
                        Barcode = productDto.Barcode,
                        Brand = string.IsNullOrEmpty( productDto.Brand) ? null: productDto.Brand,
                        Color = string.IsNullOrEmpty(productDto.Color) ? null: productDto.Color,
                        Size = string.IsNullOrEmpty(productDto.Size) ? null: productDto.Size,
                        DateAdded = Utils.Util.GetMxDateTime(),
                        CompanyId = companyId
                    };

                    _dBContext.Products.Add(existingProduct);
                    
                }
                else if (existingProduct != null)
                {
                    existingProduct.Name = productDto.Name.Trim();
                    existingProduct.Description = productDto.Description;
                    existingProduct.CategoryId = productDto.CategoryId;
                    existingProduct.SupplierId = productDto.SupplierId;
                    existingProduct.BuyPrice = productDto.BuyPrice;
                    existingProduct.UnitPrice = productDto.UnitPrice;
                    existingProduct.UnitPrice1 = productDto.UnitPrice1;
                    existingProduct.UnitPrice2 = productDto.UnitPrice2;
                    existingProduct.UnitPrice3 = productDto.UnitPrice3;

                    existingProduct.UnitsInStock = productDto.UnitsInStock;
                    existingProduct.Discontinued = productDto.Discontinued;
                    existingProduct.ImgUrl = productDto.ImgUrl;
                    existingProduct.DateModified = Utils.Util.GetMxDateTime();
                    existingProduct.Weight = productDto.Weight;
                    existingProduct.Sku = (string.IsNullOrEmpty(productDto.SKU) ? null : productDto.SKU);
                    existingProduct.Barcode = productDto.Barcode;
                    existingProduct.Brand = productDto.Brand;
                    existingProduct.Color = productDto.Color;
                    existingProduct.Size = productDto.Size;

                    _dBContext.Products.Update(existingProduct);
                    
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

    }
    }
