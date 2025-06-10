using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using UsaloYa.Services.interfaces;
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

            foreach (var kv in variables)
            {
                html = html.Replace($"{{{{{kv.Key}}}}}", kv.Value);
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_configuration["EmailSettings:SenderName"], _configuration["EmailSettings:SenderEmail"]));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = html };

            var option = _configuration["EmailSettings:SecureSocketOption"];

            var secureOption = option switch
            {
                "SslOnConnect" => SecureSocketOptions.SslOnConnect,
                "StartTls" => SecureSocketOptions.StartTls,
                "None" => SecureSocketOptions.None,
                _ => SecureSocketOptions.Auto
            };

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(
                _configuration["EmailSettings:SmtpServer"],
                int.Parse(_configuration["EmailSettings:SmtpPort"]),
                secureOption
                );

                await client.AuthenticateAsync(_configuration["EmailSettings:SenderEmail"], _configuration["EmailSettings:Password"]);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al enviar el correo: " + ex.Message, ex);
            }
        }
    }
}
