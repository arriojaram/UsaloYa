using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsaloYa.Dto;
using UsaloYa.Dto.Enums;
using UsaloYa.Library.Models;

namespace UsaloYa.Services.interfaces
{
    public interface IUserService
    {
        Task<UserDto> SaveUser(UserDto userDto);
        Task<bool> SetToken(string userName, string token);
        Task<UserResponseDto> GetUser(int userId, bool isLogin);
        Task<IEnumerable<GroupDto>> GetGroups();
        Task<IEnumerable<UserResponseDto>> GetAllUsers(int companyId, string name, Role requestorRole, int requestorId);
        Task<(bool isValid, string message, int userId)> Validate(string deviceId, UserTokenDto request);
        Task ManageNumConnectedUsers(int companyId, int licenseNumUsers);
        Task<int?> Logout(string userName);
        Task<UserResponseDto> RegisterNewUserAndCompany(RegisterUserAndCompanyDto request);
        Task<bool> IsUsernameUnique(string companyName);
        Task<bool> IsEmailUnique(string email);

        Task<(bool isValid, string message, int userId)> RequestVerificationCodeEmail(RequestVerificationCodeDto data, string deviceId);

    }
}
