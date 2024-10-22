using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsaloYa.API.DTO;
using UsaloYa.API.Models;
using UsaloYa.API.Utils;

namespace UsaloYa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
       
        private readonly ILogger<UserController> _logger;
        private readonly DBContext _dBContext;

        public UserController(DBContext dBContext,  ILogger<UserController> logger)
        {
            _logger = logger;
            _dBContext = dBContext;
        }

        [HttpGet("HelloWorld")]
        public async Task<ActionResult> HelloWorld()
        {
            return Ok("Api is up and running!");
        }

        [HttpPost("SaveUser")]
        public async Task<ActionResult> SaveUser([FromBody] UserDto userDto)
        {
            User userToSave = null;
            try
            {
                if (userDto.UserId == 0)
                {
                    var existsUser = await _dBContext.Users.AnyAsync(u => u.UserName == userDto.UserName);
                    if (existsUser)
                        return Conflict(new { message = "$_NombreDeUsuarioDuplicado" });

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

                    _dBContext.Users.Update(userToSave);
                    
                }

                await _dBContext.SaveChangesAsync();
                return Ok(userToSave);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveUser.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "No se puede procesar la solicitud, error en el servidor." });
            }
        }

        [HttpPost("SetToken")]
        public async Task<IActionResult> SetToken([FromBody] UserTokenDto token)
        {
            try
            {
                var user = await _dBContext.Users.FirstOrDefaultAsync(u => u.UserName == token.UserName);
                if (user == null)
                    return NotFound();

                user.Token = Utils.Util.EncryptPassword(token.Token);
                _dBContext.Users.Update(user);

                await _dBContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SetToken.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "$_ExcepcionOcurrida" });
            }
        }

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser(int userId)
        {
            var u = await _dBContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (u == null) 
                return NotFound();

            var userResponseDto = new UserResponseDto()
            {
                UserId = u.UserId,
                IsEnabled = u.IsEnabled?? false,
                UserName = u.UserName,
                FirstName = u.FirstName,
                LastName = u.LastName,
                CompanyId = u.CompanyId,
                GroupId = u.GroupId,
                LastAccess = u.LastAccess,
                StatusId = u.StatusId,  
                CreationDate = u.CreationDate
            };

            var createdByUserName = await _dBContext.Users
                .Where(user => user.UserId == u.CreatedBy)
                    .Select(user => user.UserName)
                    .FirstOrDefaultAsync();
            
            var updatedByUserName = await _dBContext.Users
                   .Where(user => user.UserId == u.LastUpdateBy)
                   .Select(user => user.UserName)
                   .FirstOrDefaultAsync();

            userResponseDto.CreatedByUserName = createdByUserName?? "";
            userResponseDto.LastUpdatedByUserName = updatedByUserName ?? "";

            userResponseDto.CompanyName = await LoadCompany(userResponseDto.CompanyId);

            return Ok(userResponseDto);
        }

        //TODO: Move this controller to a System Controller
        private async Task<string> LoadCompany(int companyId)
        {
            var company = await _dBContext.Companies.FirstAsync(c => c.CompanyId == companyId);
            if (company != null)
                return company.Name;
            return "No-Company";

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
                return StatusCode(500, new { message = "$_ExcepcionOcurrida" });
            }
        }

        //TODO: Move this controller to a System Controller
        [HttpGet("GetCompanies")]
        public async Task<IActionResult> GetCompanies()
        {
            try
            {
                var companies = await _dBContext.Companies.ToListAsync();
                var companyDtos = companies.Select(u => new CompanyDto
                {
                   CompanyId = u.CompanyId,
                   Name = u.Name,
                   Address = u.Address
                });

                return Ok(companyDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCompanies.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "$_ExcepcionOcurrida" });
            }
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] int companyId, string name = "-1")
        {
            try
            {
                var users = (string.IsNullOrEmpty(name) || string.Equals(name, "-1", StringComparison.OrdinalIgnoreCase))
                    ? await _dBContext.Users.Where(c => (c.CompanyId == companyId || companyId == 0)).OrderByDescending(u => u.UserId).Take(50).ToListAsync()
                    : await _dBContext.Users
                        .Where(u => (
                                   u.FirstName.Contains(name) || u.LastName.Contains(name)
                                || name.Contains(u.FirstName) || name.Contains(u.LastName)
                                ) 
                                && (u.CompanyId == companyId || companyId == 0))
                        .OrderBy(u => u.FirstName)
                        .ThenBy(u => u.LastName)
                        .Take(50)
                        .ToListAsync();

                var userDtos = users.Select(u => new UserResponseDto
                {
                    UserId = u.UserId,
                    IsEnabled = (bool)u.IsEnabled,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    CompanyId = u.CompanyId,
                    GroupId = u.GroupId,
                    LastAccess = u.LastAccess,
                    StatusId = u.StatusId
                });

                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAll.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "$_ExcepcionOcurrida" });
            }
        }

        [HttpPost("Validate")]
        public async Task<IActionResult> Validate([FromBody] UserTokenDto token)
        {
            try
            {
                
                var encryptedPassword = Utils.Util.EncryptPassword(token.Token);
                var user = await _dBContext.Users
                    .FirstOrDefaultAsync(u => u.UserName == token.UserName 
                        && u.Token == encryptedPassword);

                if (user == null)
                {
                    return Unauthorized("Usuario o contraseña inválidos");
                }
                else
                {
                    if (user.IsEnabled?? false)
                    {
                        user.LastAccess = DateTime.Now;
                        user.StatusId = (int)Enumerations.UserStatus.Conectado;
                        _dBContext.Users.Update(user);
                        await _dBContext.SaveChangesAsync();
                    }
                    else
                        return Unauthorized("Usuario no valido");
                }

                return Ok(user.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Validate.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "No se puede procesar la solicitud, error de servidor." });
            }
        }

        [HttpPost("LogOut")]
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
                await _dBContext.SaveChangesAsync();
            }

            return Ok(user.UserId);
        }

    }
}
