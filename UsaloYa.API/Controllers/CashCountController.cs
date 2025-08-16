using Microsoft.AspNetCore.Mvc;
using UsaloYa.Services.Interfaces;
using UsaloYa.Dto;
using UsaloYa.Services;
using UsaloYa.Dto.Enums;
using UsaloYa.Library.Config;
using UsaloYa.API.Security;

namespace UsaloYa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(AccessValidationFilter))]
    public class CashCountController : ControllerBase
    {
        private readonly ILogger<CashCountController> _logger;
        private readonly ICashCounter _cashCounterService;
        private readonly HeaderValidatorService _headerValidatorService;
        public CashCountController(ICashCounter cashCounterService, HeaderValidatorService headerValidatorService, ILogger<CashCountController> logger)
        {
            _cashCounterService = cashCounterService;
            _headerValidatorService = headerValidatorService;
            _logger = logger;
        }

        [HttpGet("GetAllByCompany")]
        public async Task<IActionResult> GetAllByCompany([FromHeader] string RequestorId, int companyId)
        {
            try
            {
                var requestor = await _headerValidatorService.ValidateRequestorSameCompanyOrTopRol(RequestorId, companyId, Role.Admin);
                if (requestor.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                var cashCounts = await _cashCounterService.GetAllByCompany(companyId);
                return Ok(cashCounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllByCompany.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }


        [HttpGet("Get")]
        public async Task<IActionResult> Get([FromHeader] string RequestorId, int companyId, int id)
        {
            try
            {
                var requestor = await _headerValidatorService.ValidateRequestorSameCompanyOrTopRol(RequestorId, companyId, Role.Admin);
                if (requestor.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                var cashCount = await _cashCounterService.Get(id);
                if (cashCount == null)
                {
                    return NotFound($"CashCount con ID {id} no encontrado");
                }
                return Ok(cashCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpPost("Save")]
        public async Task<IActionResult> Save([FromHeader] string RequestorId, int companyId, [FromBody] CashCountDto cashCountDto)
        {
            try
            {
                var requestor = await _headerValidatorService.ValidateRequestorSameCompanyOrTopRol(RequestorId, companyId, Role.Admin);
                if (requestor.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var id = await _cashCounterService.Save(cashCountDto);
                return Ok(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Save.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }
    }
}
