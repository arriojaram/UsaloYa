using UsaloYa.Dto;
using UsaloYa.Dto.Utils;
using UsaloYa.Library.Models;
using UsaloYa.Services.interfaces;

namespace UsaloYa.Services
{
    public class RefundService : IRefundService
    {
        private readonly DBContext _dBContext;
        private readonly ICompanyService _companyService;

        public RefundService(DBContext dBContext, ICompanyService companyService)
        {
            _dBContext = dBContext;
            _companyService = companyService;        
        }

        public async Task<bool> ManageRefund(RequestRefundDto requestRefundDto, int companyId)
        {
            
            var canRefund = await this.CanSaleBeRefund(requestRefundDto.SaleDate, companyId);

            if (canRefund)
                return await this.AddRefund(requestRefundDto);
            else
                return false;

        }

        public async Task<bool> CanSaleBeRefund(DateTime? SaleDate, int companyId)
        {
            if (SaleDate is null) return false;

            var maxDaysToRefund = await _companyService.GetMaxDaysToRefund(companyId);
            var days = Utils.differenceOfDays(SaleDate, maxDaysToRefund);

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
