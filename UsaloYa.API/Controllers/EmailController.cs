using Microsoft.AspNetCore.Mvc;
using UsaloYa.Services.Interfaces;
using UsaloYa.Dto;
using Azure.Identity;
using UsaloYa.Dto.Utils;

namespace UsaloYa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        private readonly IQuestionnaireService _questionnaireService;



        public EmailController(IEmailService emailService, IWebHostEnvironment env, IConfiguration config, IQuestionnaireService questionnaireService)
        {
            _emailService = emailService;
            _env = env;
            _config = config;
            _questionnaireService = questionnaireService;
         
        }

        [HttpPost("SendEmailNewUsers")]
        public async Task<IActionResult> SendEmailNewUsers(SendVerificationCodeDto request)
        {
            var templatePath = Path.Combine(_env.ContentRootPath, "Templates", "Notificacion.html");

            await _emailService.SendEmailNewUsers(request, templatePath);

            return Ok("Correo enviado.");
        }


        [HttpPost("SendEmailToAdmins")]
        public async Task<IActionResult> SendEmailToAdmins(string name, string company, int idUserRegister)
        {

            var templatePath = Path.Combine(_env.ContentRootPath, "Templates", "Notificacion.html");
            await _emailService.SendEmailToAdmins(name, company, idUserRegister, templatePath);

            return Ok("Correo enviado.");
        }

        [HttpPost("SendWelcomeEmail")]
        public async Task<IActionResult> SendWelcomeEmail(string email)
        {

            var templatePath = Path.Combine(_env.ContentRootPath, "Templates", "Notificacion.html");
            await _emailService.SendWelcomeEmail(email, templatePath);

            return Ok("Correo enviado.");
        }

    }
}