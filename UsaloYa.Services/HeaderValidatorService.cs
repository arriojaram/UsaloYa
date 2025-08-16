using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsaloYa.Dto.Enums;
using UsaloYa.Dto;
using UsaloYa.Library.Models;
using Microsoft.EntityFrameworkCore;
using UsaloYa.Services.Interfaces;

namespace UsaloYa.Services
{
    public class HeaderValidatorService
    {
        private readonly IUserService _userService;
        public HeaderValidatorService(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Esta funcion valida si el Requestor es un usuario existente en la base de datos y si el topRol del requestor es igual o mayor.
        /// En caso de que el topRol no sea mayor, entonce se evalua si el requestor pertenece a la misma compañia
        /// </summary>
        /// <param name="requestor"></param>
        /// <param name="userCompanyId"></param>
        /// <param name="topRol"></param>
        /// <param name="_dBContext"></param>
        /// <returns>Retorna un objeto usuario(Id, UserName y RoleId) si el requestor es mayor al topRol</returns>
        public async Task<UserDto> ValidateRequestorSameCompanyOrTopRol(string requestor, int userCompanyId, Role topRol)
        {
            int userId = 0;
            var user = new UserDto() { UserId = -1 };

            if (!int.TryParse(requestor, out userId))
                return user;
            if (userId <= 0)
                return user;

            //Validate user status and rol
            var requestorInfo = await _userService.GetUser(userId, false);
            if (requestorInfo.UserId == 0)
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
        public async Task<UserDto> ValidateRequestor(string requestor, Role topRol)
        {
            int userId = 0;
            var user = new UserDto() { UserId = -1 };
            if (!int.TryParse(requestor, out userId))
                return user;
            if (userId <= 0)
                return user;

            //Validate user status and rol
            var userDb = await _userService.GetUser(userId, false);
            if (userDb.UserId == 0 || user.RoleId < (int)topRol)
                return user;

            user.UserId = userDb.UserId;
            user.UserName = userDb.UserName;
            user.RoleId = userDb.RoleId;
            user.CompanyStatusId = userDb.CompanyStatusId;

            return user;
        }

        /// <summary>
        /// Esta funcion obtiene la informacion del Usuario(Requestor) y evalua si el usuario es mayor o igual al topRol recibido y si pertenece a la misma compañia
        /// </summary>
        /// <param name="requestor"></param>
        /// <param name="topRol"></param>
        /// <param name="_dBContext"></param>
        /// <returns>Retorna un objeto usuario(Id, UserName y RoleId) si el requestor es mayor al topRol</returns>
        public async Task<UserDto> ValidateRequestorSameCompany(string requestor, Role topRol, int companyId)
        {
            int userId = 0;
            var user = new UserDto() { UserId = -1 };
            if (!int.TryParse(requestor, out userId))
                return user;
            if (userId <= 0)
                return user;

            //Validate user status and rol
            var userDb = await _userService.GetUser(userId, false);

            if (userDb.UserId == 0 || userDb.RoleId < (int)topRol || userDb.CompanyId != companyId)
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
