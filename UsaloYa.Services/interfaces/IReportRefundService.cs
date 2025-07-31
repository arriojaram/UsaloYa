using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsaloYa.Services.interfaces
{
    public interface IReportRefundService
    {
        Task<IEnumerable<object>> GetRefundsReport(DateTime fromDate, DateTime toDate, int companyId, int userId);
        Task<IEnumerable<object>> GetRefundDetails(int saleId, int companyId);
    }
}
