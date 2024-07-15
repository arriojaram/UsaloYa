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
                    return BadRequest(new { Message = "$_EmpresaOProductoInvalido" });
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
                return StatusCode(500, new { message = "$_ExcepcionOcurrida" });
            }
        }

        [HttpGet("GetProduct")]
        public async Task<IActionResult> GetProduct(int productId, int companyId)
        {
            try
            {
                if (productId <= 0 || companyId <= 0)
                {
                    return BadRequest(new { Message = "$_EmpresaOProductoInvalido" });
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
                return StatusCode(500, new { message = "$_ExcepcionOcurrida" });
            }
        }

        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromBody] ProductDto productDto, int companyId)
        {
            
            try
            {
                if (productDto.Equals(default(ProductDto)) || companyId <= 0)
                {
                    return BadRequest(new { Message = "$_EmpresaOProductoInvalido" });
                }

                var existingProduct = await _dBContext.Products
                    .FirstOrDefaultAsync(p => p.ProductId == productDto.ProductId && p.CompanyId == companyId);

                // Check if a product with the same Barcode and SKU already exists
                var productWithSameBarcodeAndSku = await _dBContext.Products
                    .FirstOrDefaultAsync(p => p.Barcode == productDto.Barcode && p.Sku == productDto.SKU && p.CompanyId == companyId);

                if (productWithSameBarcodeAndSku != null && productWithSameBarcodeAndSku.ProductId != productDto.ProductId)
                {
                    return Conflict(new { Message = "$_ProductoConMismoCodigoDeBarrasYaExiste" });
                }

                if (existingProduct == null && productDto.ProductId == 0)
                {
                    existingProduct = new Product
                    {
                        Name = productDto.Name.Trim(),
                        Description = productDto.Description,
                        CategoryId = productDto.CategoryId,
                        SupplierId = productDto.SupplierId,
                        UnitPrice = productDto.UnitPrice,
                        UnitsInStock = productDto.UnitsInStock,
                        Discontinued = productDto.Discontinued,
                        ImgUrl = productDto.ImgUrl,
                        DateModified = Utils.Util.GetMxDateTime(),
                        Weight = productDto.Weight,
                        Sku = productDto.SKU,
                        Barcode = productDto.Barcode,
                        Brand = productDto.Brand,
                        Color = productDto.Color,
                        Size = productDto.Size,
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
                    existingProduct.UnitPrice = productDto.UnitPrice;
                    existingProduct.UnitsInStock = productDto.UnitsInStock;
                    existingProduct.Discontinued = productDto.Discontinued;
                    existingProduct.ImgUrl = productDto.ImgUrl;
                    existingProduct.DateModified = Utils.Util.GetMxDateTime();
                    existingProduct.Weight = productDto.Weight;
                    existingProduct.Sku = productDto.SKU;
                    existingProduct.Barcode = productDto.Barcode;
                    existingProduct.Brand = productDto.Brand;
                    existingProduct.Color = productDto.Color;
                    existingProduct.Size = productDto.Size;

                    _dBContext.Products.Update(existingProduct);
                    
                }
                else
                {
                    return NotFound("$_EmpresaOProductoInvalido");
                }
                await _dBContext.SaveChangesAsync();
                return Ok(existingProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddProduct.ApiError");
                return StatusCode(500, new { message = "$_ExcepcionOcurrida" });
            }
        }

    }
    }
