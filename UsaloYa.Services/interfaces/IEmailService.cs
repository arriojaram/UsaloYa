using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsaloYa.Services.interfaces
{
    public interface IEmailService
    {
        Task SendEmailFromTemplateAsync(string toEmail, string subject, string templatePath, Dictionary<string, string> variables);
    }


}
