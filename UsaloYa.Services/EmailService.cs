using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using UsaloYa.Dto;
using UsaloYa.Dto.Utils;
using UsaloYa.Library.Models;
using UsaloYa.Services.interfaces;
using static Org.BouncyCastle.Math.EC.ECCurve;
namespace UsaloYa.Services
{

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IQuestionnaireService _questionnaireService;

        public EmailService(IConfiguration configuration, IQuestionnaireService questionnaireService)
        {
            _configuration = configuration;
            _questionnaireService = questionnaireService;
        }

        public async Task<bool> SendEmailNewUsers(SendVerificationCodeDto request, string templatePath)
        {

            if (!File.Exists(templatePath))
                throw new FileNotFoundException("Plantilla no encontrada", templatePath);
            var html = await File.ReadAllTextAsync(templatePath);

            var variables = new Dictionary<string, string>
        {
            { "Name", _configuration.GetSection("NotificationTemplates:NewUsers:Name").Value + request.FirstName },
            { "Message", _configuration.GetSection("NotificationTemplates:NewUsers:Message").Value +
                         $"<strong>{request.CodeVerification}</strong>" },
            { "Verification", _configuration.GetSection("NotificationTemplates:NewUsers:Verification").Value +
                                  _configuration.GetSection("NotificationTemplates:NewUsers:Message2").Value +
                                  _configuration.GetSection("NotificationTemplates:NewUsers:LinkVerification").Value
            }

        };


            foreach (var kv in variables)
            {
                html = html.Replace($"{{{{{kv.Key}}}}}", kv.Value);
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_configuration["EmailSettings:SenderName"], _configuration["EmailSettings:SenderEmail"]));
            
            message.To.Add(MailboxAddress.Parse(request.Email));

            message.Subject = _configuration.GetSection("NotificationTemplates:NewUsers:Title").Value;
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
                return true;
                

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al enviar el correo: " + ex.Message, ex);
            }
        }

        public async Task<bool> SendEmailToAdmins( string username, string company, int idUserRegister, string templatePath)
        {
            if (!File.Exists(templatePath))
                throw new FileNotFoundException("Plantilla no encontrada", templatePath);
            List<string> adminEmails = _configuration.GetSection("EmailSettings:OnRegisterNotificationList").Get<List<string>>();
            var responsequestionnaire = await _questionnaireService.GetQuestionnaireByUser(idUserRegister) as List<QuestionDto>;
            var questionnaireHtml = Utils.GenerateHtmlQuestions(responsequestionnaire);
            var html = await File.ReadAllTextAsync(templatePath);   
            var variables = new Dictionary<string, string>
            {
                { "Name", _configuration.GetSection("NotificationTemplates:NewRegister:Name").Value },
                { "Message", _configuration.GetSection("NotificationTemplates:NewRegister:Message").Value + $"<strong>{username}</strong>"+"<br/>"+
                             _configuration.GetSection("NotificationTemplates:NewRegister:Message2").Value + $"<strong>{company}</strong>" +"<br/><br/>"+
                             _configuration.GetSection("NotificationTemplates:NewRegister:Message3").Value + "<br/>"+
                                                    questionnaireHtml
            },
                { "Verification", "" }
            };

            foreach (var kv in variables)
            {
                html = html.Replace($"{{{{{kv.Key}}}}}", kv.Value);
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_configuration["EmailSettings:SenderName"], _configuration["EmailSettings:SenderEmail"]));
            
            foreach (var email in adminEmails)
            {
                message.To.Add(MailboxAddress.Parse(email));
            }

            message.Subject = _configuration.GetSection("NotificationTemplates:NewRegister:Title").Value;
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
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error al enviar el correo: " + ex.Message, ex);
            }
        }
    }
}
