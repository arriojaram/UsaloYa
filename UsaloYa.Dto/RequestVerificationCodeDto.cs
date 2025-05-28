using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsaloYa.Dto
{
    public class RequestVerificationCodeDto
    {
        public string Code { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
