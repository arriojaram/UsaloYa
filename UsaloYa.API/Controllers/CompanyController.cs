using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsaloYa.API.DTO;
using UsaloYa.API.Models;
using UsaloYa.API.Utils;

namespace UsaloYa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : Controller
    {
        private readonly ILogger<CompanyController> _logger;
        private readonly DBContext _dBContext;

        public CompanyController(DBContext dBContext, ILogger<CompanyController> logger)
        {
            _logger = logger;
            _dBContext = dBContext;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var companies = await _dBContext.Companies
                        .OrderBy(u => u.Name)
                        .ToListAsync();

                var companyDtos = companies.Select(u => new CompanyDto
                {
                   Name = u.Name,
                   Address = u.Address?? "",
                   CompanyId = u.CompanyId,
                   PaymentsJson = u.PaymentsJson?? "",
                   LastUpdatedBy = u.LastUpdateBy,
                   CreationDate = u.CreationDate,
                   CreatedBy = u.CreatedBy
                });

                return Ok(companyDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAll.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }


        [HttpGet("GetCompany")]
        public async Task<IActionResult> GetCompany(int companyId)
        {
            var c = await _dBContext.Companies.FirstOrDefaultAsync(u => u.CompanyId == companyId);
            if (c == null)
                return NotFound();

            var companyResponseDto = new CompanyDto()
            {
               CompanyId = c.CompanyId,
               Name = c.Name,
               Address = c.Address?? "",
               CreatedBy = c.CreatedBy,
               CreationDate = c.CreationDate,
               LastUpdatedBy = c.LastUpdateBy,
               PaymentsJson = c.PaymentsJson?? ""
            };

            return Ok(companyResponseDto);
        }

        [HttpPost("SaveCompany")]
        public async Task<ActionResult> SaveCompany([FromBody] CompanyDto companyDto)
        {
            Company objectToSave = null;
            try
            {
                if (companyDto.CompanyId == 0)
                {
                    var existsObject = await _dBContext.Companies.AnyAsync(
                            c => c.Name.ToLower() == companyDto.Name.ToLower());

                    if (existsObject)
                        return Conflict(new { message = "$_Negocio_Existente" });

                    objectToSave = new Company
                    {
                        Address = companyDto.Address,
                        LastUpdateBy = companyDto.LastUpdatedBy,
                        Name = companyDto.Name,
                        CreatedBy = companyDto.CreatedBy,
                        CreationDate = Util.GetMxDateTime(),
                        PaymentsJson = ""
                    };
                    _dBContext.Companies.Add(objectToSave);
                }
                else
                {
                    objectToSave = await _dBContext.Companies.FindAsync(companyDto.CompanyId);
                    if (objectToSave == null)
                        return NotFound();

                    objectToSave.Address = companyDto.Address;
                    objectToSave.LastUpdateBy = companyDto.LastUpdatedBy;
                    objectToSave.Name = companyDto.Name;
                    
                    _dBContext.Companies.Update(objectToSave);
                }

                await _dBContext.SaveChangesAsync();
                return Ok(objectToSave);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveCompany.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }
    }
}
