using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsaloYa.Dto.Enums;
using UsaloYa.Dto.Utils;
using UsaloYa.Dto;
using UsaloYa.Library.Models;
using UsaloYa.Services.interfaces;
using Microsoft.Extensions.Configuration;

namespace UsaloYa.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly DBContext _dBContext;
        private readonly IConfiguration _configuration;

        public CompanyService(DBContext dBContext, IConfiguration configuration)
        {
            _dBContext = dBContext;
            _configuration = configuration;
        }

        public async Task<IEnumerable<GenericObjectDto>> GetAll4List(string name)
        {
            var companies = (string.IsNullOrEmpty(name) || string.Equals(name, "-1", StringComparison.OrdinalIgnoreCase))
                ? await _dBContext.Companies.OrderBy(u => u.Name).ToListAsync()
                : await _dBContext.Companies.Where(c => c.Name.Contains(name) || name.Contains(c.Name)).OrderBy(u => u.Name).ToListAsync();

            return companies.Select(c => new GenericObjectDto
            {
                Name = c.Name,
                CompanyId = c.CompanyId,
                IsActive = !(c.StatusId == (int)CompanyStatus.Inactive || c.StatusId == (int)CompanyStatus.Expired)
            });
        }

        public async Task<CompanyDto> GetCompanyById(int companyId)
        {
            var company = await _dBContext.Companies.Include(c => c.CreatedByNavigation)
                                                    .Include(c => c.LastUpdateByNavigation)
                                                    .Include(c => c.Plan)
                                                    .FirstOrDefaultAsync(u => u.CompanyId == companyId);

            return company == null ? null : new CompanyDto
            {
                CompanyId = company.CompanyId,
                Name = company.Name,
                Address = company.Address ?? "",
                CreatedBy = company.CreatedBy,
                CreationDate = company.CreationDate,
                LastUpdateBy = company.LastUpdateBy,
                PaymentsJson = company.PaymentsJson ?? "",
                StatusId = company.StatusId,
                ExpirationDate = company.ExpirationDate,
                CreatedByUserName = company.CreatedByNavigation?.UserName,
                CreatedByFullName = $"{company.CreatedByNavigation?.FirstName} {company.CreatedByNavigation?.LastName}",
                LastUpdateByUserName = company.LastUpdateByNavigation?.UserName,
                TelNumber = company.PhoneNumber,
                CelNumber = company.CelphoneNumber,
                Email = company.Email,
                OwnerInfo = company.OwnerInfo,
                PlanId = company.PlanId,
                PlanIdUI = company.Plan?.Name,
                PlanPrice = company.Plan?.Price
            };
        }

        public async Task<CompanyDto> SaveCompany(CompanyDto companyDto)
        {
            Company company;

            if (companyDto.CompanyId == 0)
            {
                if (await _dBContext.Companies.AnyAsync(c => c.Name.ToLower() == companyDto.Name.ToLower()))
                    throw new InvalidOperationException("Company already exists.");

                company = new Company
                {
                    Address = companyDto.Address,
                    LastUpdateBy = companyDto.LastUpdateBy,
                    Name = companyDto.Name,
                    CreatedBy = companyDto.CreatedBy,
                    CreationDate = Utils.GetMxDateTime(),
                    ExpirationDate = Utils.GetMxDateTime().AddDays(30),
                    StatusId = (int)CompanyStatus.Active,
                    PhoneNumber = companyDto.TelNumber,
                    CelphoneNumber = companyDto.CelNumber,
                    Email = companyDto.Email,
                    OwnerInfo = companyDto.OwnerInfo,
                    PlanId = 1
                };
                if (companyDto.LastUpdateBy == 0 || companyDto.CreatedBy == 0)
                {
                    company.CreatedBy = _configuration.GetValue<int>("SelfRegisterDefaults:CreatedBy");
                    company.LastUpdateBy = _configuration.GetValue<int>("SelfRegisterDefaults:LastUpdateBy");
                }

                _dBContext.Companies.Add(company);
            }
            else
            {
                company = await _dBContext.Companies.FindAsync(companyDto.CompanyId);
                if (company == null)
                    throw new KeyNotFoundException("Company not found.");

                company.Address = companyDto.Address;
                company.LastUpdateBy = companyDto.LastUpdateBy;
                company.Name = companyDto.Name;
                company.PhoneNumber = companyDto.TelNumber;
                company.CelphoneNumber = companyDto.CelNumber;
                company.Email = companyDto.Email;
                company.OwnerInfo = companyDto.OwnerInfo;

                if (company.ExpirationDate == null)
                    company.ExpirationDate = Utils.GetMxDateTime();

                _dBContext.Entry(company).State = EntityState.Modified;
            }

            await _dBContext.SaveChangesAsync();
            companyDto.CompanyId = company.CompanyId;
            return companyDto;
        }

        public async Task<bool> UpdateSettings(int companyId, string settingsXml)
        {
            var company = await _dBContext.Companies.FindAsync(companyId);
            if (company == null) return false;

            company.PaymentsJson = settingsXml;
            _dBContext.Entry(company).State = EntityState.Modified;
            await _dBContext.SaveChangesAsync();

            return true;
        }

        public async Task<string> GetSettings(int companyId)
        {
            var company = await _dBContext.Companies.FindAsync(companyId);
            return company?.PaymentsJson ?? "";
        }


        public async Task<bool> UpdateCompanyStatus(int companyId, int statusId)
        {
            var company = await _dBContext.Companies.FindAsync(companyId);
            if (company == null) return false;

            company.StatusId = statusId;
            _dBContext.Entry(company).State = EntityState.Modified;
            await _dBContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateCompanyLicense(int companyId, int planId)
        {
            var company = await _dBContext.Companies.FindAsync(companyId);
            if (company == null) return false;

            company.PlanId = planId;
            company.StatusId = (int)CompanyStatus.Active;
            _dBContext.Entry(company).State = EntityState.Modified;
            await _dBContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CheckExpiration(int companyId)
        {
            var company = await _dBContext.Companies.FindAsync(companyId);
            if (company == null) return false;

            var expirationDate = company.ExpirationDate ?? Utils.GetMxDateTime();
            if (expirationDate.Date > Utils.GetMxDateTime().Date)
            {
                company.StatusId = (int)CompanyStatus.Expired;
                await _dBContext.SaveChangesAsync();
                return false;
            }

            return true;
        }

        public async Task<int?> AddRent(RentDto rentDto)
        {
            var company = await _dBContext.Companies.Include(c => c.Plan)
                                                    .FirstOrDefaultAsync(u => u.CompanyId == rentDto.CompanyId);

            if (company == null) return null;

            var rentType = EConverter.GetEnumFromValue<RentTypeId>(rentDto.StatusId);
            if (rentType == default) return null;

            var expirationDate = await CalculateExpirationDate(company, rentDto.Amount, rentType);

            var rent = new Renta
            {
                CompanyId = rentDto.CompanyId,
                ReferenceDate = Utils.GetMxDateTime(),
                Amount = rentDto.Amount,
                AddedByUserId = rentDto.AddedByUserId,
                StatusId = rentDto.StatusId,
                TipoRentaDesc = rentDto.TipoRentaDesc,
                Notas = rentDto.Notas,
                ExpirationDate = expirationDate
            };

            _dBContext.Rentas.Add(rent);
            await _dBContext.SaveChangesAsync();
            return rent.Id;
        }

        public async Task<DateTime> CalculateExpirationDate(Company company, decimal rentAmount, RentTypeId typeId)
        {
            var expirationDate = company.ExpirationDate ?? Utils.GetMxDateTime();

            if (Utils.GetMxDateTime().Date > expirationDate.Date && expirationDate.AddDays(5) <= Utils.GetMxDateTime())
            {
                expirationDate = Utils.GetMxDateTime();
            }

            switch (typeId)
            {
                case RentTypeId.Mensualidad:
                    var numMonths = rentAmount / company.Plan.Price;
                    expirationDate = expirationDate.AddMonths((int)numMonths);
                    break;
                case RentTypeId.Condonacion:
                case RentTypeId.Extension:
                    int costDay = (int)(company.Plan.Price / 31);
                    int days = (int)(rentAmount / costDay);
                    expirationDate = expirationDate.AddDays(days);
                    break;
                default:
                    break;
            }

            company.StatusId = (int)CompanyStatus.Active;
            company.ExpirationDate = expirationDate;
            _dBContext.Entry(company).State = EntityState.Modified;
            await _dBContext.SaveChangesAsync();

            return expirationDate;
        }

        public async Task<List<RentDto>> GetPaymentHistory(int companyId)
        {
            var rentHistory = await _dBContext.Rentas.Where(c => c.CompanyId == companyId)
                              .Select(r => new RentDto
                              {
                                  AddedByUserId = r.AddedByUserId,
                                  StatusId = r.StatusId,
                                  Amount = r.Amount,
                                  CompanyId = companyId,
                                  Id = r.Id,
                                  ReferenceDate = r.ReferenceDate,
                                  TipoRentaDesc = r.TipoRentaDesc,
                                  ByUserName = r.AddedByUser.UserName,
                                  ExpirationDate = r.ExpirationDate ?? Utils.GetMxDateTime(),
                                  Notas = r.Notas
                              })
                              .OrderByDescending(d => d.ReferenceDate)
                              .ToListAsync();

            rentHistory.ForEach(r => r.StatusIdUI = EConverter.GetEnumNameFromValue<RentTypeId>(r.StatusId));
            return rentHistory;
        }


        public async Task<bool> IsCompanyUnique(string companyName)
        {
            var existCompany = await _dBContext.Companies.AnyAsync(u => u.Name == companyName);
            if (existCompany)
                return false;

            return true;
        }

    }
}
