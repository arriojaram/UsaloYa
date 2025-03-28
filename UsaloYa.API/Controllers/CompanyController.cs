using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Runtime;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UsaloYa.API.Config;
using UsaloYa.API.DTO;
using UsaloYa.API.Enums;
using UsaloYa.API.Models;
using UsaloYa.API.Security;
using UsaloYa.API.Utils;

namespace UsaloYa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(AccessValidationFilter))]
    public class CompanyController : Controller
    {
        private readonly ILogger<CompanyController> _logger;
        private readonly DBContext _dBContext;

        public CompanyController(DBContext dBContext, ILogger<CompanyController> logger)
        {
            _logger = logger;
            _dBContext = dBContext;
        }

        [HttpGet("GetAll4List")]
        public async Task<IActionResult> GetAll4List([FromHeader] string RequestorId, int companyId, string name = "-1")
        {
            try
            {
                var requestor = await Util.ValidateRequestorSameCompanyOrTopRol(RequestorId, companyId, Role.Admin, _dBContext);

                if (requestor.UserId <= 0)
                {
                    return Unauthorized(AppConfig.NO_AUTORIZADO);
                }

                var companies = (string.IsNullOrEmpty(name) || string.Equals(name, "-1", StringComparison.OrdinalIgnoreCase))
                                ? await _dBContext.Companies.OrderBy(u => u.Name)
                                                                        .ToListAsync()
                                : await _dBContext.Companies.Where(c => (c.Name.Contains(name) || name.Contains(c.Name)))
                                                                            .OrderBy(u => u.Name)
                                                                            .ToListAsync();
                if (requestor.RoleId == (int)Role.Admin)
                {
                    companies = companies.Where(u => u.CompanyId == requestor.CompanyId).ToList();
                }
                else if (requestor.RoleId < (int)Role.SysAdmin)
                {
                    var userCompany = companies.FirstOrDefault(c => c.CompanyId == requestor.CompanyId);
                    companies = companies.Where(u => u.CreatedBy == requestor.UserId).ToList();
                    if (!companies.Contains(userCompany))
                    {
                        companies.Add(userCompany);
                    }
                }

                return Ok(companies.Select(c => new GenericObjectDto
                {
                    Name = c.Name,
                    CompanyId = c.CompanyId,
                    IsActive = !(c.StatusId == (int)Enums.CompanyStatus.Inactive || c.StatusId == (int)Enums.CompanyStatus.Expired)
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAll4List.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }


        [HttpGet("GetCompany")]
        public async Task<IActionResult> GetCompany([FromHeader] string RequestorId, int companyId)
        {
            var user = await Util.ValidateRequestor(RequestorId, Role.Admin, _dBContext);
            if (user.UserId <= 0)
                return Unauthorized(AppConfig.NO_AUTORIZADO);

            var c = await _dBContext.Companies
                .Include(u => u.CreatedByNavigation)
                .Include(u => u.LastUpdateByNavigation)
                .Include(c => c.Plan)
                .FirstOrDefaultAsync(u => u.CompanyId == companyId);
            if (c == null)
                return NotFound();

            var companyResponseDto = new CompanyDto()
            {
                CompanyId = c.CompanyId,
                Name = c.Name,
                Address = c.Address ?? "",
                CreatedBy = c.CreatedBy,
                CreationDate = c.CreationDate,
                LastUpdateBy = c.LastUpdateBy,
                PaymentsJson = c.PaymentsJson ?? "",
                StatusId = c.StatusId,
                ExpirationDate = c.ExpirationDate,
                CreatedByUserName = c.CreatedByNavigation?.UserName,
                CreatedByFullName = c.CreatedByNavigation?.FirstName + " " + c.CreatedByNavigation?.LastName,
                LastUpdateByUserName = c.LastUpdateByNavigation?.UserName,

                TelNumber = c.PhoneNumber,
                CelNumber = c.CelphoneNumber,
                Email = c.Email,
                OwnerInfo = c.OwnerInfo,

                PlanId = c.PlanId,
                PlanIdUI = c.Plan?.Name,
                PlanPrice = c.Plan?.Price
            };

            return Ok(companyResponseDto);
        }

        [HttpPost("SaveCompany")]
        public async Task<ActionResult> SaveCompany([FromHeader] string RequestorId, [FromBody] CompanyDto companyDto)
        {
            Company objectToSave = null;

            try
            {
                var user = await Util.ValidateRequestor(RequestorId, Role.Ventas, _dBContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                if (companyDto.CompanyId == 0)
                {
                    var existsObject = await _dBContext.Companies.AnyAsync(
                            c => c.Name.ToLower() == companyDto.Name.ToLower());

                    if (existsObject)
                        return Conflict(new { message = "$_Negocio_Existente" });

                    objectToSave = new Company
                    {
                        Address = companyDto.Address,
                        LastUpdateBy = companyDto.LastUpdateBy,
                        Name = companyDto.Name,
                        CreatedBy = companyDto.CreatedBy,
                        CreationDate = Util.GetMxDateTime(),
                        ExpirationDate = Util.GetMxDateTime().AddDays(30),
                        StatusId = (int)CompanyStatus.Active,
                        PhoneNumber = companyDto.TelNumber,
                        CelphoneNumber = companyDto.CelNumber,
                        Email = companyDto.Email,
                        OwnerInfo = companyDto.OwnerInfo,
                        PlanId = 1 // Hardcode the beta plan for new companies.
                    };
                    _dBContext.Companies.Add(objectToSave);
                }
                else
                {
                    objectToSave = await _dBContext.Companies.FindAsync(companyDto.CompanyId);
                    if (objectToSave == null)
                        return NotFound();

                    objectToSave.Address = companyDto.Address;
                    objectToSave.LastUpdateBy = companyDto.LastUpdateBy;
                    objectToSave.Name = companyDto.Name;
                    objectToSave.PhoneNumber = companyDto.TelNumber;
                    objectToSave.CelphoneNumber = companyDto.CelNumber;
                    objectToSave.Email = companyDto.Email;
                    objectToSave.OwnerInfo = companyDto.OwnerInfo;
                    
                    if (objectToSave.ExpirationDate == null)
                        objectToSave.ExpirationDate = Util.GetMxDateTime();

                    _dBContext.Entry(objectToSave).State = EntityState.Modified;
                }

                await _dBContext.SaveChangesAsync();
                return Ok(new CompanyDto()
                {
                    CompanyId = objectToSave.CompanyId,
                    Name = objectToSave.Name
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveCompany.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpPost("SetSettings")]
        public async Task<ActionResult> SetSettings([FromHeader] string RequestorId, [FromBody] CompanySettingsDto settingsDto)
        {
            Company objectToSave = null;
            try
            {
                if (settingsDto.CompanyId  <= 0)
                    return NotFound();

                var user = await Util.ValidateRequestor(RequestorId, Role.Admin, _dBContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

               
                objectToSave = await _dBContext.Companies.FindAsync(settingsDto.CompanyId);
                if (objectToSave == null)
                    return NotFound();

                
                objectToSave.PaymentsJson = Util.XmlSerializeSettings(settingsDto.Settings);
                _dBContext.Entry(objectToSave).State = EntityState.Modified;

                await _dBContext.SaveChangesAsync();
                return Ok(0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SetSettings.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpGet("GetSettings")]
        public async Task<ActionResult> GetSettings([FromHeader] string RequestorId, int companyId)
        {
            try
            {
                if (companyId <= 0)
                    return NotFound();

                var user = await Util.ValidateRequestor(RequestorId, Role.Admin, _dBContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);


                var companySettings = await _dBContext.Companies.FindAsync(companyId);
                if (companySettings == null)
                    return NotFound();

                var settingsStr = companySettings.PaymentsJson ?? "";
                if (string.IsNullOrEmpty(settingsStr))
                {
                    return Ok();
                }
                var settings = Util.DeserializeSettings(settingsStr);
                
                return Ok(settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetSettings.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpPost("SetCompanyStatus")]
        public async Task<ActionResult> SetCompanyStatus([FromHeader] string RequestorId, [FromBody] SetStatusDto statusDto)
        {
            Company objectToSave = null;
            try
            {
                if (statusDto.ObjectId <= 0)
                    return NotFound();

                var user = await Util.ValidateRequestor(RequestorId, Role.SysAdmin, _dBContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                var statusId = EConverter.GetEnumFromValue<CompanyStatus>(statusDto.StatusId);
               

                objectToSave = await _dBContext.Companies.FindAsync(statusDto.ObjectId);
                if (objectToSave == null)
                    return NotFound();

                objectToSave.StatusId = statusDto.StatusId;
                _dBContext.Entry(objectToSave).State = EntityState.Modified;

                await _dBContext.SaveChangesAsync();
                return Ok(objectToSave.CompanyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SetCompanyStatus.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpPost("SetCompanyLicense")]
        public async Task<ActionResult> SetCompanyLicense([FromHeader] string RequestorId, [FromBody] SetValueDto valDto)
        {
            Company objectToSave = null;
            try
            {
                if (valDto.ObjectId <= 0)
                    return NotFound();

                var user = await Util.ValidateRequestor(RequestorId, Role.SysAdmin, _dBContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                objectToSave = await _dBContext.Companies.FindAsync(valDto.ObjectId);
                if (objectToSave == null)
                    return NotFound();

                objectToSave.PlanId = valDto.ValueId;
                objectToSave.StatusId = (int)CompanyStatus.Active;
                _dBContext.Entry(objectToSave).State = EntityState.Modified;

                await _dBContext.SaveChangesAsync();
                return Ok(objectToSave.CompanyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SetCompanyLicense.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpPost("CheckExpiration")]
        public async Task<ActionResult> CheckExpiration(int companyId)
        {
            Company company = null;
            try
            {
                company = await _dBContext.Companies.FindAsync(companyId);
                if (company == null)
                    return NotFound();

                var expirationDate = company.ExpirationDate ?? Util.GetMxDateTime();

                if (expirationDate.Date > Util.GetMxDateTime().Date)
                {
                    company.StatusId = (int)CompanyStatus.Expired;

                    await _dBContext.SaveChangesAsync();
                    return Forbid("$_Pago_requerido");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SetCompanyStatus.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }


        [HttpPost("AddRent")]
        public async Task<ActionResult> AddRent([FromHeader] string RequestorId, [FromBody] RentDto rentDto)
        {
            Renta objectToSave = null;
            int sumExpirationDays = 0;
            try
            {
                if (rentDto.CompanyId <= 0)
                    return BadRequest("$_Compañia_Invalida");
                if (rentDto.Amount <= 0)
                    return BadRequest("$_El_monto_debe_ser_mayor_a_cero");

                var user = await Util.ValidateRequestor(RequestorId, Role.Ventas, _dBContext);

                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                var rentType = EConverter.GetEnumFromValue<RentTypeId>(rentDto.StatusId);
                if (rentType == default)
                    return BadRequest("$_Estatus_Invalido");

                var c = await _dBContext.Companies
                   .Include(c => c.Plan)
                   .FirstOrDefaultAsync(u => u.CompanyId == rentDto.CompanyId);
                if (c == null)
                    return NotFound("$_Compañia_Invalida");

                if (rentType == RentTypeId.Mensualidad && rentDto.Amount < c.Plan.Price)
                    return BadRequest("Revisa la cantidad ingresada");

                objectToSave = new Renta
                {
                    Id = rentDto.Id,
                    CompanyId = rentDto.CompanyId,
                    ReferenceDate = Util.GetMxDateTime(),
                    Amount = rentDto.Amount,
                    AddedByUserId = rentDto.AddedByUserId,
                    StatusId = rentDto.StatusId,
                    TipoRentaDesc = rentDto.TipoRentaDesc,
                    Notas = rentDto.Notas
                };


                var newExpirationDate = await SetExpirationDate(c, rentDto.Amount, rentType);

                objectToSave.ExpirationDate = newExpirationDate;

                _dBContext.Rentas.Add(objectToSave);
                await _dBContext.SaveChangesAsync();
                return Ok(objectToSave.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddRent.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }


        private async Task<DateTime> SetExpirationDate(Company company, decimal rentAmount, RentTypeId typeId)
        {
            var newExpirationDate = DateTime.Now;
            try
            {
                var expirationDate = company.ExpirationDate ?? Util.GetMxDateTime();

                if (Util.GetMxDateTime().Date > expirationDate.Date && expirationDate.AddDays(5) <= Util.GetMxDateTime())
                {
                    //status = CompanyStatus.Expired; -- If the company is expired then take the current day as the new initial subscription day
                    expirationDate = Util.GetMxDateTime();
                }
                
               
                switch (typeId)
                {
                    case RentTypeId.Mensualidad:
                        var numMonths = rentAmount / company.Plan.Price;
                        newExpirationDate = expirationDate.AddMonths((int)numMonths);
                        break;
                    case RentTypeId.Condonacion:
                    case RentTypeId.Extension:
                        int costDay = (int)(company.Plan.Price / 31);
                        int days = (int)(rentAmount / costDay);
                        newExpirationDate = expirationDate.AddDays(days);
                        break;
                    default:
                        break;
                }

                company.StatusId = (int)CompanyStatus.Active;
                company.ExpirationDate = newExpirationDate;

                _dBContext.Entry(company).State = EntityState.Modified;
                await _dBContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"No se pudo agregar el pago para la compañia: {company.Name}");

            }
            return newExpirationDate;
        }

        [HttpGet("GetPaymentHistory")]
        public async Task<IActionResult> GetPaymentHistory([FromHeader] string RequestorId, int companyId)
        {
            var user = await Util.ValidateRequestorSameCompanyOrTopRol(RequestorId, companyId, Role.Admin, _dBContext);
            if (user.UserId <= 0)
                return Unauthorized(AppConfig.NO_AUTORIZADO);

            var c = await _dBContext.Rentas.Where(c => c.CompanyId == companyId)
                                .Select(r => new RentDto
                                {
                                    AddedByUserId = r.AddedByUserId,
                                    StatusId = r.StatusId,
                                    Amount = r.Amount,
                                    CompanyId = companyId,
                                    Id = r.Id,
                                    ReferenceDate = r.ReferenceDate,
                                    TipoRentaDesc = r.TipoRentaDesc,
                                    ByUserName = r.AddedByUser.UserName,
                                    ExpirationDate = r.ExpirationDate == null ? Util.GetMxDateTime() : r.ExpirationDate.Value,
                                    Notas = r.Notas
                                })
                                .OrderByDescending(d => d.ReferenceDate)
                                .ToListAsync();
            c.ForEach(c => c.StatusIdUI = Enums.EConverter.GetEnumNameFromValue<RentTypeId>(c.StatusId));

            return Ok(c);
        }
    }
}
