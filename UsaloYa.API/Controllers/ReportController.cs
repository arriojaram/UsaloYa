using Microsoft.AspNetCore.Mvc;
using UsaloYa.API.Models;

namespace UsaloYa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly ILogger<ReportController> _logger;
        private readonly DBContext _dBContext;

        public ReportController(DBContext dBContext, ILogger<ReportController> logger)
        {
            _logger = logger;
            _dBContext = dBContext;
        }

        [HttpGet("GetSalesReport")]
        public IActionResult GetSalesReport([FromQuery] DateTime fromDate, DateTime toDate, int companyId, int userId = 0)
        {
            try
            {
                if (fromDate.Date == toDate.Date)
                {
                    toDate = fromDate.AddDays(1);
                }

                var result = from s in _dBContext.Sales
                             join u in _dBContext.Users on s.UserId equals u.UserId
                             where s.CompanyId == companyId
                                && (u.UserId == userId || userId == 0)
                                && s.SaleDate >= fromDate.Date && s.SaleDate <= toDate.Date
                             select new
                             {
                                 SaleID = s.SaleId,
                                 SaleDate = s.SaleDate,
                                 UserId = s.UserId,
                                 UserName = u.UserName,
                                 FullName = u.FirstName + ' ' +u.LastName,
                                 Notes = s.Notes,
                                 Status = s.Status,
                                 TotalSale = s.TotalSale
                             };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetSalesReport.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "$_ExcepcionOcurrida" });
            }
        }

        [HttpGet("GetSaleDetails")]
        public IActionResult GetSaleDetails([FromQuery] int saleId, int companyId)
        {
            try
            {
                var result = from sd in _dBContext.SaleDetails
                             join p in _dBContext.Products on sd.ProductId equals p.ProductId
                             join s in _dBContext.Sales on sd.SaleId equals s.SaleId
                             join u in _dBContext.Users on s.UserId equals u.UserId
                             where s.SaleId == saleId && s.CompanyId == companyId
                             select new
                             {
                                 Barcode = p.Barcode,
                                 ProductName = p.Name,
                                 Quantity = sd.Quantity,
                                 TotalPrice = sd.TotalPrice,
                                 SaleID = s.SaleId,
                                 SaleDate = s.SaleDate,
                                 TotalSale = s.TotalSale,
                                 UserId = s.UserId,
                                 UserName = u.UserName,
                                 FullName = u.FirstName + ' ' + u.LastName,

                             };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetSaleDetails.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "$_ExcepcionOcurrida" });
            }
        }
    }
}
