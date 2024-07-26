using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using UsaloYa.API.DTO;
using UsaloYa.API.Models;

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
            int userId = 0;
            try
            {
                if (userDto.UserId == 0)
                {
                    var existingUser = await _dBContext.Users.AnyAsync(u => u.UserName == userDto.UserName);
                    if (existingUser)
                        return Conflict(new { message = "$_NombreDeUsuarioDuplicado" });

                    var user = new User
                    {
                        UserName = userDto.UserName.Trim(),
                        Token = Guid.NewGuid().ToString(),
                        FirstName = userDto.FirstName.Trim(),
                        LastName = userDto.LastName.Trim(),
                        CompanyId = userDto.CompanyId,
                        GroupId = userDto.GroupId,
                        LastAccess = userDto.LastAccess,
                        IsEnabled = true
                    };
                    _dBContext.Users.Add(user);
                    userId = user.UserId;
                }
                else
                {
                    var user = await _dBContext.Users.FindAsync(userDto.UserId);
                    if (user == null) 
                        return NotFound();

                    user.IsEnabled = userDto.IsEnabled;
                    user.FirstName = userDto.FirstName.Trim();
                    user.LastName = userDto.LastName.Trim();
                    user.CompanyId = userDto.CompanyId;
                    user.GroupId = userDto.GroupId;
                    user.LastAccess = userDto.LastAccess;

                    _dBContext.Users.Update(user);
                    userId = user.UserId;
                }

                await _dBContext.SaveChangesAsync();
                return Ok(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveUser.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "$_ExcepcionOcurrida" });
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
                IsEnabled = (bool)u.IsEnabled,
                UserName = u.UserName,
                FirstName = u.FirstName,
                LastName = u.LastName,
                CompanyId = u.CompanyId,
                GroupId = u.GroupId,
                LastAccess = u.LastAccess,
                StatusId = u.StatusId
            };
            return Ok(userResponseDto);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] string name = "-1")
        {
            var users = (string.IsNullOrEmpty(name) || string.Equals(name, "-1", StringComparison.OrdinalIgnoreCase))
                ? await _dBContext.Users.OrderByDescending(u => u.UserId).Take(20).ToListAsync()
                : await _dBContext.Users
                    .Where(u => u.FirstName.Contains(name) || u.LastName.Contains(name)
                            || name.Contains(u.FirstName) || name.Contains(u.LastName) )
                    .OrderBy(u => u.FirstName)
                    .ThenBy(u => u.LastName)
                    .Take(20)
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
                LastAccess = u.LastAccess
            });

            return Ok(userDtos);
        }

        [HttpPost("Validate")]
        public async Task<IActionResult> Validate([FromBody] UserTokenDto token)
        {
            var encryptedPassword = Utils.Util.EncryptPassword(token.Token);
            var user = await _dBContext.Users
                .FirstOrDefaultAsync(u => u.UserName == token.UserName && u.Token == encryptedPassword);

            if (user == null) 
                return Unauthorized("Usuario o contraseña invalidos");

            return Ok(user.UserId);
        }
    }
}
