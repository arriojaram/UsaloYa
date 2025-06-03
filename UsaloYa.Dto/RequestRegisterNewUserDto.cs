using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsaloYa.Dto
{
    public class RequestRegisterNewUserDto
    {
        public string UserName { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Address { get; set; }
       
        

    }
}
