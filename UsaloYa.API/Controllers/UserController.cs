using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsaloYa.API.Config;
using UsaloYa.API.DTO;
using UsaloYa.API.Enums;
using UsaloYa.API.Migrations;
using UsaloYa.API.Models;
using UsaloYa.API.Security;
using UsaloYa.API.Utils;

namespace UsaloYa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {

        private readonly ILogger<UserController> _logger;
        private readonly DBContext _dBContext;
        private readonly AppConfig _settings;

        public UserController(DBContext dBContext, ILogger<UserController> logger, AppConfig settings)
        {
            _logger = logger;
            _dBContext = dBContext;
            _settings = settings;
        }

        [HttpGet("HelloWorld")]
        public async Task<ActionResult> HelloWorld()
        {
            return Ok("Api is up and running!");
        }

        [HttpPost("SaveUser")]
        public async Task<ActionResult> SaveUser([FromHeader] string RequestorId, [FromBody] UserDto userDto)
        {
            User userToSave = null;
            var roleId = Enums.EConverter.GetEnumFromValue<Enums.Role>(userDto.RoleId ?? 0);
            if (roleId == default)
                return BadRequest("$_Rol_Invalido");

            if (roleId == Enums.Role.Root)
                return BadRequest("$_Rol_No_Asignable");

            try
            {
                var user = await Util.ValidateRequestor(RequestorId, Role.Admin, _dBContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                if (userDto.UserId == 0)
                {
                    var existsUser = await _dBContext.Users.AnyAsync(u => u.UserName == userDto.UserName);
                    if (existsUser)
                        return Conflict(new { message = "$_Nombre_De_Usuario_Duplicado" });


                    //TODO: Agregar validación de número de usuarios permitidos por la licencia

                    userToSave = new User
                    {
                        UserName = userDto.UserName.Trim(),
                        Token = Guid.NewGuid().ToString(),
                        FirstName = userDto.FirstName.Trim(),
                        LastName = userDto.LastName.Trim(),
                        CompanyId = userDto.CompanyId,
                        GroupId = userDto.GroupId,
                        LastUpdateBy = userDto.LastUpdatedBy,
                        CreatedBy = userDto.CreatedBy,
                        LastAccess = null,
                        IsEnabled = true,
                        StatusId = (int)Enumerations.UserStatus.Desconocido,
                        CreationDate = Util.GetMxDateTime()

                        , RoleId = userDto.RoleId
                    };
                    _dBContext.Users.Add(userToSave);
                }
                else
                {
                    userToSave = await _dBContext.Users.FindAsync(userDto.UserId);
                    if (userToSave == null)
                        return NotFound();

                    userToSave.IsEnabled = userDto.IsEnabled;
                    userToSave.FirstName = userDto.FirstName.Trim();
                    userToSave.LastName = userDto.LastName.Trim();
                    userToSave.CompanyId = userDto.CompanyId;
                    userToSave.GroupId = userDto.GroupId;
                    userToSave.LastAccess = userDto.LastAccess;
                    userToSave.LastUpdateBy = userDto.LastUpdatedBy;
                    userToSave.RoleId = userDto.RoleId;

                    _dBContext.Entry(userToSave).State = EntityState.Modified;

                }

                await _dBContext.SaveChangesAsync();
                return Ok(new UserDto() {
                    CompanyId = userToSave.CompanyId,
                    CreatedBy = userToSave.CreatedBy ?? 0,
                    UserId = userToSave.UserId,
                    CreationDate = userToSave.CreationDate,
                    FirstName = userToSave.FirstName,
                    GroupId = userToSave.GroupId,
                    IsEnabled = userToSave.IsEnabled ?? false,
                    LastAccess = userToSave.LastAccess,
                    LastName = userToSave.LastName,
                    LastUpdatedBy = userToSave.LastUpdateBy ?? 0,
                    RoleId = userToSave.RoleId,
                    UserName = userToSave.UserName,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveUser.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "No se puede procesar la solicitud, error en el servidor." });
            }
        }

        [HttpPost("SetToken")]
        public async Task<IActionResult> SetToken([FromHeader] string RequestorId, [FromBody] UserTokenDto token)
        {
            try
            {
                var user = await _dBContext.Users.FirstOrDefaultAsync(u => u.UserName == token.UserName);
                if (user == null)
                    return NotFound();


                var requestor = await Util.ValidateRequestorSameCompanyOrTopRol(RequestorId, user.CompanyId, Role.Ventas, _dBContext);
                if (requestor.UserId <= 0)
                {
                    return Unauthorized(AppConfig.NO_AUTORIZADO);
                }

                user.Token = Utils.Util.EncryptPassword(token.Token);

                await _dBContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SetToken.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpGet("GetUser")]
        [ServiceFilter(typeof(AccessValidationFilter))]
        public async Task<IActionResult> GetUser([FromHeader] string RequestorId, int userId, string i = "") //parameter i (invoked) is used only on the login component
        {
            User? u;
            try
            {
                if (!i.Equals("login"))
                {
                    u = await _dBContext.Users
                   .Include(c => c.Company)
                   .FirstOrDefaultAsync(u => u.UserId == userId);
                }
                else
                {
                    u = await _dBContext.Users
                    .Include(c => c.CreatedByNavigation)
                    .Include(c => c.LastUpdateByNavigation)
                    .Include(c => c.Company)
                    .FirstOrDefaultAsync(u => u.UserId == userId);
                }

                if (u == null)
                    return NotFound();

                if (!i.Equals("login"))
                {
                    var requestor = await Util.ValidateRequestorSameCompanyOrTopRol(RequestorId, u.CompanyId, Role.Ventas, _dBContext);
                    if (requestor.UserId <= 0)
                    {
                        return Unauthorized(AppConfig.NO_AUTORIZADO);
                    }
                   
                }

                var userResponseDto = new UserResponseDto()
                {
                    UserId = u.UserId,
                    IsEnabled = u.IsEnabled ?? false,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    CompanyId = u.CompanyId,
                    GroupId = u.GroupId,
                    LastAccess = u.LastAccess,
                    StatusId = u.StatusId,
                    CreationDate = u.CreationDate,
                    RoleId = u.RoleId ?? 0,
                    CreatedByUserName = u.CreatedByNavigation == null ? "" : u.CreatedByNavigation.UserName,
                    LastUpdatedByUserName = u.LastUpdateByNavigation == null ? "" : u.LastUpdateByNavigation.UserName,
                    CompanyName = u.Company.Name,
                    CompanyStatusId = u.Company.StatusId
                };

                return Ok(userResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUser.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        //TODO: Move this controller to a System Controller
        [HttpGet("GetGroups")]
        public async Task<IActionResult> GetGroups()
        {
            try
            {
                var g = await _dBContext.Groups.ToListAsync();
                var groupDtos = g.Select(u => new GroupDto
                {
                    GroupId = u.GroupId,
                    Name = u.Name,
                    Description = u.Description
                });
                return Ok(groupDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetGroups.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromHeader] string RequestorId, [FromQuery] int companyId, string name = "-1")
        {
            try
            {
                var requestor = await Util.ValidateRequestorSameCompanyOrTopRol(RequestorId, companyId, Role.User, _dBContext);

                if (requestor.UserId <= 0)
                {
                    return Unauthorized(AppConfig.NO_AUTORIZADO);
                }

                if (companyId == 0)
                {
                    if (requestor.RoleId < (int)Role.SysAdmin)
                        return Unauthorized(AppConfig.NO_AUTORIZADO);
                }
                else if (requestor.RoleId == (int)Role.User)
                {
                    var userQuery = await _dBContext.Users
                     .Include(em => em.Company)
                     .Where(u => u.CompanyId == companyId && u.UserId == requestor.UserId)
                     .ToListAsync();
                    var userDtos1 = userQuery.Select(u => new UserResponseDto
                    {
                        UserId = u.UserId,
                        IsEnabled = u.IsEnabled ?? false,
                        UserName = u.UserName,
                        FirstName = u.FirstName,
                        LastName = u.LastName,

                    });
                    return Ok(userDtos1);
                }

                var companyQuery = (string.IsNullOrEmpty(name) || string.Equals(name, "-1", StringComparison.OrdinalIgnoreCase))
                    ? 
                    await _dBContext.Users.Where(c => (c.CompanyId == companyId || companyId == 0)).OrderByDescending(u => u.UserId).Take(50).ToListAsync()
                    : 
                    await _dBContext.Users
                        .Include(em => em.Company)
                        .Where(u => (
                                   u.FirstName.Contains(name) || u.LastName.Contains(name)
                                || name.Contains(u.FirstName) || name.Contains(u.LastName)
                                || u.Company.Name.Contains(name) 
                                ) 
                                && (u.CompanyId == companyId || companyId == 0)
                        )
                        .Take(50)
                        .ToListAsync();

                var salesmanQuery = await _dBContext.Users.Include(c => c.Company)
                    .Where(u => (requestor.RoleId > (int)Role.Admin && u.CreatedBy == requestor.UserId))
                    .ToListAsync();

                var salesmanUsers = salesmanQuery.Select(u => new UserResponseDto
                {
                    UserId = u.UserId,
                    IsEnabled = u.IsEnabled ?? false,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,

                });

                var userDtos = companyQuery.Select(u => new UserResponseDto
                {
                    UserId = u.UserId,
                    IsEnabled = u.IsEnabled?? false,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                   
                });

                var result = userDtos.Union(salesmanUsers)
                    .OrderBy(u => u.FirstName)
                    .ThenBy(u => u.LastName)
                    .ToList<UserResponseDto>();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAll.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpPost("Validate")]
        public async Task<IActionResult> Validate([FromHeader] string DeviceId, [FromBody] UserTokenDto request)
        {
            var msg = string.Empty;
            try
            {
                var encryptedPassword = Util.EncryptPassword(request.Token);
                var user = await _dBContext.Users
                    .FirstOrDefaultAsync(u => u.UserName == request.UserName
                        && u.Token == encryptedPassword);

                if (user == null)
                    return Unauthorized("Usuario o contraseña inválidos");

                var isUserEnabled = user.IsEnabled ?? false;
                if (!isUserEnabled)
                    return Unauthorized("Usuario no valido");


                var userRol = EConverter.GetEnumFromValue<Role>(user.RoleId ?? 0);
                // Check company expiration
                var companyInfo = await GetCompanyStatus(user.CompanyId);
                if(companyInfo == null)
                {
                    return Unauthorized("Hay un error con la información del negocio.");
                }

                if (userRol < Role.SysAdmin && companyInfo.StatusId == (int)CompanyStatus.Expired)
                    return Unauthorized("$_Expired_License");
                if (userRol < Role.SysAdmin && companyInfo.StatusId == (int)CompanyStatus.Inactive)
                    return Unauthorized("Empresa desactivada");

                if (!string.IsNullOrEmpty(user.DeviceId) && user.DeviceId != DeviceId.Trim())
                {
                    msg = "Este usuario esta activo en otro dispositivo. La sesión en ese otro dispositivo será terminada.";
                }

                if (userRol == Role.Root || 
                    (companyInfo.StatusId == (int)CompanyStatus.Active 
                        || companyInfo.StatusId == (int)CompanyStatus.PendingPayment
                        || companyInfo.StatusId == (int)CompanyStatus.Free
                    )
                )
                {
                    user.LastAccess = DateTime.Now;
                    user.StatusId = (int)Enumerations.UserStatus.Conectado;
                    user.DeviceId = DeviceId;
                    user.SessionToken = Guid.NewGuid();

                    _dBContext.Entry(user).State = EntityState.Modified;

                    await _dBContext.SaveChangesAsync();
                }
                else
                    return Unauthorized("La compañia se encuentra en un estado inactivo, contacta a tu vendedor.");

                await ManageNumConnectedUsers(user.CompanyId, companyInfo.PlanNumUsers?? 1);

                if (!string.IsNullOrEmpty(user.DeviceId) && user.DeviceId != DeviceId)
                {
                    return Ok(new { Id = user.UserId, Msg = "El usuario estaba usando otro dispositivo. La sesión en ese dispositivo será cerrada." });
                }

                return Ok(new { Id = user.UserId, Msg = msg });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Validate.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "No se puede procesar la solicitud, error de servidor." });
            }
        }

        private async Task ManageNumConnectedUsers(int companyId, int licenseNumUsers)
        {
            var companyActiveUsers = await _dBContext.Users
                    .Where(u => u.CompanyId == companyId)
                    .OrderBy(u => u.LastAccess)
                    .ToListAsync();

            if (companyActiveUsers.Count > licenseNumUsers)
            {
                var firstLoggedInUser = companyActiveUsers[0];
                firstLoggedInUser.SessionToken = null;
                firstLoggedInUser.DeviceId = null;
                firstLoggedInUser.StatusId = (int)Enumerations.UserStatus.Desconectado;
                _dBContext.Entry(firstLoggedInUser).State = EntityState.Modified;
                await _dBContext.SaveChangesAsync();
            }
        }

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
                    var expirationDate = company.ExpirationDate ?? Util.GetMxDateTime();
                    numUsers = company.Plan.NumUsers;
                    if (status != CompanyStatus.Inactive && Util.GetMxDateTime().Date > expirationDate.Date)
                    {
                        
                        if (expirationDate.AddDays(_settings.MaxPendingPaymentDaysAllowAccess) >= Util.GetMxDateTime())
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
            var user = await _dBContext.Users
                .FirstOrDefaultAsync(u => u.UserName == token.UserName);

            if (user == null)
            {
                return Unauthorized("Usuario inválido");
            }
            else
            {
                user.LastAccess = DateTime.Now;
                user.StatusId = (int)Enumerations.UserStatus.Desconectado;
                user.DeviceId = null;
                user.SessionToken = null;

                await _dBContext.SaveChangesAsync();
            }

            return Ok(user.UserId);
        }

    }
}
