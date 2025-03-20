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
    public class GeneralController : Controller
    {
        private readonly ILogger<GeneralController> _logger;
        private readonly DBContext _dBContext;

        public GeneralController(DBContext dBContext, ILogger<GeneralController> logger)
        {
            _logger = logger;
            _dBContext = dBContext;
        }

        [HttpGet("GetLicenses")]
        public async Task<IActionResult> GetLicenses([FromHeader] string RequestorId)
        {
            try
            {
                var requestor = await Util.ValidateRequestor(RequestorId, Role.Ventas, _dBContext);

                if (requestor.UserId <= 0)
                {
                    return Unauthorized(AppConfig.NO_AUTORIZADO);
                }

                var l = await _dBContext.PlanRentas.OrderBy(u => u.Name).ToListAsync();
              
                return Ok(l.Select(c => new LicenseDto
                {
                   Id = c.Id,
                   Name = c.Name,
                   Notes = c.Notes,
                   NumUsers = c.NumUsers,
                   Price = c.Price,
                   StatusId = c.StatusId
                   
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetLicenses.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }


    }
}
