using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;
using UsaloYa.API.Config;
using UsaloYa.Dto;
using UsaloYa.Dto.Enums;
using UsaloYa.Library.Models;
using UsaloYa.API.Security;
using UsaloYa.Services;
using UsaloYa.Services.interfaces;

namespace UsaloYa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(AccessValidationFilter))]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _CategoryService;
        private readonly ILogger<CategoryController> _logger;
        private readonly DBContext _dBContext;
        private readonly ProductCategoryService _productCategoryService;

        public CategoryController(DBContext dBContext, ILogger<CategoryController> logger, ProductCategoryService prodCatService, ICategoryService categoryService)
        {
            _logger = logger;
            _dBContext = dBContext;
            _productCategoryService = prodCatService;
            _CategoryService = categoryService;
        }

        [HttpGet("GetAll4List")]
        public async Task<IActionResult> GetAll4List([FromHeader] string RequestorId, int companyId, string keyword)
        {
            try
            {
                var requestor = await HeaderValidatorService.ValidateRequestorSameCompany(RequestorId, Role.User, companyId, _dBContext);

                if (requestor.UserId <= 0)
                {
                    return Unauthorized(AppConfig.NO_AUTORIZADO);
                }
                var ListAll = await _CategoryService.GetAll4List(companyId, keyword);
                return Ok(ListAll); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAll4List.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpPost("DeleteCategory")]
        public async Task<IActionResult> DeleteCategory([FromHeader] string RequestorId, ProductCategoryDto categoryInfo, int companyId)
        {
            try
            {
                var user = await HeaderValidatorService.ValidateRequestorSameCompany(RequestorId, Role.Admin, companyId, _dBContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                var c = await _dBContext.ProductCategories
                    .FirstOrDefaultAsync(u => u.CompanyId == companyId && u.CategoryId == categoryInfo.CategoryId);
                if (c == null)
                    return NotFound();

                _dBContext.ProductCategories.Remove(c);

                await _dBContext.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteCategory.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }


        [HttpGet("GetCategory")]
        public async Task<IActionResult> GetCategory([FromHeader] string RequestorId, int categoryId, int companyId)
        {
            try
            {
                var user = await HeaderValidatorService.ValidateRequestorSameCompany(RequestorId, Role.Admin, companyId, _dBContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                var catInfo = await _productCategoryService.GetCategory(categoryId, companyId);
                if (catInfo == null)
                    return NotFound();

                return Ok(catInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCategory.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpPost("SaveCategory")]
        public async Task<ActionResult> SaveCategory([FromHeader] string RequestorId, [FromBody] ProductCategoryDto categoryDto)
        {
            ProductCategory objectToSave;
            try
            {
                var user = await HeaderValidatorService.ValidateRequestor(RequestorId, Role.Admin, _dBContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                var existingCategoryName = await _dBContext.ProductCategories
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == categoryDto.Name.ToLower() 
                                                && c.CompanyId == categoryDto.CompanyId
                                                && c.CategoryId != categoryDto.CategoryId);
                if (existingCategoryName != null)
                    return Conflict("El nombre de la categoria ya existe");
                

                var catInfo = await _productCategoryService.SaveCategory(categoryDto);
                if (catInfo == null)
                    return NotFound();

                return Ok(catInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveCategory.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

    }
}
