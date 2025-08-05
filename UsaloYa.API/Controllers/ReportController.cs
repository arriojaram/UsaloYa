using Microsoft.AspNetCore.Mvc;
using UsaloYa.API.Security;
using UsaloYa.Library.Models;
using UsaloYa.Services.Interfaces;

namespace UsaloYa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(AccessValidationFilter))]
    public class ReportController : ControllerBase
    {
        private readonly ILogger<ReportController> _logger;
        private readonly IReportSaleService _reportService;
        private readonly IReportRefundService _reportRefundService;
        private readonly DBContext _dBContext;

        public ReportController(DBContext dBContext, IReportSaleService reportService, ILogger<ReportController> logger, IReportRefundService reportRefundService)
        {
            _logger = logger;
            _reportService = reportService;
            _dBContext = dBContext;
            _reportRefundService = reportRefundService;
        }

        [HttpGet("GetSalesReport")]
        public async Task<IActionResult> GetSalesReport([FromQuery] DateTime fromDate, DateTime toDate, int companyId, int userId = 0)
        {
            try
            {
                var result = await _reportService.GetSalesReport(fromDate, toDate, companyId, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetSalesReport.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpGet("GetSaleDetails")]
        public async Task<IActionResult> GetSaleDetails([FromQuery] int saleId, int companyId)
        {
            try
            {
                var result = await _reportService.GetSaleDetails(saleId, companyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetSaleDetails.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpGet("GetRefundsReport")]
        public async Task<IActionResult> GetRefundsReport([FromQuery] DateTime fromDate, DateTime toDate, int companyId)
        {
            try
            {
                var result = await _reportRefundService.GetRefundsReport(fromDate, toDate, companyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetSalesReport.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpGet("GetRefundDetails")]
        public async Task<IActionResult> GetRefundDetails([FromQuery] int saleId, int companyId)
        {
            try
            {
                var result = await _reportRefundService.GetRefundDetails(saleId, companyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetSaleDetails.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }
    }
}
