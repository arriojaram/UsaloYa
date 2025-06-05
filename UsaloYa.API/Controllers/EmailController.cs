using Microsoft.AspNetCore.Mvc;
using UsaloYa.Services.interfaces;
using UsaloYa.Dto;

namespace UsaloYa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _env;

        public EmailController(IEmailService emailService, IWebHostEnvironment env)
        {
            _emailService = emailService;
            _env = env;
        }


        [HttpPost("EnviarCorreo")]
        public async Task<IActionResult> Enviar([FromBody] SendVerificationCodeDto request)
        {
            var templatePath = Path.Combine(_env.ContentRootPath, "Templates", "Notificacion.html");

            var variables = new Dictionary<string, string>
        {
            { "Nombre", request.FirstName },
            { "Mensaje", "Tu correo ha"+ request.CodeVerification+"  enviado correctamente usando Gmail SMTP y MailKit." }
        };

            await _emailService.SendEmailFromTemplateAsync(
                toEmail: request.Email,
                subject: "Verificación de correo electrónico.",
                templatePath: templatePath,
                variables: variables
            );

            return Ok("Correo enviado.");
        }
    }

}
