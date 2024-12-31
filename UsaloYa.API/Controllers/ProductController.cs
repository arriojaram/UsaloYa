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
                    .Select(p => new Product4ListDto() { ProductId=p.ProductId, Discontinued=p.Discontinued, Sku=p.Sku, Description=p.Description, Name=p.Name,  CompanyId =  p.CompanyId })
                    .Where(p => p.CompanyId == companyId)
                    .OrderBy(p=> p.Name)
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
            int pageSize = 30;
            if (pageNumber == -1) //Used to manage the cache products function
            {
                pageSize = 4000;
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
                    .Where(p => p.CompanyId == companyId)
                    .OrderBy(p => p.Name)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync()

                    : await _dBContext.Products
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

                return Ok(products.Select(p => new ProductDto(){ 
                    Barcode = p.Barcode,
                    Name = p.Name,
                    Brand = p.Brand,
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
                       
                        BuyPrice = productDto.BuyPrice == 1 ? null : productDto.BuyPrice,
                        UnitPrice = productDto.UnitPrice == 0 ? 1 : productDto.UnitPrice,
                        UnitPrice1 = productDto.UnitPrice1 == 0 ? null : productDto.UnitPrice1,
                        UnitPrice2 = productDto.UnitPrice2 == 0 ? null : productDto.UnitPrice2,
                        UnitPrice3 = productDto.UnitPrice3 == 0 ? null : productDto.UnitPrice3,

                        UnitsInStock = productDto.UnitsInStock,
                        Discontinued = productDto.Discontinued,
                        DateModified = Utils.Util.GetMxDateTime(),
                       
                        Sku = (string.IsNullOrEmpty(productDto.SKU) ? null: productDto.SKU),
                        Barcode = productDto.Barcode,
                        Brand = string.IsNullOrEmpty( productDto.Brand) ? null: productDto.Brand,
                        DateAdded = Utils.Util.GetMxDateTime(),
                        CompanyId = companyId
                    };

                    _dBContext.Products.Add(existingProduct);
                    
                }
                else if (existingProduct != null)
                {
                    existingProduct.Name = productDto.Name.Trim();
                    existingProduct.Description = productDto.Description;
      
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
                    existingProduct.Brand = productDto.Brand;
     

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

    }
    }
