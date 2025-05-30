using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsaloYa.API.Config;
using UsaloYa.API.Security;
using UsaloYa.Dto.Enums;
using UsaloYa.Dto;
using UsaloYa.Library.Models;
using UsaloYa.Services.interfaces;
using UsaloYa.Services;

namespace UsaloYa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly DBContext _dBContext;
        private readonly AppConfig _settings;

        public UserController(DBContext dBContext, IUserService userService, ILogger<UserController> logger, AppConfig settings)
        {
            _logger = logger;
            _userService = userService;
            _dBContext = dBContext;
            _settings = settings;
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

        /////////////////////
        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser([FromHeader] string RequestorId, int userId, string i = "")
        {
            try
            {
                bool isLogin = i.Equals("login");
                var user = await _userService.GetUser(userId, isLogin);

                if (user == null) return NotFound();

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
                return StatusCode(500, new { message = "No se puede procesar la solicitud, error de servidor." });
            }
        }

        //////////////////////////
        [NonAction]
        public async Task<CompanyDto> GetCompanyStatus(int companyId)
        {
            CompanyStatus status = CompanyStatus.Inactive;
            int numUsers = 1;
            CompanyDto companyInfo = null;
            try
            {
                var company = await _dBContext
                    .Companies
                    .Include(c => c.Plan)
                    .FirstOrDefaultAsync(c => c.CompanyId == companyId);

                if (company != null)
                {
                    status = EConverter.GetEnumFromValue<CompanyStatus>(company.StatusId);
                    var expirationDate = company.ExpirationDate ?? Utils.GetMxDateTime();
                    numUsers = company.Plan.NumUsers;
                    if (status != CompanyStatus.Inactive && Utils.GetMxDateTime().Date > expirationDate.Date)
                    {
                        
                        if (expirationDate.AddDays(_settings.MaxPendingPaymentDaysAllowAccess) >= Utils.GetMxDateTime())
                        {
                            status = CompanyStatus.PendingPayment;
                        }
                        else 
                        {
                            status = CompanyStatus.Free; // Compañia marcada con acceso free
                            company.PlanId = 1;
                            numUsers = 1;
                        }

                        company.StatusId = (int)status;
                        _dBContext.Entry(company).State = EntityState.Modified;
                        await _dBContext.SaveChangesAsync();

                    }
                    companyInfo = new CompanyDto();
                    companyInfo.CompanyId = companyId;
                    companyInfo.StatusId = company.StatusId;
                    companyInfo.PlanNumUsers = numUsers;
                    
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "IsCompanyExpired.ApiFunctionError");
            }
            return companyInfo;
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

    }
}
