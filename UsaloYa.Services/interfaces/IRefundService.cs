using UsaloYa.Dto.Enums;
using UsaloYa.Dto;

namespace UsaloYa.Services.interfaces
{
    public interface IRefundService
    {
        Task<bool> CanSaleBeRefund(DateTime? SaleDate, int companyId);
        Task<bool> ManageRefund(RequestRefundDto requestRefundDto, int companyId);
    }
}
