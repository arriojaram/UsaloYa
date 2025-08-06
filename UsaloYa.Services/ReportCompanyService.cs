using Microsoft.EntityFrameworkCore;
using System.Linq;
using UsaloYa.Dto;
using UsaloYa.Library.Models;
using UsaloYa.Services.interfaces;

namespace UsaloYa.Services
{
    public class ReportCompanyService : IReportCompanyService
    {
        private readonly DBContext _dBContext;
        public ReportCompanyService(DBContext dBContext)
        {
            _dBContext = dBContext;
        }

        public async Task<IEnumerable<CompanyReportDto>> GetCompaniesReport(int InactiveDays, int status, string company)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-InactiveDays);

            return await _dBContext.Companies
                .Include(c => c.Sales)
                .Include(c => c.Products)
                .Include(c => c.Users)
                .Where(c =>
                    (status == 0 ? c.StatusId == 0 : c.StatusId != 0) &&
                    (company == "-1" || c.Name.Contains(company)) &&
                    ((c.Users.Any() ? c.Users.Max(u => u.LastAccess) : c.CreationDate) <= cutoffDate)
                )
                .OrderByDescending(c => c.Users.Any() ? c.Users.Max(u => u.LastAccess) : c.CreationDate)
                .Select(r => new CompanyReportDto
                {
                    UsersNumer = r.Users.Count(),
                    CompanyName = r.Name,
                    LastAcces = r.Users.Any() ? r.Users.Max(u => u.LastAccess) : r.CreationDate,
                    Phone = r.PhoneNumber,
                    ProductsNumber = r.Products.Count(),
                    SalesNumber = r.Sales.Count(),
                })
                .ToListAsync();
        }


    }
}
