using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsaloYa.Dto;

namespace UsaloYa.Services.interfaces
{
    public interface IReportCompanyService
    {
        Task<IEnumerable<CompanyReportDto>> GetCompaniesReport(int InactiveDays, int status, string company);
    }
}
