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

        public async Task<IEnumerable<CompanyReportDto>> GetCompaniesReport(int inactiveDays, CompanyStatus status, string company)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-inactiveDays);
            company = company?.Trim() ?? "-1";

            // 1. Proyección intermedia: traemos solo lo necesario y calculamos LastAccess de forma segura
            var query = _dBContext.Companies
                .Include(c => c.Sales)
                .Include(c => c.Products)
                .Include(c => c.Users)
                .Select(c => new
                {
                    Company = c,
                    LastAccess = (c.Users.Any()
                        ? c.Users.Max(u => (DateTime?)u.LastAccess)
                        : null
                    ) ?? c.CreationDate
                });

            // 2. Filtro: si company = -1 → filtramos por estado e inactividad, si no → por nombre
            if (company == "-1")
            {
                query = query.Where(x =>
                    (status == CompanyStatus.Inactive
                        ? x.Company.StatusId == (int)CompanyStatus.Inactive
                        : x.Company.StatusId != (int)CompanyStatus.Inactive)
                    &&
                    x.LastAccess <= cutoffDate
                );
            }
            else
            {
                query = query.Where(x => x.Company.Name.Contains(company));
            }

            // 3. Ordenar y mapear a DTO
            var result = await query
                .OrderByDescending(x => x.LastAccess)
                .Select(x => new CompanyReportDto
                {
                    NumberOfUsers = x.Company.Users.Count,
                    CompanyName = x.Company.Name,
                    LastAccess = x.LastAccess,
                    Phone = x.Company.PhoneNumber,
                    NumberOfProducts = x.Company.Products.Count,
                    NumberOfSales = x.Company.Sales.Count,
                    Status = x.Company.StatusId
                })
                .ToListAsync();

            return result;
        }



    }
}
