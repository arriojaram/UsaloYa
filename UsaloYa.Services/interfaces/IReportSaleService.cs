using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsaloYa.Dto;

namespace UsaloYa.Services.interfaces
{
    public interface IReportSaleService
    {
        Task<IEnumerable<object>> GetSalesReport(DateTime fromDate, DateTime toDate, int companyId, int userId);
        Task<IEnumerable<object>> GetSaleDetails(int saleId, int companyId);
    }
}
