using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsaloYa.API.Config;
using UsaloYa.API.DTO;
using UsaloYa.API.Enums;
using UsaloYa.API.Models;
using UsaloYa.API.Security;
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

        public CategoryController(DBContext dBContext, ILogger<CategoryController> logger)
        {
            _logger = logger;
            _dBContext = dBContext;
        }

        [HttpGet("GetAll4List")]
        public async Task<IActionResult> GetAll4List([FromHeader] string RequestorId, int companyId)
        {
            try
            {
                var requestor = await Util.ValidateRequestorSameCompany(RequestorId, Role.Admin, companyId, _dBContext);

                if (requestor.UserId <= 0)
                {
                    return Unauthorized(AppConfig.NO_AUTORIZADO);
                }

                var categories = await _dBContext.ProductCategories
                                            .Where(c => c.CompanyId == companyId)
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

        [HttpGet("GetCategory")]
        public async Task<IActionResult> GetCategory([FromHeader] string RequestorId, int categoryId, int companyId)
        {
            var user = await Util.ValidateRequestorSameCompany(RequestorId, Role.Admin, companyId, _dBContext);
            if (user.UserId <= 0)
                return Unauthorized(AppConfig.NO_AUTORIZADO);

            var c = await _dBContext.ProductCategories
                .FirstOrDefaultAsync(u => u.CompanyId == companyId && u.CategoryId == categoryId);
            if (c == null)
                return NotFound();

            var responseDto = new ProductCategoryDto()
            {
                CategoryId = c.CategoryId,
                Description = c.Description?? "",
                Name = c.Name
            };

            return Ok(responseDto);
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

                if (categoryDto.CategoryId == 0)
                {
                    
                    objectToSave = new ProductCategory
                    {
                       Name = categoryDto.Name,
                       CategoryId = categoryDto.CategoryId,
                       CompanyId = categoryDto.CompanyId,
                       Description = categoryDto.Description
                    };
                    _dBContext.ProductCategories.Add(objectToSave);
                }
                else
                {
                    objectToSave = await _dBContext.ProductCategories.FindAsync(categoryDto.CategoryId);
                    if (objectToSave == null)
                        return NotFound();

                    objectToSave.Description = categoryDto.Description;
                    objectToSave.Name = categoryDto.Name;

                    _dBContext.Entry(objectToSave).State = EntityState.Modified;

                }

                await _dBContext.SaveChangesAsync();
                return Ok(new ProductCategoryDto() { 
                    Name = categoryDto.Name,
                    CategoryId = objectToSave.CategoryId,
                    CompanyId = categoryDto.CompanyId,
                    Description = categoryDto.Description
                });
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
