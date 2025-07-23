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
    public class CompanyController : ControllerBase
    {
        private readonly ILogger<CompanyController> _logger;
        private readonly ICompanyService _companyService;
        private readonly DBContext _dBContext;

        public CompanyController(DBContext dBContext, ICompanyService companyService, ILogger<CompanyController> logger)
        {
            _logger = logger;
            _companyService = companyService;
            _dBContext = dBContext;
        }

        [HttpGet("GetAll4List")]
        public async Task<IActionResult> GetAll4List([FromHeader] string RequestorId, int companyId, string name = "-1")
        {
            try
            {
                var requestor = await HeaderValidatorService.ValidateRequestorSameCompanyOrTopRol(RequestorId, companyId, Role.Admin, _dBContext);
                if (requestor.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                var companies = await _companyService.GetAll4List(name);

                return Ok(companies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAll4List.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }


        [HttpGet("GetCompany")]
        public async Task<IActionResult> GetCompany([FromHeader] string RequestorId, int companyId)
        {
            try
            {
                var user = await HeaderValidatorService.ValidateRequestor(RequestorId, Role.Admin, _dBContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                var company = await _companyService.GetCompanyById(companyId);
                return company == null ? NotFound() : Ok(company);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCompany.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }


        [HttpPost("SaveCompany")]
        public async Task<IActionResult> SaveCompany([FromHeader] string RequestorId, [FromBody] CompanyDto companyDto)
        {
            try
            {
                var user = await HeaderValidatorService.ValidateRequestor(RequestorId, Role.Ventas, _dBContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                var savedCompany = await _companyService.SaveCompany(companyDto);
                return Ok(savedCompany);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveCompany.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }


        [HttpPost("SetSettings")]
        public async Task<ActionResult> SetSettings([FromHeader] string RequestorId, [FromBody] CompanySettingsDto settingsDto)
        {
            try
            {
                if (settingsDto.CompanyId <= 0)
                    return NotFound();

                var user = await HeaderValidatorService.ValidateRequestor(RequestorId, Role.Admin, _dBContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                var serializedSettings = Utils.XmlSerializeSettings(settingsDto.Settings);
                var updated = await _companyService.UpdateSettings(settingsDto.CompanyId, serializedSettings);

                return updated ? Ok(0) : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SetSettings.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpGet("GetSettings")]
        public async Task<ActionResult> GetSettings([FromHeader] string RequestorId, int companyId)
        {
            try
            {
                if (companyId <= 0)
                    return NotFound();

                var user = await HeaderValidatorService.ValidateRequestor(RequestorId, Role.Admin, _dBContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                var settingsXml = await _companyService.GetSettings(companyId);
                var settings = string.IsNullOrEmpty(settingsXml) ? null : Utils.DeserializeSettings(settingsXml);

                return Ok(settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetSettings.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }


        [HttpPost("SetCompanyStatus")]
        public async Task<ActionResult> SetCompanyStatus([FromHeader] string RequestorId, [FromBody] SetStatusDto statusDto)
        {
            try
            {
                if (statusDto.ObjectId <= 0)
                    return NotFound();

                var user = await HeaderValidatorService.ValidateRequestor(RequestorId, Role.SysAdmin, _dBContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                var updated = await _companyService.UpdateCompanyStatus(statusDto.ObjectId, statusDto.StatusId);
                return updated ? Ok(statusDto.ObjectId) : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SetCompanyStatus.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpPost("SetCompanyLicense")]
        public async Task<ActionResult> SetCompanyLicense([FromHeader] string RequestorId, [FromBody] SetValueDto valDto)
        {
            try
            {
                if (valDto.ObjectId <= 0)
                    return NotFound();

                var user = await HeaderValidatorService.ValidateRequestor(RequestorId, Role.SysAdmin, _dBContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                var updated = await _companyService.UpdateCompanyLicense(valDto.ObjectId, valDto.ValueId);
                return updated ? Ok(valDto.ObjectId) : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SetCompanyLicense.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpPost("CheckExpiration")]
        public async Task<ActionResult> CheckExpiration(int companyId)
        {
            try
            {
                var companyExists = await _companyService.CheckExpiration(companyId);
                return companyExists ? Ok() : Forbid("$_Pago_requerido");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CheckExpiration.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }


       
        [HttpPost("AddRent")]
        public async Task<ActionResult> AddRent([FromHeader] string RequestorId, [FromBody] RentDto rentDto)
        {
            try
            {
                if (rentDto.CompanyId <= 0)
                    return BadRequest("$_Compañia_Invalida");
                if (rentDto.Amount <= 0)
                    return BadRequest("$_El_monto_debe_ser_mayor_a_cero");

                var user = await HeaderValidatorService.ValidateRequestor(RequestorId, Role.Ventas, _dBContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                var rentId = await _companyService.AddRent(rentDto);
                return rentId.HasValue ? Ok(rentId.Value) : NotFound("$_Compañia_Invalida");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddRent.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpGet("GetPaymentHistory")]
        public async Task<IActionResult> GetPaymentHistory([FromHeader] string RequestorId, int companyId)
        {
            try
            {
                var user = await HeaderValidatorService.ValidateRequestorSameCompanyOrTopRol(RequestorId, companyId, Role.Admin, _dBContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                var history = await _companyService.GetPaymentHistory(companyId);
                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetPaymentHistory.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }


        [HttpPost("IsCompanyUnique")]
        public async Task<IActionResult> IsCompanyUnique([FromBody] string name)
        {
            try
            {
                var result = await _companyService.IsCompanyUnique(name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout.ApiError");
                return StatusCode(500, new { message = "No se puede procesar la solicitud, error de servidor." });
            }
        }

    }
}
