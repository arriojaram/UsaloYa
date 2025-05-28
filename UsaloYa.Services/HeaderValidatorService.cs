using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsaloYa.Dto.Enums;
using UsaloYa.Dto;
using UsaloYa.Library.Models;
using Microsoft.EntityFrameworkCore;

namespace UsaloYa.Services
{
    class HeaderValidatorService
    {
        /// <summary>
        /// Esta funcion valida si el Requestor es un usuario existente en la base de datos y si el topRol del requestor es igual o mayor.
        /// En caso de que el topRol no sea mayor, entonce se evalua si el requestor pertenece a la misma compañia
        /// </summary>
        /// <param name="requestor"></param>
        /// <param name="userCompanyId"></param>
        /// <param name="topRol"></param>
        /// <param name="_dBContext"></param>
        /// <returns>Retorna un objeto usuario(Id, UserName y RoleId) si el requestor es mayor al topRol</returns>
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
            if (requestorInfo == null)
            {
                return user;
            }

            if (requestorInfo.RoleId < (int)topRol)
            {
                if (userCompanyId != requestorInfo.CompanyId)
                    return user;

            }
            user.UserId = requestorInfo.UserId;
            user.RoleId = requestorInfo.RoleId;
            user.CompanyId = requestorInfo.CompanyId;
            return user;
        }

        /// <summary>
        /// Esta funcion obtiene la informacion del Usuario(Requestor) y evalua si el usuario es mayor o igual al topRol recibido
        /// </summary>
        /// <param name="requestor"></param>
        /// <param name="topRol"></param>
        /// <param name="_dBContext"></param>
        /// <returns>Retorna un objeto usuario(Id, UserName y RoleId) si el requestor es mayor al topRol</returns>
        public async static Task<UserDto> ValidateRequestor(string requestor, Role topRol, DBContext _dBContext)
        {
            int userId = 0;
            var user = new UserDto() { UserId = -1 };
            if (!int.TryParse(requestor, out userId))
                return user;
            if (userId <= 0)
                return user;

            //Validate user status and rol
            var userDb = await _dBContext.Users.Include(c => c.Company).FirstOrDefaultAsync(u => u.UserId == userId);
            if (userDb == null || user.RoleId < (int)topRol)
                return user;

            user.UserId = userDb.UserId;
            user.UserName = userDb.UserName;
            user.RoleId = userDb.RoleId;
            user.CompanyStatusId = userDb.Company.StatusId;

            return user;
        }

        /// <summary>
        /// Esta funcion obtiene la informacion del Usuario(Requestor) y evalua si el usuario es mayor o igual al topRol recibido y si pertenece a la misma compañia
        /// </summary>
        /// <param name="requestor"></param>
        /// <param name="topRol"></param>
        /// <param name="_dBContext"></param>
        /// <returns>Retorna un objeto usuario(Id, UserName y RoleId) si el requestor es mayor al topRol</returns>
        public async static Task<UserDto> ValidateRequestorSameCompany(string requestor, Role topRol, int companyId, DBContext _dBContext)
        {
            int userId = 0;
            var user = new UserDto() { UserId = -1 };
            if (!int.TryParse(requestor, out userId))
                return user;
            if (userId <= 0)
                return user;

            //Validate user status and rol
            var userDb = await _dBContext.Users.FindAsync(userId);
            if (userDb == null || userDb.RoleId < (int)topRol || userDb.CompanyId != companyId)
            {
                return user;
            }
            user.UserId = userDb.UserId;
            user.UserName = userDb.UserName;
            user.RoleId = userDb.RoleId;

            return user;
        }
    }
}
