using Microsoft.EntityFrameworkCore;
using UsaloYa.Dto;
using UsaloYa.Library.Models;
using UsaloYa.Services.Interfaces;

namespace UsaloYa.Services
{
    public class ReportSaleService : IReportSaleService
    {
        private readonly DBContext _dBContext;

        public ReportSaleService(DBContext dBContext)
        {
            _dBContext = dBContext;
        }

        public async Task<IEnumerable<object>> GetSalesReport(DateTime fromDate, DateTime toDate, int companyId, int userId)
        {
            toDate = toDate.AddDays(1);

            return await _dBContext.Sales
                .Include(s => s.User)
                .Include(c => c.Customer)
                .Where(s => s.CompanyId == companyId
                    && (s.User.UserId == userId || userId == 0)
                    && s.SaleDate >= fromDate.Date && s.SaleDate <= toDate.Date)
                .Select(r => new
                {
                    SaleID = r.SaleId,
                    Folio = r.Folio,
                    SaleDate = r.SaleDate,
                    UserId = r.UserId,
                    UserName = r.User.UserName,
                    FullName = r.User.FirstName + " " + r.User.LastName,
                    CustomerName = r.Customer == null ? "" : r.Customer.FirstName + " " + r.Customer.LastName1,
                    Notes = r.Notes,
                    Payment = r.PaymentMethod,
                    Status = r.Status,
                    TotalSale = r.TotalSale,

                    RefundAmountTotal = _dBContext.Refunds
                        .Where(f => f.SaleId == r.SaleId)
                        .Sum(f => (decimal?)f.RefundAmount) ?? 0m
                })
                .ToListAsync();
        }


        public async Task<IEnumerable<object>> GetSaleDetails(int saleId, int companyId)
        {
            return await _dBContext.SaleDetails
                .Include(d => d.Product)
                .Include(d => d.Sale)
                .Include(d => d.Sale.User)
                .Include(d => d.Sale.Refunds)
                .Where(s => s.SaleId == saleId && s.Sale.CompanyId == companyId)
                .Select(r => new
                {
                    Barcode = r.Product.Barcode,
                    ProductId = r.Product.ProductId,
                    ProductName = r.Product.Name,
                    Quantity = r.Quantity,
                    Measure = r.Product.Measure,
                    CanBeRefunded = r.Product.CanBeRefunded,
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
                    FullName = r.Sale.User.FirstName + " " + r.Sale.User.LastName,
                    PriceLevel = r.PriceLevel ?? 0,
                    r.Sale.Status,

                    RefundProducts = _dBContext.Refunds
                        .Where(f => f.SaleId == r.SaleId && f.ProductId == r.ProductId)
                        .Sum(f => (decimal?)f.Quantity) ?? 0m,

                    RefundAmount = _dBContext.Refunds
                        .Where(f => f.SaleId == r.SaleId && f.ProductId == r.ProductId)
                        .Sum(f => (decimal?)f.RefundAmount) ?? 0m
                })
                .ToListAsync();
        }

    }
}
