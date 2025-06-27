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
            var variables = new Dictionary<string, string>
            {
                { "Name", _configuration["NotificationTemplates:NewUsers:Name"] + request.FirstName },
                { "Message", _configuration["NotificationTemplates:NewUsers:Message"] + $"<strong>{request.CodeVerification}</strong>" },
                { "Verification", _configuration["NotificationTemplates:NewUsers:Verification"] +
                                  _configuration["NotificationTemplates:NewUsers:Message2"] +
                                  _configuration["NotificationTemplates:NewUsers:LinkVerification"] }
            };

            var subject = _configuration["NotificationTemplates:NewUsers:Title"];
            var recipients = new List<string> { request.Email };

            return await SendTemplatedEmailAsync(subject, templatePath, variables, recipients);
        }


        public async Task<bool> SendEmailToAdmins(string username, string company, int idUserRegister, string templatePath)
        {
            var adminEmails = _configuration.GetSection("EmailSettings:OnRegisterNotificationList").Get<List<string>>();
            var responsequestionnaire = await _questionnaireService.GetQuestionnaireByUser(idUserRegister) as List<QuestionDto>;
            var questionnaireHtml = Utils.GenerateHtmlQuestions(responsequestionnaire);

            var variables = new Dictionary<string, string>
            {
                { "Name", _configuration["NotificationTemplates:NewRegister:Name"] },
                { "Message", _configuration["NotificationTemplates:NewRegister:Message"] + $"<strong>{username}</strong><br/>" +
                             _configuration["NotificationTemplates:NewRegister:Message2"] + $"<strong>{company}</strong><br/><br/>" +
                             _configuration["NotificationTemplates:NewRegister:Message3"] + "<br/>" + questionnaireHtml },
                { "Verification", "" }
            };

            var subject = _configuration["NotificationTemplates:NewRegister:Title"];

            return await SendTemplatedEmailAsync(subject, templatePath, variables, adminEmails);
        }



        public async Task<bool> SendTemplatedEmailAsync(string subject, string templatePath, Dictionary<string, string> variables, List<string> recipients)
        {
            if (!File.Exists(templatePath))
                throw new FileNotFoundException("Plantilla no encontrada", templatePath);

            var html = await File.ReadAllTextAsync(templatePath);

            foreach (var kv in variables)
            {
                html = html.Replace($"{{{{{kv.Key}}}}}", kv.Value);
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                _configuration["EmailSettings:SenderName"],
                _configuration["EmailSettings:SenderEmail"]
            ));

            foreach (var email in recipients)
            {
                message.To.Add(MailboxAddress.Parse(email));
            }

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

                await client.AuthenticateAsync(
                    _configuration["EmailSettings:SenderEmail"],
                    _configuration["EmailSettings:Password"]
                );

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

