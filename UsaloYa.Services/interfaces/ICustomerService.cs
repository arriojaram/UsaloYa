using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsaloYa.Dto;

namespace UsaloYa.Services.interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDto>> GetAllCustomers(int companyId, string nameOrPhoneOrEmail);
        Task<CustomerDto> GetCustomerById(int customerId);
        Task<CustomerDto> SaveCustomer(CustomerDto customerDto);
    }

}
