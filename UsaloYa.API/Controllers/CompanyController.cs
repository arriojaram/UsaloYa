using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using UsaloYa.API.DTO;
using UsaloYa.API.Enums;
using UsaloYa.API.Models;
using UsaloYa.API.Utils;

namespace UsaloYa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        public async Task<IActionResult> GetAll4List([FromHeader] string RequestorId)
        {
            try
            {
                var companies = await _dBContext.Companies
                        .Select(c => new GenericObjectDto {
                                Name = c.Name,
                                CompanyId = c.CompanyId,
                                IsActive = !(c.StatusId == (int)Enums.CompanyStatus.Inactive
                                                || c.StatusId == (int)Enums.CompanyStatus.Expired)
                            })
                        .OrderBy(u => u.Name)
                        .ToListAsync();

                return Ok(companies);
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
            var userId = await Util.ValidateRequestor(RequestorId, Role.Admin, _dBContext);
            if (userId <= 0)
                return Unauthorized(RequestorId);

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
            int PlanId = 1; //TODO: Capturar desde la interfaz (Fase 3)
            try
            {
                var userId = await Util.ValidateRequestor(RequestorId, Role.SysAdmin, _dBContext);
                if (userId <= 0)
                    return Unauthorized(RequestorId);

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
                        ExpirationDate = Util.GetMxDateTime().AddMonths(1),
                        StatusId = (int)CompanyStatus.Inactive,
                        PhoneNumber = companyDto.TelNumber,
                        CelphoneNumber = companyDto.CelNumber,
                        Email = companyDto.Email,
                        OwnerInfo = companyDto.OwnerInfo,
                        PlanId = PlanId
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
                    objectToSave.PlanId = PlanId;

                    if (objectToSave.ExpirationDate == null) 
                        objectToSave.ExpirationDate = Util.GetMxDateTime();

                    _dBContext.Companies.Update(objectToSave);
                }

                await _dBContext.SaveChangesAsync();
                return Ok(objectToSave);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveCompany.ApiError");

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
                if(statusDto.ObjectId <= 0)
                    return NotFound();

                var userId = await Util.ValidateRequestor(RequestorId, Role.SysAdmin, _dBContext);
                if (userId <= 0)
                    return Unauthorized(RequestorId);

                var statusId = EConverter.GetEnumFromValue<CompanyStatus>(statusDto.StatusId);
                if (statusId == default)
                    return BadRequest("$_Estatus_Invalido");

                objectToSave = await _dBContext.Companies.FindAsync(statusDto.ObjectId);
                if (objectToSave == null)
                    return NotFound();

                objectToSave.StatusId = statusDto.StatusId;

                _dBContext.Companies.Update(objectToSave);
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


        [HttpPost("AddRent")]
        public async Task<ActionResult> AddRent([FromHeader] string RequestorId, [FromBody] RentDto rentDto)
        {
            Renta objectToSave = null;
            try
            {
                var userId = await Util.ValidateRequestor(RequestorId, Role.SysAdmin, _dBContext);
                if (userId <= 0)
                    return Unauthorized(RequestorId);

                var statusId = EConverter.GetEnumFromValue<RentStatusId>(rentDto.StatusId);
                if (statusId == default)
                    return BadRequest("$_Estatus_Invalido");


                objectToSave = new Renta
                {
                    Id = rentDto.Id,
                    CompanyId = rentDto.CompanyId,
                    ReferenceDate = Util.GetMxDateTime(),
                    Amount = rentDto.Amount,
                    AddedByUserId = rentDto.AddedByUserId,
                    StatusId = rentDto.StatusId,
                    TipoRentaDesc = rentDto.TipoRentaDesc
                };
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

        [HttpGet("GetPaymentHistory")]
        public async Task<IActionResult> GetPaymentHistory([FromHeader] string RequestorId, int companyId)
        {
            var userId = await Util.ValidateRequestorSameCompanyOrTopRol(RequestorId, companyId, Role.Admin, _dBContext);
            if (userId <= 0)
                return Unauthorized(RequestorId);

            var c = await _dBContext.Rentas
                                .Select(r => new RentDto { 
                                    AddedByUserId = r.AddedByUserId,
                                    StatusId = r.StatusId,
                                    Amount = r.Amount,
                                    CompanyId = companyId,
                                    Id = r.Id,
                                    ReferenceDate = r.ReferenceDate,
                                    TipoRentaDesc = r.TipoRentaDesc,
                                    ByUserName = r.AddedByUser.UserName,
                                })
                                .Where(c => c.CompanyId == companyId)
                                .OrderBy(d => d.ReferenceDate)
                                .ToListAsync();
            c.ForEach(c => c.StatusIdUI = Enums.EConverter.GetEnumNameFromValue<RentStatusId>(c.StatusId));

            return Ok(c);
        }
    }
}
