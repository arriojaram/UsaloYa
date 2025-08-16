using UsaloYa.Dto;

namespace UsaloYa.Services.Interfaces
{
    public interface IReportRefundService
    {
        Task<IEnumerable<RefundReportDto>> GetRefundsReport(DateTime fromDate, DateTime toDate, int companyId);
        Task<RefundReportDto> GetRefundDetails(int saleId, int companyId);
    }
}
