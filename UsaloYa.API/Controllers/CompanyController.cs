using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsaloYa.API.DTO;
using UsaloYa.API.Enums;
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
        public async Task<IActionResult> GetAll([FromHeader] string R)
        {
            try
            {
                var companies = await _dBContext.Companies
                        .OrderBy(c => c.Name)
                        .ToListAsync();

                var companyDtos = companies.Select(c => new CompanyDto
                {
                   Name = c.Name,
                   Address = c.Address?? "",
                   CompanyId = c.CompanyId,
                   PaymentsJson = c.PaymentsJson?? "",
                   LastUpdateBy = c.LastUpdateBy,
                   CreationDate = c.CreationDate,
                   CreatedBy = c.CreatedBy,
                   StatusId = c.StatusId
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

        [HttpGet("GetAll4List")]
        public async Task<IActionResult> GetAll4List([FromHeader] string RequestorId)
        {
            try
            {
                var companies = await _dBContext.Companies
                        .Select(c => new GenericObjectDto {
                                Name = c.Name,
                                CompanyId = c.CompanyId,
                                IsActive = !(c.StatusId == (int)Enums.CompanyStatus.Inactive
                                                || c.StatusId == (int)Enums.CompanyStatus.Expired)
                            })
                        .OrderBy(u => u.Name)
                        .ToListAsync();

                return Ok(companies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAll4List.ApiError");

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
                Address = c.Address ?? "",
                CreatedBy = c.CreatedBy,
                CreationDate = c.CreationDate,
                LastUpdateBy = c.LastUpdateBy,
                PaymentsJson = c.PaymentsJson ?? "",
                StatusId = c.StatusId,
                ExpirationDate = c.ExpirationDate,
                CreatedByUserName = (await _dBContext.Users.FindAsync(c.CreatedBy)).UserName,
                LastUpdateByUserName = (await _dBContext.Users.FindAsync(c.LastUpdateBy)).UserName,


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
                        LastUpdateBy = companyDto.LastUpdateBy,
                        Name = companyDto.Name,
                        CreatedBy = companyDto.CreatedBy,
                        CreationDate = Util.GetMxDateTime(),
                        ExpirationDate = Util.GetMxDateTime(),
                        StatusId = (int)CompanyStatus.Inactive,
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
                    objectToSave.LastUpdateBy = companyDto.LastUpdateBy;
                    objectToSave.Name = companyDto.Name;
                    
                    if(objectToSave.ExpirationDate == null) 
                        objectToSave.ExpirationDate = Util.GetMxDateTime();

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
