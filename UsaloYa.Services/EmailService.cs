using System.Net;
using System.Net.Mail;
using UsaloYa.Services.interfaces;
using Microsoft.Extensions.Configuration;
namespace UsaloYa.Services
{

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailFromTemplateAsync(string toEmail, string subject, string templatePath, Dictionary<string, string> variables)
        {
            if (!File.Exists(templatePath))
                throw new FileNotFoundException("Plantilla no encontrada", templatePath);

            var html = await File.ReadAllTextAsync(templatePath);

            // Reemplazar {{Variable}}
            foreach (var kv in variables)
            {
                html = html.Replace($"{{{{{kv.Key}}}}}", kv.Value);
            }

            var fromEmail = _configuration["EmailSettings:SenderEmail"];
            var fromName = _configuration["EmailSettings:SenderName"];
            var password = _configuration["EmailSettings:Password"];
            var smtpHost = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);

            var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = html,
                IsBodyHtml = true
            };
            message.To.Add(toEmail);

            using var smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(fromEmail, password),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true
            };

            await smtpClient.SendMailAsync(message);
        }
    }
}
