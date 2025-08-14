using Microsoft.EntityFrameworkCore;
using System.Linq;
using UsaloYa.Dto;
using UsaloYa.Dto.Enums;
using UsaloYa.Library.Models;
using UsaloYa.Services.Interfaces;

namespace UsaloYa.Services
{
    public class ReportCompanyService : IReportCompanyService
    {
        private readonly DBContext _dBContext;
        public ReportCompanyService(DBContext dBContext)
        {
            _dBContext = dBContext;
        }

        public async Task<IEnumerable<CompanyReportDto>> GetCompaniesReport(int InactiveDays, CompanyStatus status, string company)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-InactiveDays);

            return await _dBContext.Companies
                .Include(c => c.Sales)
                .Include(c => c.Products)
                .Include(c => c.Users)
                .Where(c =>
                    (status == CompanyStatus.Inactive ? c.StatusId.Equals(CompanyStatus.Inactive) : !c.StatusId.Equals(CompanyStatus.Inactive)) &&
                    (company == "-1" || c.Name.Contains(company)) &&
                    ((c.Users.Any() ? c.Users.Max(u => u.LastAccess) : c.CreationDate) <= cutoffDate)
                )
                .OrderByDescending(c => c.Users.Any() ? c.Users.Max(u => u.LastAccess) : c.CreationDate)
                .Select(r => new CompanyReportDto
                {
                    NumberOfUsers = r.Users.Count(),
                    CompanyName = r.Name,
                    LastAccess = r.Users.Any() ? r.Users.Max(u => u.LastAccess) : r.CreationDate,
                    Phone = r.PhoneNumber,
                    NumberOfProducts = r.Products.Count(),
                    NumberOfSales = r.Sales.Count(),
                })
                .ToListAsync();
        }


    }
}
