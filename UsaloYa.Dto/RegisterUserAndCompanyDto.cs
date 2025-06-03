using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsaloYa.Dto
{
    public class RegisterUserAndCompanyDto
    {
        public RequestRegisterNewUserDto RequestRegisterNewUserDto { get; set; }
        public CompanyDto CompanyDto { get; set; }
    }

}
