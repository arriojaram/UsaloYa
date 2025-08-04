using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsaloYa.Dto;

namespace UsaloYa.Services.interfaces
{
    public interface IReportRefundService
    {
        Task<IEnumerable<RefundReportDto>> GetRefundsReport(DateTime fromDate, DateTime toDate, int companyId);
        Task<RefundReportDto> GetRefundDetails(int saleId, int companyId);
    }
}
