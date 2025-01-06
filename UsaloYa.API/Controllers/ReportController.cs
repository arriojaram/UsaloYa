using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsaloYa.API.DTO;
using UsaloYa.API.Models;
using UsaloYa.API.Security;
using static Azure.Core.HttpHeader;

namespace UsaloYa.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(AccessValidationFilter))]
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
        public async Task<IActionResult> GetSalesReport([FromQuery] DateTime fromDate, DateTime toDate, int companyId, int userId = 0)
        {
            try
            {
                if (fromDate.Date == toDate.Date)
                {
                    toDate = fromDate.AddDays(1);
                }

                var result = await _dBContext.Sales
                            .Include(s => s.User)
                            .Include(c => c.Customer)
                            .Where(s => s.CompanyId == companyId
                                    && (s.User.UserId == userId || userId == 0)
                                    && s.SaleDate >= fromDate.Date && s.SaleDate <= toDate.Date
                            ).Select(r => new { 
                                 SaleID = r.SaleId,
                                 SaleDate = r.SaleDate,
                                 UserId = r.UserId,
                                 UserName = r.User.UserName,
                                 FullName = r.User.FirstName + ' ' + r.User.LastName,
                                 CustomerName = r.Customer == null ? "" : r.Customer.FirstName + ' ' + r.Customer.LastName1,
                                 Notes = r.Notes,
                                 Payment = r.PaymentMethod,
                                 Status = r.Status,
                                 TotalSale = r.TotalSale
                             }).ToListAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetSalesReport.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpGet("GetSaleDetails")]
        public async Task<IActionResult> GetSaleDetails([FromQuery] int saleId, int companyId)
        {
            try
            {
                var result = await _dBContext.SaleDetails
                                .Include(d => d.Product)
                                .Include(d => d.Sale)
                                .Include(d => d.Sale.User)
                            .Where(s => s.SaleId == saleId && s.Sale.CompanyId == companyId)
                            .Select(r => new
                            {
                                Barcode = r.Product.Barcode,
                                ProductName = r.Product.Name,
                                Quantity = r.Quantity,
                                BuyPrice = r.Product.BuyPrice,
                                SoldPrice = r.UnitPrice,
                                ProductPrice1 = r.Product.UnitPrice1,
                                ProductPrice2 = r.Product.UnitPrice2,
                                ProductPrice3 = r.Product.UnitPrice3,
                                TotalPrice = r.TotalPrice,
                                SaleID = r.SaleId,
                                SaleDate = r.Sale.SaleDate,
                                TotalSale = r.Sale.TotalSale,
                                UserId = r.Sale.UserId,
                                UserName = r.Sale.User.UserName,
                                FullName = r.Sale.User.FirstName + ' ' + r.Sale.User.LastName,
                                PriceLevel = r.PriceLevel?? 0

                            }).ToListAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetSaleDetails.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }
    }
}
