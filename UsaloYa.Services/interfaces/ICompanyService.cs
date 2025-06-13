using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsaloYa.Dto;
using UsaloYa.Dto.Enums;
using UsaloYa.Library.Models;

namespace UsaloYa.Services.interfaces
{
    public interface ICompanyService
    {
        Task<IEnumerable<GenericObjectDto>> GetAll4List(string name);
        Task<CompanyDto> GetCompanyById(int companyId);
        Task<CompanyDto> SaveCompany(CompanyDto companyDto);
        Task<bool> UpdateSettings(int companyId, string settingsXml);
        Task<string> GetSettings(int companyId);
        Task<bool> UpdateCompanyStatus(int companyId, int statusId);
        Task<bool> UpdateCompanyLicense(int companyId, int planId);
        Task<bool> CheckExpiration(int companyId);
        Task<int?> AddRent(RentDto rentDto);
        Task<List<RentDto>> GetPaymentHistory(int companyId);
        Task<DateTime> CalculateExpirationDate(Company company, decimal rentAmount, RentTypeId typeId);
        Task<bool> IsCompanyUnique(string companyName);


    }
}
