using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using UsaloYa.Services.interfaces;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
namespace UsaloYa.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration settings)
        {
            _configuration = settings;
        }

        public async Task SendEmailFromTemplateAsync(string toEmail, string subject, string templatePath, Dictionary<string, string> variables)
        {
            if (!File.Exists(templatePath))
                throw new FileNotFoundException("Plantilla no encontrada", templatePath);

            var html = await File.ReadAllTextAsync(templatePath);

            // Reemplazar {{Variable}}3
             foreach (var kv in variables)
            {
                html = html.Replace($"{{{{{kv.Key}}}}}", kv.Value);
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_configuration.GetValue<string>("EmailSettings:SenderName"), _configuration.GetValue<string>("EmailSettings:SenderEmail")));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            message.Body = new TextPart("html") { Text = html };

            try
            {
                using var client = new MailKit.Net.Smtp.SmtpClient();

                // Conexión segura con STARTTLS (puerto 587)
                var smtpServer = _configuration.GetValue<string>("EmailSettings:SmtpServer");
                var smtpPort = _configuration.GetValue<int>("EmailSettings:SmtpPort");
                var senderEmail = _configuration.GetValue<string>("EmailSettings:SenderEmail");
                var senderPassword = _configuration.GetValue<string>("EmailSettings:Password");
                client.RequireTLS = true;
                await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(senderEmail, senderPassword);

                await client.SendAsync(message);

                // Muy importante cerrar correctamente
                await client.DisconnectAsync(true);
            }
            catch (SmtpCommandException smtpEx)
            {
                // Puedes registrar detalles específicos del fallo SMTP
                throw new InvalidOperationException("SMTP command error: {StatusCode}" + smtpEx.StatusCode);

            }
            catch (SmtpProtocolException protocolEx)
            {
                // Fallo del protocolo (como el que reportaste)
                throw new InvalidOperationException("SMTP protocol error: {Message}" + protocolEx.Message);
                
            }
            catch (Exception ex)
            {
                // Otro tipo de error inesperado
                throw new InvalidOperationException("Error general al enviar correo." + ex);


            }
        }
    }
}
