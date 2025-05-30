using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsaloYa.Library.Models;
using UsaloYa.Services.interfaces;

namespace UsaloYa.Services
{
    public class ReportService : IReportService
    {
        private readonly DBContext _dBContext;

        public ReportService(DBContext dBContext)
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
                    SaleDate = r.SaleDate,
                    UserId = r.UserId,
                    UserName = r.User.UserName,
                    FullName = r.User.FirstName + ' ' + r.User.LastName,
                    CustomerName = r.Customer == null ? "" : r.Customer.FirstName + ' ' + r.Customer.LastName1,
                    Notes = r.Notes,
                    Payment = r.PaymentMethod,
                    Status = r.Status,
                    TotalSale = r.TotalSale
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetSaleDetails(int saleId, int companyId)
        {
            return await _dBContext.SaleDetails
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
                    PriceLevel = r.PriceLevel ?? 0,
                    r.Sale.Status
                })
                .ToListAsync();
        }
    }
}
