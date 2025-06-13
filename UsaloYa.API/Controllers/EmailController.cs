using Microsoft.AspNetCore.Mvc;
using UsaloYa.Services.interfaces;
using UsaloYa.Dto;

namespace UsaloYa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
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
            { "Mensaje", $"Tu código de verificación es: {request.CodeVerification}" }
        };

            await _emailService.SendEmailFromTemplateAsync(
                request.Email,
                "Verificación de correo electrónico.",
                templatePath,
                variables
            );

            return Ok("Correo enviado.");
        }

    }
}