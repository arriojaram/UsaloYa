using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsaloYa.Library.Config;
using UsaloYa.API.Security;
using UsaloYa.Dto;
using UsaloYa.Dto.Enums;
using UsaloYa.Dto.Utils;
using UsaloYa.Library.Models;
using UsaloYa.Services;
using UsaloYa.Services.interfaces;

namespace UsaloYa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(AccessValidationFilter))]

    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _productService;
        private readonly DBContext _dbContext;


        public ProductController(IProductService productService, ILogger<ProductController> logger, DBContext dbContext)
        {
            _logger = logger;
            _productService = productService;
            _dbContext = dbContext;
        }


        [HttpGet("FilterProducts")]
        public async Task<IActionResult> FilterProducts(int pageNumber, int categoryId, int companyId)
        {
            try
            {
                if (companyId <= 0)
                    return BadRequest(new { Message = "$_Empresa_O_Producto_Invalido" });

                var products = await _productService.FilterProducts(pageNumber, categoryId, companyId);
                return products.Any() ? Ok(products) : NotFound();
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
            try
            {
                if (string.IsNullOrEmpty(keyword) || companyId <= 0)
                    return BadRequest(new { Message = "$_Empresa_O_Producto_Invalido" });

                var products = await _productService.SearchProduct4List(pageNumber, keyword, companyId);
                return products.Any() ? Ok(products) : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SearchProduct4List.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }


        [HttpGet("SearchProductFull")]
        public async Task<IActionResult> SearchProductFull(int pageNumber, string keyword, int companyId)
        {
            try
            {
                if (string.IsNullOrEmpty(keyword) || companyId <= 0)
                    return BadRequest(new { Message = "$_Empresa_O_Producto_Invalido" });

                var products = await _productService.SearchProductFull(pageNumber, keyword, companyId);
                return products.Any() ? Ok(products) : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SearchProductFull.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }


        [HttpGet("GetProduct")]
        public async Task<IActionResult> GetProduct(int productId, int companyId)
        {
            try
            {
                if (productId <= 0 || companyId <= 0)
                    return BadRequest(new { Message = "$_Empresa_O_Producto_Invalido" });

                var product = await _productService.GetProduct(productId, companyId);
                return product != null ? Ok(product) : NotFound();
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
                var success = await _productService.ImportProduct(RequestorId, productDto, companyId);
                return success ? Ok() : Conflict(new { Message = "$_Producto_Con_Mismo_Codigo_De_Barras_Ya_Existe" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ImportProduct.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }


        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromHeader] string RequestorId, [FromBody] ProductDto productDto, int companyId)
        {
            try
            {
                var product = await _productService.AddProduct(RequestorId, productDto, companyId);
                return product != null ? Ok(product) : Conflict(new { Message = "$_Producto_Con_Mismo_Codigo_De_Barras_Ya_Existe" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddProduct.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }


        [HttpGet("GetInventarioByCategoryId")]
        public async Task<IActionResult> GetInventarioByCategoryId([FromHeader] int categoryId, int companyId, int pageNumber)
        {
            try
            {
                var products = await _productService.GetInventarioByCategoryId(categoryId, companyId, pageNumber);
                return products.Any() ? Ok(products) : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetInventarioByCategoryId.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }


        [HttpGet("GetInventarioByAlertId")]
        public async Task<IActionResult> GetInventarioByAlertId([FromHeader] string RequestorId, int alertLevel, int companyId, int pageNumber)
        {
            try
            {
                var user = await HeaderValidatorService.ValidateRequestorSameCompany(RequestorId, Role.User, companyId, _dbContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                if (companyId <= 0)
                    return BadRequest(new { Message = "$_Empresa_O_Producto_Invalido" });

                var products = await _productService.GetInventarioByAlertId(alertLevel, companyId, pageNumber);
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
            try
            {
                var user = await HeaderValidatorService.ValidateRequestorSameCompany(RequestorId, Role.User, companyId, _dbContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);
               

                if (companyId <= 0)
                    return BadRequest(new { Message = "$_Empresa_O_Producto_Invalido" });

                var products = await _productService.GetInventarioWithDiscrepancias(companyId, pageNumber);

                if (products == null || products.Count == 0)
                    return NotFound();

                return Ok(products);
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
            try
            {
                var user = await HeaderValidatorService.ValidateRequestorSameCompany(RequestorId, Role.User, companyId, _dbContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                if (string.IsNullOrEmpty(keyword) || companyId <= 0)
                    return BadRequest(new { Message = "$_Empresa_O_Producto_Invalido" });

                var inventory = await _productService.GetInventarioTop50(keyword, companyId, pageNumber);
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
            try
            {
                var user = await HeaderValidatorService.ValidateRequestorSameCompany(RequestorId, Role.User, companyId, _dbContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                var products = await _productService.GetInventarioItemsUpdated(companyId, pageNumber);
                return Ok(products);
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
            try
            {
                var user = await HeaderValidatorService.ValidateRequestorSameCompany(RequestorId, Role.Admin, companyId, _dbContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                if (string.IsNullOrEmpty(barcodeDto.Code) || companyId <= 0)
                    return BadRequest(new { Message = "$_Empresa_O_Producto_Invalido" });

                var product = await _productService.SetInVentario(barcodeDto.Code, barcodeDto.Quantity, companyId);
                return product != null ? Ok(product) : NotFound("$Producto_Invalido");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SetInVentario.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpPost("SetAllUnitsStock")]
        public async Task<IActionResult> SetAllUnitsStock([FromHeader] string RequestorId, [FromBody] IdDto company)
        {
            try
            {
                var user = await HeaderValidatorService.ValidateRequestorSameCompany(RequestorId, Role.Admin, company.Id, _dbContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                if (company.Id <= 0)
                    return BadRequest(new { Message = "$_Empresa_Invalida" });

                await _productService.SetAllUnitsStock(company.Id);
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
                var user = await HeaderValidatorService.ValidateRequestorSameCompany(RequestorId, Role.Admin, company.Id, _dbContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                if (company.Id <= 0)
                    return BadRequest(new { Message = "$_Empresa_Invalida" });

                await _productService.ResetAllInVentario(company.Id);
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
            try
            {
                var user = await HeaderValidatorService.ValidateRequestorSameCompany(RequestorId, Role.Admin, companyId, _dbContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                if (product.Id == 0 || companyId <= 0)
                    return BadRequest(new { Message = "$_Empresa_O_Producto_Invalido" });

                var newStockVal = await _productService.SetUnitsInStockByProductId(product.Id, companyId);
                return newStockVal != -1 ? Ok(newStockVal) : NotFound("$_Empresa_O_Producto_Invalido");
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
            try
            {
                var user = await HeaderValidatorService.ValidateRequestorSameCompany(RequestorId, Role.Admin, companyId, _dbContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                if (stock.ProductId == 0 || companyId <= 0)
                    return BadRequest(new { Message = "$_Empresa_O_Producto_Invalido" });

                var newStockVal = await _productService.SetUnitsInStock(stock.ProductId, stock.UnitsInStock, stock.IsHardReset, companyId);
                return newStockVal != -1 ? Ok(newStockVal) : NotFound("$_Empresa_O_Producto_Invalido");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SetUnitsInStock.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }
    }
}
