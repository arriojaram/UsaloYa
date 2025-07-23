using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsaloYa.Dto;

namespace UsaloYa.Services.interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailNewUsers(SendVerificationCodeDto request, string templatePath);
        Task<bool> SendEmailToAdmins(string username, string company, int idUserRegister, string templatePath);
        Task<bool> SendWelcomeEmail(string email, string templatePath);
    }


}
