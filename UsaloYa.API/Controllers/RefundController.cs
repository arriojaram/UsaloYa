using Microsoft.AspNetCore.Mvc;
using UsaloYa.API.Security;
using UsaloYa.Dto;
using UsaloYa.Dto.Utils;
using UsaloYa.Dto.Enums;
using UsaloYa.Library.Config;
using UsaloYa.Library.Models;
using UsaloYa.Services;
using UsaloYa.Services.Interfaces;

namespace UsaloYa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(AccessValidationFilter))]
    public class RefundController : ControllerBase
    {
        private readonly ILogger<RefundController> _logger;
        private readonly IRefundService _refundService;
        private readonly DBContext _dBContext;
        private readonly HeaderValidatorService _headerValidatorService;

        public RefundController(DBContext dBContext, IRefundService refundService, ILogger<RefundController> logger, HeaderValidatorService headerValidatorService)
        {
            _refundService = refundService;
            _dBContext = dBContext;
            _logger = logger;
            _headerValidatorService = headerValidatorService;
        }

        [HttpGet("CanSaleBeRefund")]
        public async Task<IActionResult> CanSaleBeRefund([FromHeader] string RequestorId, int companyId,[FromHeader] DateTime dateTimeSale)
        {
            try
            {
                var requestor = await _headerValidatorService.ValidateRequestorSameCompany(RequestorId, Role.User, companyId);
                if (requestor.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);
                var result = await _refundService.CanSaleBeRefund(dateTimeSale, companyId);
                return Ok(result);


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetMaxDaysToRefund.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpPost("ManageRefund")]
        public async Task<IActionResult> ManageRefund([FromHeader] string RequestorId, int companyId,[FromBody] RequestRefundDto requestRefundDto)
        {
            try
            {
                var requestor = await _headerValidatorService.ValidateRequestorSameCompany(RequestorId, Role.User, companyId);
                if (requestor.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);
                var result = await _refundService.ManageRefund(requestRefundDto, companyId);
                return Ok(result);
              
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetSalesReport.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }
    }
}
