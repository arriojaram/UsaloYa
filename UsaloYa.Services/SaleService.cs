using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsaloYa.Dto.Enums;
using UsaloYa.Dto.Utils;
using UsaloYa.Dto;
using UsaloYa.Library.Models;
using UsaloYa.Services.interfaces;

namespace UsaloYa.Services
{
    public class SaleService : ISaleService
    {
        private readonly DBContext _dBContext;

        public SaleService(DBContext dBContext)
        {
            _dBContext = dBContext;
        }

        public async Task<int> AddSale(SaleDto sale)
        {
            var newSale = new Sale
            {
                CompanyId = sale.CompanyId,
                CustomerId = sale.CustomerId == 0 ? null : sale.CustomerId,
                Notes = sale.Notes,
                PaymentMethod = sale.PaymentMethod,
                SaleDate = Utils.GetMxDateTime(),
                Status = "Abierta",
                Tax = sale.Tax,
                UserId = sale.UserId,
                TotalSale = 0
            };

            _dBContext.Sales.Add(newSale);
            await _dBContext.SaveChangesAsync();

            return newSale.SaleId;
        }

        public async Task<bool> AddProductsToSale(int saleId, List<SaleDetailsDto> saleDetails)
        {
            decimal totalSale = 0;

            foreach (var detail in saleDetails)
            {
                totalSale += detail.TotalPrice;

                var product = new SaleDetail
                {
                    SaleId = saleId,
                    ProductId = detail.ProductId,
                    Quantity = detail.Quantity,
                    TotalPrice = detail.TotalPrice,
                    UnitPrice = detail.UnitPrice,
                    PriceLevel = detail.PriceLevel
                };

                _dBContext.SaleDetails.Add(product);
                await UpdateStock(product.ProductId, product.Quantity);
            }

            if (saleDetails.Count > 0)
            {
                await _dBContext.SaveChangesAsync();
                if (totalSale > 0) await UpdateTotalSale(saleId, totalSale);
            }

            return true;
        }

        public async Task<bool> UpdateStock(int productId, int selledItems)
        {
            var existingProduct = await _dBContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            if (existingProduct == null) return false;

            existingProduct.UnitsInStock -= selledItems;
            _dBContext.Entry(existingProduct).State = EntityState.Modified;
            await _dBContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateTotalSale(int saleId, decimal totalSale)
        {
            var sale = await _dBContext.Sales.FindAsync(saleId);
            if (sale == null) return false;

            sale.SaleDate = Utils.GetMxDateTime();
            sale.Status = SaleStatus.Completada.ToString();
            sale.TotalSale = totalSale;
            _dBContext.Entry(sale).State = EntityState.Modified;
            await _dBContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateSaleStatus(int saleId, SaleStatus status)
        {
            var sale = await _dBContext.Sales.FindAsync(saleId);
            if (sale == null) return false;

            sale.Status = status.ToString();
            _dBContext.Entry(sale).State = EntityState.Modified;
            await _dBContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateStockAfterStatusChange(int saleId, int companyId, bool isCancelAction)
        {
            var saleProds = await _dBContext.SaleDetails
                .Include(d => d.Product)
                .Where(s => s.SaleId == saleId && s.Sale.CompanyId == companyId)
                .Select(r => new { r.Product.ProductId, r.Quantity })
                .ToListAsync();

            foreach (var item in saleProds)
            {
                var prod = await _dBContext.Products.FindAsync(item.ProductId);
                if (prod == null) continue;

                prod.UnitsInStock += isCancelAction ? item.Quantity : -item.Quantity;
                _dBContext.Entry(prod).State = EntityState.Modified;
            }

            await _dBContext.SaveChangesAsync();
            return true;
        }
    }
}
