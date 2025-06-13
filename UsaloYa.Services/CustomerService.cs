using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsaloYa.Dto;
using UsaloYa.Dto.Utils;
using UsaloYa.Library.Models;
using UsaloYa.Services.interfaces;

namespace UsaloYa.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly DBContext _dBContext;
       

        public CustomerService(DBContext dBContext)
        {
            _dBContext = dBContext;
           
        }

        public async Task<IEnumerable<CustomerDto>> GetAllCustomers(int companyId, string nameOrPhoneOrEmail)
        {
            var customers = (string.IsNullOrEmpty(nameOrPhoneOrEmail) || string.Equals(nameOrPhoneOrEmail, "-1", StringComparison.OrdinalIgnoreCase))
                ? await _dBContext.Customers.Where(c => (c.CompanyId == companyId || companyId == 0))
                                            .OrderByDescending(u => u.CustomerId)
                                            .Take(50)
                                            .ToListAsync()
                : await _dBContext.Customers
                    .Where(u => (
                        (u.FirstName.Contains(nameOrPhoneOrEmail) || u.LastName1.Contains(nameOrPhoneOrEmail) || (u.LastName2 ?? "$1").Contains(nameOrPhoneOrEmail)) ||
                        (u.Email != null && (u.Email.Contains(nameOrPhoneOrEmail) || nameOrPhoneOrEmail.Contains(u.Email ?? "$1"))) ||
                        (u.CellPhoneNumber != null && (u.CellPhoneNumber.Contains(nameOrPhoneOrEmail) || nameOrPhoneOrEmail.Contains(u.CellPhoneNumber ?? "$1"))) ||
                        (u.WorkPhoneNumber != null && (u.WorkPhoneNumber.Contains(nameOrPhoneOrEmail) || nameOrPhoneOrEmail.Contains(u.WorkPhoneNumber ?? "$1")))
                        ) && u.CompanyId == companyId)
                    .OrderBy(u => u.FirstName)
                    .ThenBy(u => u.LastName1)
                    .Take(50)
                    .ToListAsync();

            return customers.Select(u => new CustomerDto
            {
                LastName1 = u.LastName1,
                CompanyId = u.CompanyId,
                CellPhoneNumber = u.CellPhoneNumber,
                WorkPhoneNumber = u.WorkPhoneNumber,
                Address = u.Address,
                CustomerId = u.CustomerId,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName2 = u.LastName2 ?? "",
                Notes = u.Notes
            });
        }

        public async Task<CustomerDto> GetCustomerById(int customerId)
        {
            var customer = await _dBContext.Customers.FirstOrDefaultAsync(u => u.CustomerId == customerId);
            return  new CustomerDto
            {
                CustomerId = customer.CustomerId,
                Address = customer.Address,
                CellPhoneNumber = customer.CellPhoneNumber,
                CompanyId = customer.CompanyId,
                Email = customer.Email,
                FirstName = customer.FirstName,
                LastName1 = customer.LastName1,
                LastName2 = customer.LastName2,
                Notes = customer.Notes,
                WorkPhoneNumber = customer.WorkPhoneNumber
            };
        }

        public async Task<CustomerDto> SaveCustomer(CustomerDto customerDto)
        {
            Customer customer;

            if (customerDto.CustomerId == 0)
            {
                var exists = await _dBContext.Customers.AnyAsync(c =>
                    (c.CellPhoneNumber ?? "-0") == (customerDto.CellPhoneNumber ?? "-1") ||
                    (c.WorkPhoneNumber ?? "-0") == (customerDto.CellPhoneNumber ?? "-1") ||
                    (c.Email ?? "-0") == (customerDto.Email ?? "-1")
                    && c.CompanyId == customerDto.CompanyId);

                if (exists) throw new InvalidOperationException("$_Email_O_Telefono_Existente");

                customer = new Customer
                {
                    Address = Utils.EmptyToNull(customerDto.Address),
                    CellPhoneNumber = customerDto.CellPhoneNumber,
                    WorkPhoneNumber = Utils.EmptyToNull(customerDto.WorkPhoneNumber),
                    Email = Utils.EmptyToNull(customerDto.Email),
                    FirstName = customerDto.FirstName,
                    CompanyId = customerDto.CompanyId,
                    LastName1 = customerDto.LastName1,
                    LastName2 = Utils.EmptyToNull(customerDto.LastName2),
                    Notes = Utils.EmptyToNull(customerDto.Notes)
                };

                _dBContext.Customers.Add(customer);
            }
            else
            {
                customer = await _dBContext.Customers.FindAsync(customerDto.CustomerId);
                if (customer == null) throw new KeyNotFoundException("Customer not found");

                customer.FirstName = customerDto.FirstName;
                customer.LastName1 = customerDto.LastName1;
                customer.LastName2 = Utils.EmptyToNull(customerDto.LastName2);
                customer.Address = Utils.EmptyToNull(customerDto.Address);
                customer.CellPhoneNumber = customerDto.CellPhoneNumber;
                customer.WorkPhoneNumber = Utils.EmptyToNull(customerDto.WorkPhoneNumber);
                customer.Email = Utils.EmptyToNull(customerDto.Email);
                customer.Notes = Utils.EmptyToNull(customerDto.Notes);

                _dBContext.Entry(customer).State = EntityState.Modified;
            }

            await _dBContext.SaveChangesAsync();
            return customerDto;
        }
    }
}
