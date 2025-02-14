using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;
using UsaloYa.API.Config;
using UsaloYa.API.DTO;
using UsaloYa.API.Enums;
using UsaloYa.API.Models;
using UsaloYa.API.Security;
using UsaloYa.API.Services;
using UsaloYa.API.Utils;

namespace UsaloYa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(AccessValidationFilter))]
    public class CategoryController : Controller
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly DBContext _dBContext;
        private readonly ProductCategoryService _productCategoryService;

        public CategoryController(DBContext dBContext, ILogger<CategoryController> logger, ProductCategoryService prodCatService)
        {
            _logger = logger;
            _dBContext = dBContext;
            _productCategoryService = prodCatService;
        }

        [HttpGet("GetAll4List")]
        public async Task<IActionResult> GetAll4List([FromHeader] string RequestorId, int companyId, string keyword)
        {
            try
            {
                var requestor = await Util.ValidateRequestorSameCompany(RequestorId, Role.Admin, companyId, _dBContext);

                if (requestor.UserId <= 0)
                {
                    return Unauthorized(AppConfig.NO_AUTORIZADO);
                }

                var categories = keyword == "-1" ?
                    await _dBContext.ProductCategories
                                            .Where(c => c.CompanyId == companyId)
                                            .OrderBy(u => u.Name)
                                            .ToListAsync()
                    :
                    await _dBContext.ProductCategories
                                            .Where(c => c.CompanyId == companyId
                                                        && c.Name.Contains(keyword))
                                            .OrderBy(u => u.Name)
                                            .ToListAsync();

                return Ok(categories.Select(c => new ProductCategoryDto
                {
                    CategoryId = c.CategoryId,
                    Description = c.Description?? "",
                    Name = c.Name,
                    CompanyId = c.CompanyId
                }));
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
                var user = await Util.ValidateRequestorSameCompany(RequestorId, Role.Admin, companyId, _dBContext);
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
                var user = await Util.ValidateRequestorSameCompany(RequestorId, Role.Admin, companyId, _dBContext);
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
                var user = await Util.ValidateRequestor(RequestorId, Role.Admin, _dBContext);
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
