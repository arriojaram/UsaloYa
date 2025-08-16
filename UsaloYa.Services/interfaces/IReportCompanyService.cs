using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsaloYa.Dto;
using UsaloYa.Dto.Enums;

namespace UsaloYa.Services.Interfaces
{
    public interface IReportCompanyService
    {
        Task<IEnumerable<CompanyReportDto>> GetCompaniesReport(int InactiveDays, CompanyStatus status, string company);
    }
}
