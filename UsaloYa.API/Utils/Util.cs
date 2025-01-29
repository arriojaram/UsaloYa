using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using UsaloYa.API.DTO;
using UsaloYa.API.Enums;
using UsaloYa.API.Migrations;
using UsaloYa.API.Models;

namespace UsaloYa.API.Utils
{
    public class Util
    {
        public async static Task<UserDto> ValidateRequestorSameCompanyOrTopRol(string requestor, int userCompanyId, Role topRol, DBContext _dBContext)
        {
            int userId = 0;
            var user = new UserDto() { UserId = -1 };

            if (!int.TryParse(requestor, out userId))
                return user;
            if (userId <= 0)
                return user;

            //Validate user status and rol
            var requestorInfo = await _dBContext.Users.FindAsync(userId);
            if (requestorInfo == null || requestorInfo.RoleId < (int)topRol)
            {
                if (userCompanyId != requestorInfo.CompanyId)
                    return user;

            }
            user.UserId = requestorInfo.UserId;
            user.RoleId = requestorInfo.RoleId;
            user.CompanyId = requestorInfo.CompanyId;
            return user;
        }

        public async static Task<UserDto> ValidateRequestor(string requestor, Role topRol, DBContext _dBContext)
        {
            int userId = 0;
            var user = new UserDto() { UserId = -1 };
            if (!int.TryParse(requestor, out userId))
                return user;
            if (userId <= 0)
                return user;

            //Validate user status and rol
            var userDb = await _dBContext.Users.FindAsync(userId);
            if (userDb == null || user.RoleId < (int)topRol)
                return user;

            user.UserId = userDb.UserId;
            user.UserName = userDb.UserName;
            user.RoleId = userDb.RoleId;

            return user;
        }

        public static string EncryptPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static DateTime GetMxDateTime()
        {
            // Obtener la zona horaria de México
            TimeZoneInfo mexicoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)");

            // Obtener la fecha y hora actual en UTC
            DateTime utcNow = DateTime.UtcNow;

            // Convertir la fecha y hora actual en UTC a la hora local de México
            DateTime mexicoTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, mexicoTimeZone);

            return mexicoTime;
        }
    }
}
