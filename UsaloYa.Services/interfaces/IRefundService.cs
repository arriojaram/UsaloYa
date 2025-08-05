using UsaloYa.Dto.Enums;
using UsaloYa.Dto;

namespace UsaloYa.Services.Interfaces
{
    public interface IRefundService
    {
        Task<bool> CanSaleBeRefund(DateTime? SaleDate, int companyId);
        Task<bool> ManageRefund(RequestRefundDto requestRefundDto, int companyId);
    }
}
