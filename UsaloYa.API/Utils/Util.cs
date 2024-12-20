using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using UsaloYa.API.Enums;
using UsaloYa.API.Models;

namespace UsaloYa.API.Utils
{
    public class Util
    {
        public async static Task<int> ValidateRequestorSameCompanyOrTopRol(string requestor, int userCompanyId, Role topRol, DBContext _dBContext)
        {
            int userId = 0;
            if (!int.TryParse(requestor, out userId))
                return -1;
            if (userId <= 0)
                return -1;

            //Validate user status and rol
            var requestorInfo = await _dBContext.Users.FindAsync(userId);
            if (requestorInfo == null || requestorInfo.RoleId < (int)topRol)
            {
                if(userCompanyId != requestorInfo.CompanyId) 
                    return -1;
            }
            return userId;
        }

        public async static Task<int> ValidateRequestor(string requestor, Role topRol, DBContext _dBContext)
        {
            int userId = 0;
            if (!int.TryParse(requestor, out userId))
                return -1;
            if (userId <= 0)
                return -1;

            //Validate user status and rol
            var user = await _dBContext.Users.FindAsync(userId);
            if (user == null || user.RoleId < (int)topRol)
                return -1;

            return userId;
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
