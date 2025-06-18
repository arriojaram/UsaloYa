using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsaloYa.Library.Config;
using UsaloYa.API.Security;
using UsaloYa.Dto.Enums;
using UsaloYa.Dto;
using UsaloYa.Library.Models;
using UsaloYa.Services.interfaces;
using UsaloYa.Services;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace UsaloYa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly DBContext _dBContext;
        private readonly AppConfig _settings;
        private readonly IWebHostEnvironment _env;

        public UserController(DBContext dBContext, IUserService userService, ILogger<UserController> logger, AppConfig settings, IEmailService emailService, IWebHostEnvironment env)
        {
            _logger = logger;
            _userService = userService;
            _dBContext = dBContext;
            _settings = settings;
            _emailService = emailService;
            _env = env;
        }

        [HttpGet("HelloWorld")]
        public IActionResult HelloWorld() => Ok("Api is up and running!");

        [HttpPost("SaveUser")]
        public async Task<ActionResult> SaveUser([FromHeader] string RequestorId, [FromBody] UserDto userDto)
        {
            try
            {
                var user = await HeaderValidatorService.ValidateRequestor(RequestorId, Role.Admin, _dBContext);
                if (user.UserId <= 0) return Unauthorized(AppConfig.NO_AUTORIZADO);

                var savedUser = await _userService.SaveUser(userDto);
                return Ok(savedUser);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "SaveUser.ValidationError");
                return Conflict(new { message = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveUser.ApiError");
                return StatusCode(500, new { message = "No se puede procesar la solicitud, error en el servidor." });
            }
        }

        [HttpPost("SetToken")]
        public async Task<IActionResult> SetToken([FromHeader] string RequestorId, [FromBody] UserTokenDto token)
        {
            try
            {
                var user = await _dBContext.Users.FirstOrDefaultAsync(u => u.UserName == token.UserName);
                if (user == null) return NotFound();

                var requestor = await HeaderValidatorService.ValidateRequestorSameCompanyOrTopRol(RequestorId, user.CompanyId, Role.Ventas, _dBContext);
                if (requestor.UserId <= 0) return Unauthorized(AppConfig.NO_AUTORIZADO);

                var updated = await _userService.SetToken(token.UserName, token.Token);
                return updated ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SetToken.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser([FromHeader] string RequestorId, int userId, string i = "")
        {
            try
            {
                bool isLogin = i.Equals("login");
                var user = await _userService.GetUser(userId, isLogin);

                if (user.Equals(default(UserResponseDto))) return NotFound();

                if (!isLogin)
                {
                    var requestor = await HeaderValidatorService.ValidateRequestorSameCompanyOrTopRol(RequestorId, user.CompanyId, Role.Ventas, _dBContext);
                    if (requestor.UserId <= 0) return Unauthorized(AppConfig.NO_AUTORIZADO);
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUser.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpGet("GetGroups")]
        public async Task<IActionResult> GetGroups()
        {
            try
            {
                var groups = await _userService.GetGroups();
                return Ok(groups);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetGroups.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromHeader] string RequestorId, [FromQuery] int companyId, string name = "-1")
        {
            try
            {
                var requestor = await HeaderValidatorService.ValidateRequestorSameCompanyOrTopRol(RequestorId, companyId, Role.User, _dBContext);
                if (requestor.UserId <= 0) return Unauthorized(AppConfig.NO_AUTORIZADO);

                if (companyId == 0 && requestor.RoleId < (int)Role.SysAdmin)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                var users = await _userService.GetAllUsers(companyId, name, (Role)requestor.RoleId, requestor.UserId);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAll.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }



        [HttpPost("Validate")]
        public async Task<IActionResult> Validate([FromHeader] string DeviceId, [FromBody] UserTokenDto request)
        {
            try
            {
                var (isValid, message, userId) = await _userService.Validate(DeviceId, request);

                if (!isValid) return Unauthorized(message);

                return Ok(new { Id = userId, Msg = message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Validate.ApiError");
                return StatusCode(500, new { message = "No se puede procesar la solicitud, error de servidor." + ex });
            }
        }


        [HttpPost("LogOut")]
        [ServiceFilter(typeof(AccessValidationFilter))]
        public async Task<IActionResult> Logout([FromBody] UserTokenDto token)
        {
            try
            {
                var userId = await _userService.Logout(token.UserName);
                return userId.HasValue ? Ok(userId.Value) : Unauthorized("Usuario inválido");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout.ApiError");
                return StatusCode(500, new { message = "No se puede procesar la solicitud, error de servidor." });
            }
        }


        [HttpPost("RegisterNewUser")]
        public async Task<IActionResult> RegisterNewUser([FromBody] RegisterUserQuestionnaireAndCompanyDto request)
        {
            try
            {
                var result = await _userService.RegisterNewUserAndCompany(request);

                if (result.CodeVerification != null)
                {
                    try
                    {

                        string templatePath = Path.Combine(_env.ContentRootPath, "Templates", "Notificacion.html");
                        var variables = new Dictionary<string, string>
                {
                    { "Nombre", result.FirstName },
                    { "Mensaje", $"Hola:<br/><br/>Cuidar tu seguridad y asegurar tu información son prioridades para nuestro equipo. Por eso, necesitamos que confirmes tu correo.<br/><br/>" +
                                 $"Tu código de verificación es <strong>{result.CodeVerification}</strong>, por favor verifique su cuenta en la siguiente página:<br/><a href='www.google.com'>www.google.com</a>" }
                };

                        await _emailService.SendEmailFromTemplateAsync(
                            toEmail: result.Email,
                            subject: "Verificación de correo electrónico.",
                            templatePath: templatePath,
                            variables: variables
                        );
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al enviar correo de verificación para el usuario {Email}", result.Email);

                        return StatusCode(500, new { message = "No se pudo enviar el correo de verificación." });
                    }

                    return Ok(new
                    {
                        message = "Usuario registrado y correo de verificación enviado."
                    });
                }

                return BadRequest(new { message = "No se pudieron registrar los datos." });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Company already exists"))
            {
                _logger.LogWarning("Registro fallido: empresa existente - {Company}", request.CompanyDto.Name);
                return Conflict(new { message = "La empresa ya se encuentra registrada." });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Error de validación al registrar usuario.");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado en RegisterNewUser.");
                return StatusCode(500, new
                {
                    message = "No se puede procesar la solicitud. Error interno del servidor.",
                    detail = ex.Message // 🔐 Solo el mensaje, no el objeto completo
                });
            }
        }



        [HttpPost("IsUsernameUnique")]
        public async Task<IActionResult> IsUsernameUnique([FromBody] string name)
        {
            try
            {
                var result = await _userService.IsUsernameUnique(name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout.ApiError");
                return StatusCode(500, new { message = "No se puede procesar la solicitud, error de servidor." });
            }
        }

        [HttpPost("IsEmailUnique")]
        public async Task<IActionResult> IsEmailUnique([FromBody] string email)
        {
            try
            {
                var result = await _userService.IsEmailUnique(email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout.ApiError");
                return StatusCode(500, new { message = "No se puede procesar la solicitud, error de servidor." });
            }
        }

        [HttpPost("RequestVerificationCodeEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> RequestVerificationCodeEmail([FromHeader] string DeviceId, [FromBody] RequestVerificationCodeDto request)
        {
            _logger.LogInformation("Recibido email: {Email}, code: {Code}, deviceId: {DeviceId}", request.Email, request.Code, DeviceId);

            try
            {
                var (isValid, message, userId) = await _userService.RequestVerificationCodeEmail(request, DeviceId);
                if (isValid == true)
                {
                    return Ok(new { isValid = isValid, userId = userId, message = message });
                }
                return BadRequest("No se logró verificar");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error verificando código: {Message}", ex.Message);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
/*
   [HttpGet("byEmail/{email}")]
        public async Task<ActionResult<UserDto>> GetUserByEmail(string email)
        {
            var user = await _userService.GetByEmailAsync(email);
            if (user == null)
                return NotFound();

            return Ok(user);
        }
*/


    } 