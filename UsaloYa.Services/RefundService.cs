using System.Xml.Linq;
using UsaloYa.Dto;
using UsaloYa.Dto.Enums;
using UsaloYa.Dto.Utils;
using UsaloYa.Library.Models;
using UsaloYa.Services.Interfaces;
using System.Xml.Linq;

namespace UsaloYa.Services
{
    public class RefundService : IRefundService
    {
        private readonly DBContext _dBContext;
        private readonly ICompanyService _companyService;
        private readonly IProductService _productService;
        private readonly ISaleService _saleService;

        public RefundService(DBContext dBContext, ICompanyService companyService, IProductService productService,ISaleService saleService)
        {
            _dBContext = dBContext;
            _companyService = companyService; 
            _productService = productService;
            _saleService = saleService;

        }

        public async Task<bool> ManageRefund(RequestRefundDto requestRefundDto, int companyId)
        {
            var canRefund = await this.CanSaleBeRefund(requestRefundDto.SaleDate, companyId);

            if (!canRefund)
                return false;

            var added = await this.AddRefund(requestRefundDto);

            if (!added)
                return false;

            var productsToAdd = requestRefundDto.ProductRefundList
                .Select(item => new SetStockDto
                {
                    ProductId = item.ProductId,
                    UnitsInStock = item.Quantity,
                    IsHardReset = false,
                })
                .ToList();

            await _saleService.UpdateSaleStatus(requestRefundDto.SaleId, SaleStatus.Reembolsado);

            await _productService.AddUnitsInStockByProductId(productsToAdd, companyId);

            return true;
        }


        public async Task<bool> CanSaleBeRefund(DateTime? SaleDate, int companyId)
        {
            if (SaleDate is null) return false;

            var settingsXml = await _companyService.GetSettings(companyId);  
            var settingsList = Utils.DeserializeSettings(settingsXml);  

            int? maxDaysToRefund = settingsList
                .Where(s => s.Key == "maxDaysToRefund")
                .Select(s => int.TryParse(s.Value, out var val) ? val : (int?)null)
                .FirstOrDefault();


            var days = Utils.DifferenceOfDays(SaleDate, maxDaysToRefund);

            var response = days > 0 ?  true :  false;
            return response;
        }
  

        public async Task<bool> AddRefund(RequestRefundDto requestRefundDto)
        {
            List<Refund> refundsToAdd = new();
            if (requestRefundDto?.ProductRefundList == null)
                return false;
            
            foreach (var refundItem in requestRefundDto.ProductRefundList)
            {    
                var refund = new Refund
                {
                    SaleId = requestRefundDto.SaleId,
                    UserId = requestRefundDto.UserId,
                    RefundDate = Utils.GetMxDateTime(),
                    RefundMethod = requestRefundDto.RefundMethod,
                    ProductId = refundItem.ProductId,
                    Barcode = refundItem.Barcode,
                    Reason = refundItem.Reason,
                    Measure = refundItem.Measure,
                    Quantity = refundItem.Quantity,
                    UnitPriceRefund = refundItem.UnitPriceRefund,
                    RefundAmount = refundItem.RefundAmount
                };
                refundsToAdd.Add(refund);
            }

            if (!refundsToAdd.Any())
                return false;

            await _dBContext.Refunds.AddRangeAsync(refundsToAdd);
            await _dBContext.SaveChangesAsync();


            return true;
        }
    }
}
