using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsaloYa.Dto
{
    public class SendVerificationCodeDto
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string? CodeVerification { get; set; }
    }
}
