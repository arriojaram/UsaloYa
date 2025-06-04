using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsaloYa.Dto.Utils;
using UsaloYa.Dto;
using UsaloYa.Library.Models;
using UsaloYa.Library.Config;
using UsaloYa.Services.interfaces;
using UsaloYa.Dto.Enums;
using System.Runtime;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using Azure.Core;


namespace UsaloYa.Services
{
    public class UserService : IUserService
    {
        private readonly DBContext _dBContext;
        private readonly AppConfig _settings;
        private readonly ICompanyService _CompanyService;
        private readonly IConfiguration _configuration;

        public UserService(DBContext dBContext, AppConfig settings, ICompanyService companyService, IConfiguration configuration)
        {
            _dBContext = dBContext;
            _settings = settings;
            _CompanyService = companyService;
            _configuration = configuration;
        }

        public async Task<UserDto> SaveUser(UserDto userDto)
        {
            var encryptedPassword = Utils.EncryptPassword(userDto.Token);
            User userToSave;

            if (userDto.UserId == 0)
            {
                var existsUser = await _dBContext.Users.AnyAsync(u => u.UserName == userDto.UserName);
                if (existsUser) throw new InvalidOperationException("$_Nombre_De_Usuario_Duplicado");

                userToSave = new User
                {
                    UserName = userDto.UserName.Trim(),
                    Token = encryptedPassword,
                    FirstName = userDto.FirstName.Trim(),
                    LastName = userDto.LastName.Trim(),
                    CompanyId = userDto.CompanyId,
                    GroupId = userDto.GroupId,
                    LastUpdateBy = userDto.LastUpdatedBy,
                    CreatedBy = userDto.CreatedBy,
                    LastAccess = null,
                    IsEnabled = true,
                    StatusId = (int)UserStatus.Desconocido,
                    CreationDate = Utils.GetMxDateTime(),
                    RoleId = userDto.RoleId
                }; 

                if (userDto.LastUpdatedBy == 0 || userDto.CreatedBy == 0)
                {
                    userToSave.CreatedBy =  _configuration.GetValue<int>("SelfRegister:CreatedBy");
                    userToSave.LastUpdateBy =  _configuration.GetValue<int>("SelfRegister:LastUpdateBy");
                }
                    _dBContext.Users.Add(userToSave);
            }
            else
            {
                userToSave = await _dBContext.Users.FindAsync(userDto.UserId);
                if (userToSave == null) throw new KeyNotFoundException();

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
            return userDto;
        }

        public async Task<bool> SetToken(string userName, string token)
        {
            var user = await _dBContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null) return false;

            user.Token = Utils.EncryptPassword(token);
            await _dBContext.SaveChangesAsync();
            return true;
        }

        public async Task<UserResponseDto> GetUser(int userId, bool isLogin)
        {
            User? user = isLogin
                ? await _dBContext.Users
                    .Include(c => c.CreatedByNavigation)
                    .Include(c => c.LastUpdateByNavigation)
                    .Include(c => c.Company)
                    .FirstOrDefaultAsync(u => u.UserId == userId)
                : await _dBContext.Users
                    .Include(c => c.Company)
                    .FirstOrDefaultAsync(u => u.UserId == userId);

            return new UserResponseDto
            {
                UserId = user.UserId,
                IsEnabled = user.IsEnabled ?? false,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CompanyId = user.CompanyId,
                GroupId = user.GroupId,
                LastAccess = user.LastAccess,
                StatusId = user.StatusId,
                CreationDate = user.CreationDate,
                RoleId = user.RoleId ?? 0,
                CreatedByUserName = user.CreatedByNavigation?.UserName ?? "",
                LastUpdatedByUserName = user.LastUpdateByNavigation?.UserName ?? "",
                CompanyName = user.Company.Name,
                CompanyStatusId = user.Company.StatusId
            };
        }

        public async Task<IEnumerable<GroupDto>> GetGroups()
        {
            var groups = await _dBContext.Groups.ToListAsync();
            return groups.Select(g => new GroupDto
            {
                GroupId = g.GroupId,
                Name = g.Name,
                Description = g.Description
            });
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsers(int companyId, string name, Role requestorRole, int requestorId)
        {
            List<User> users;

            if (string.IsNullOrEmpty(name) || name.Equals("-1", StringComparison.OrdinalIgnoreCase))
            {
                users = await _dBContext.Users
                    .Where(c => c.CompanyId == companyId || companyId == 0)
                    .OrderByDescending(u => u.UserId)
                    .Take(50)
                    .ToListAsync();
            }
            else
            {
                users = await _dBContext.Users
                    .Include(em => em.Company)
                    .Where(u => (u.FirstName.Contains(name) || u.LastName.Contains(name) || name.Contains(u.Company.Name))
                                && (u.CompanyId == companyId || companyId == 0))
                    .Take(50)
                    .ToListAsync();
            }

            if (requestorRole > Role.Admin)
            {
                var salesmanUsers = await _dBContext.Users
                    .Include(c => c.Company)
                    .Where(u => u.CreatedBy == requestorId)
                    .ToListAsync();

                users.AddRange(salesmanUsers);
            }

            return users.Select(u => new UserResponseDto
            {
                UserId = u.UserId,
                IsEnabled = u.IsEnabled ?? false,
                UserName = u.UserName,
                FirstName = u.FirstName,
                LastName = u.LastName
            }).OrderBy(u => u.FirstName).ThenBy(u => u.LastName).ToList();

        }


        ////////////////////////////////////
        public async Task<(bool isValid, string message, int userId)> Validate(string deviceId, UserTokenDto request)
        {
            var encryptedPassword = Utils.EncryptPassword(request.Token);
            var user = await _dBContext.Users
                .FirstOrDefaultAsync(u => u.UserName == request.UserName && u.Token == encryptedPassword);

            if (user == null) return (false, "Usuario o contraseña inválidos", 0);

            if (!(user.IsEnabled ?? false)) return (false, "Usuario no válido", 0);

            var userRol = EConverter.GetEnumFromValue<Role>(user.RoleId ?? 0);
            var companyInfo = await GetCompanyStatus(user.CompanyId);
            if (companyInfo == null) return (false, "Hay un error con la información del negocio.", 0);

            if (userRol < Role.SysAdmin && companyInfo.StatusId == (int)CompanyStatus.Expired) return (false, "$_Expired_License", 0);
            if (userRol < Role.SysAdmin && companyInfo.StatusId == (int)CompanyStatus.Inactive) return (false, "Empresa desactivada", 0);

            string msg = "";
            if (!string.IsNullOrEmpty(user.DeviceId) && user.DeviceId != deviceId.Trim())
            {
                msg = "Este usuario está activo en otro dispositivo. La sesión en ese otro dispositivo será terminada.";
            }

            if (userRol == Role.Root || companyInfo.StatusId is (int)CompanyStatus.Active or (int)CompanyStatus.PendingPayment or (int)CompanyStatus.Free)
            {
                user.LastAccess = DateTime.Now;
                user.StatusId = (int)UserStatus.Conectado;
                user.DeviceId = deviceId;
                user.SessionToken = Guid.NewGuid();


                _dBContext.Entry(user).State = EntityState.Modified;
                await _dBContext.SaveChangesAsync();

                await ManageNumConnectedUsers(user.CompanyId, companyInfo.PlanNumUsers ?? 1);
            }
            else
            {
                return (false, "La compañía se encuentra en un estado inactivo, contacta a tu vendedor.", 0);
            }

            return (true, msg, user.UserId);
        }

        public async Task ManageNumConnectedUsers(int companyId, int licenseNumUsers)
        {
            var activeUsers = await _dBContext.Users
                .Where(u => u.CompanyId == companyId)
                .OrderBy(u => u.LastAccess)
                .ToListAsync();

            if (activeUsers.Count > licenseNumUsers)
            {
                var firstLoggedInUser = activeUsers.First();
                firstLoggedInUser.SessionToken = null;
                firstLoggedInUser.DeviceId = null;
                firstLoggedInUser.StatusId = (int)UserStatus.Desconectado;

                _dBContext.Entry(firstLoggedInUser).State = EntityState.Modified;
                await _dBContext.SaveChangesAsync();
            }
        }


        public async Task<CompanyDto> GetCompanyStatus(int companyId)
        {
            CompanyStatus status = CompanyStatus.Inactive;
            int numUsers = 1;
            CompanyDto companyInfo = null;

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
            return companyInfo;
        }
        public async Task<int?> Logout(string userName)
        {
            var user = await _dBContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null) return null;

            user.LastAccess = DateTime.Now;
            user.StatusId = (int)UserStatus.Desconectado;
            user.DeviceId = null;
            user.SessionToken = null;

            _dBContext.Entry(user).State = EntityState.Modified;
            await _dBContext.SaveChangesAsync();

            return user.UserId;
        }

        public async Task<bool> RegisterNewUserAndCompany(RegisterUserAndCompanyDto request)
        {
            var company = await _CompanyService.SaveCompany(request.CompanyDto);
            if (company.CompanyId == 0)
                throw new InvalidOperationException("$_Compania no creada");

            UserDto userDto = new UserDto
            {
                UserName = request.RequestRegisterNewUserDto.UserName.Trim(),
                FirstName = request.RequestRegisterNewUserDto.FirstName.Trim(),
                LastName = request.RequestRegisterNewUserDto.LastName.Trim(),
                Token = request.RequestRegisterNewUserDto.Token,
                CompanyId = company.CompanyId,
                GroupId = 1,
                CodeVerification = Utils.GenerateCode(),
                CreatedBy = 0,
                LastUpdatedBy = 0, // Assuming 0 is the system user
                RoleId = (int)Role.Admin,
                IsEnabled = true,

            };
            var user = await this.SaveUser(userDto);
            return true;

        }

        public async Task<bool> IsUsernameUnique(string userName)
        {
            var existsUser = await _dBContext.Users.AnyAsync(u => u.UserName == userName);
            if (existsUser)
            return false;

            return true;
        }


        public async Task<User> VerificationCodeEmail(RequestVerificationCodeDto data)
        {
            var user = await _dBContext.Users
                .FirstOrDefaultAsync(u => u.CodeVerification == data.Code && u.Email == data.Email);
            if (user == null) throw new InvalidOperationException("$_Datos incorrectos");

            return user;
        }

        public async Task<bool> RequestVerificationCodeEmail(RequestVerificationCodeDto data, string deviceId)
        {
            var user = await VerificationCodeEmail(data);
            if(user != null)
            {
                UserTokenDto userRequest = new UserTokenDto
                {
                    UserName = user.UserName,
                    Token = user.Token,
                    
                };
                await Validate(deviceId, userRequest);
            }
            return false;

        }
    }
}
