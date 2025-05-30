using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsaloYa.API.Config;
using UsaloYa.API.Security;
using UsaloYa.Dto.Enums;
using UsaloYa.Library.Models;
using UsaloYa.Services.interfaces;
using UsaloYa.Services;


namespace UsaloYa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(AccessValidationFilter))]
    public class GeneralController : ControllerBase
    {
        private readonly ILogger<GeneralController> _logger;
        private readonly IGeneralService _generalService;
        private readonly DBContext _dBContext;

        public GeneralController(DBContext dBContext, IGeneralService generalService, ILogger<GeneralController> logger)
        {
            _logger = logger;
            _generalService = generalService;
            _dBContext = dBContext;
        }

        [HttpGet("GetLicenses")]
        public async Task<IActionResult> GetLicenses([FromHeader] string RequestorId)
        {
            try
            {
                var requestor = await HeaderValidatorService.ValidateRequestor(RequestorId, Role.Ventas, _dBContext);
                if (requestor.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                var licenses = await _generalService.GetLicenses();
                return Ok(licenses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetLicenses.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }
    }
}
