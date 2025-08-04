using Microsoft.EntityFrameworkCore;
using UsaloYa.Dto;
using UsaloYa.Library.Models;
using UsaloYa.Services.interfaces;

namespace UsaloYa.Services
{
    public class ReportRefundService : IReportRefundService
    {
        private readonly DBContext _dBContext;

        public ReportRefundService(DBContext dBContext)
        {
            _dBContext = dBContext;
        }

        public async Task<IEnumerable<RefundReportDto>> GetRefundsReport(DateTime fromDate, DateTime toDate, int companyId)
        {
            var toDateInclusive = toDate.Date.AddDays(1);

            return await _dBContext.Refunds
                .Where(r => r.Sale.CompanyId == companyId &&
                            r.RefundDate >= fromDate.Date &&
                            r.RefundDate < toDateInclusive)
                .GroupBy(r => new
                {
                    r.SaleId,
                    r.UserId,
                    r.Sale.Folio,
                    r.User.UserName,                    
                    r.RefundDate.Date,
                    r.RefundMethod
                })
                .Select(g => new RefundReportDto
                {
                    SaleId = g.Key.SaleId,
                    Folio = g.Key.Folio,
                    UserId = g.Key.UserId,
                    Name = g.Key.UserName,
                    RefundDate = g.Key.Date,
                    RefundMethod = g.Key.RefundMethod,
                    RefundAmountTotal = g.Sum(x => x.RefundAmount)
                })
                .ToListAsync();
        }


        public async Task<RefundReportDto> GetRefundDetails(int saleId, int companyId)
        {
            var refundReport = await _dBContext.Refunds
                .Include(r => r.Sale)
                .Include(r => r.User)
                .Include(r => r.Product)
                .Where(r => r.SaleId == saleId && r.Sale.CompanyId == companyId)
                .GroupBy(r => new
                {
                    r.SaleId,
                    r.UserId,
                    r.User.UserName,
                    r.RefundDate,
                    r.RefundMethod
                })
                .Select(g => new RefundReportDto
                {
                    SaleId = g.Key.SaleId,
                    UserId = g.Key.UserId,
                    Name = g.Key.UserName,
                    RefundDate = g.Key.RefundDate,
                    RefundMethod = g.Key.RefundMethod,
                    RefundAmountTotal = g.Sum(x => x.RefundAmount),

                    Products = g.Select(r => new RefundProductDto
                    {
                        Barcode = r.Barcode,
                        ProductName = r.Product.Name,
                        Reason = r.Reason,
                        Measure = r.Measure,
                        Quantity = r.Quantity,
                        UnitPriceRefund = r.UnitPriceRefund,
                        RefundAmount = r.RefundAmount
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return refundReport;
        }
    }
}
