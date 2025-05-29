using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsaloYa.API.Config;
using UsaloYa.API.Security;
using UsaloYa.Dto;
using UsaloYa.Dto.Enums;
using UsaloYa.Dto.Utils;
using UsaloYa.Library.Models;
using UsaloYa.Services;


namespace UsaloYa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(AccessValidationFilter))]
    public class CustomerController : Controller
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly DBContext _dBContext;
        private readonly AppConfig _settings;

        public CustomerController(DBContext dBContext, ILogger<CustomerController> logger, AppConfig settings)
        {
            _logger = logger;
            _dBContext = dBContext;
            _settings = settings;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] int companyId, string nameOrPhoneorEmail = "-1")
        {
            try
            {
                var customers = (string.IsNullOrEmpty(nameOrPhoneorEmail) || string.Equals(nameOrPhoneorEmail, "-1", StringComparison.OrdinalIgnoreCase))
                    ? await _dBContext.Customers.Where(c => (c.CompanyId == companyId || companyId == 0)).OrderByDescending(u => u.CustomerId).Take(50).ToListAsync()
                    : await _dBContext.Customers
                        .Where(u => (
                                (u.FirstName.Contains(nameOrPhoneorEmail) || u.LastName1.Contains(nameOrPhoneorEmail) || (u.LastName2??"$1").Contains(nameOrPhoneorEmail))
                                || (u.Email != null && (u.Email.Contains(nameOrPhoneorEmail) || nameOrPhoneorEmail.Contains(u.Email??"$1")))
                                || (u.CellPhoneNumber != null && (u.CellPhoneNumber.Contains(nameOrPhoneorEmail) || nameOrPhoneorEmail.Contains(u.CellPhoneNumber ?? "$1")))
                                || (u.WorkPhoneNumber != null && (u.WorkPhoneNumber.Contains(nameOrPhoneorEmail) || nameOrPhoneorEmail.Contains(u.WorkPhoneNumber ?? "$1")))
                                )
                                && u.CompanyId == companyId)
                        .OrderBy(u => u.FirstName)
                        .ThenBy(u => u.LastName1)
                        .Take(50)
                        .ToListAsync();

                var customerDtos = customers.Select(u => new CustomerDto
                {
                  LastName1 = u.LastName1,
                  CompanyId = u.CompanyId,
                  CellPhoneNumber = u.CellPhoneNumber,
                  WorkPhoneNumber = u.WorkPhoneNumber,
                  Address = u.Address,
                  CustomerId = u.CustomerId,
                  Email = u.Email,
                  FirstName = u.FirstName,
                  LastName2 = u.LastName2?? "",
                  Notes = u.Notes
                });

                return Ok(customerDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAll.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }


        [HttpGet("GetCustomer")]
        public async Task<IActionResult> GetCustomer(int customerId)
        {
            var c = await _dBContext.Customers.FirstOrDefaultAsync(u => u.CustomerId == customerId);
            if (c == null)
                return NotFound();

            var customerResponseDto = new CustomerDto()
            {
               CustomerId = c.CustomerId,
               Address = c.Address,
               CellPhoneNumber = c.CellPhoneNumber,
               CompanyId = c.CompanyId,
               Email = c.Email,
               FirstName = c.FirstName,
               LastName1 = c.LastName1,
               LastName2 = c.LastName2,
               Notes = c.Notes,
               WorkPhoneNumber = c.WorkPhoneNumber
            };

            return Ok(customerResponseDto);
        }

        [HttpPost("SaveCustomer")]
        public async Task<ActionResult> SaveCustomer([FromHeader] string RequestorId, [FromBody] CustomerDto customerDto)
        {
            Customer objectToSave = null;
            try
            {
                var user = await HeaderValidatorService.ValidateRequestor(RequestorId, Role.User, _dBContext);

                if (customerDto.CustomerId == 0)
                {
                    var existsObject = await _dBContext.Customers.AnyAsync(
                            c => (
                            (c.CellPhoneNumber?? "-0") == (customerDto.CellPhoneNumber?? "-1")
                                || (c.WorkPhoneNumber?? "-0") == (customerDto.CellPhoneNumber ?? "-1")
                                || (c.Email?? "-0") == (customerDto.Email ?? "-1")
                            ) && c.CompanyId == customerDto.CompanyId);
                    
                    if (existsObject)
                        return Conflict(new { message = "$_Email_O_Telefono_Existente" });

                    if (user.CompanyStatusId == (int)CompanyStatus.Free)
                    {
                        var numExistingRecords = await _dBContext.Customers.CountAsync(c => c.CompanyId == customerDto.CompanyId);
                        if (numExistingRecords >= _settings.FreeRoleMaxCustomers)
                        {
                            return Conflict(new { message = _settings.FreeRoleMaxLimitReachedMsg });
                        }
                    }

                    objectToSave = new Customer
                    {
                        Address = Utils.EmptyToNull(customerDto.Address),
                        CellPhoneNumber = customerDto.CellPhoneNumber,
                        WorkPhoneNumber = Utils.EmptyToNull(customerDto.WorkPhoneNumber),
                        Email = Utils.EmptyToNull(customerDto.Email),
                        FirstName = customerDto.FirstName,
                        CompanyId = customerDto.CompanyId,
                        LastName1 = customerDto.LastName1,
                        LastName2 = Utils.EmptyToNull(customerDto.LastName2),
                        Notes = Utils.EmptyToNull(customerDto.Notes),
                    };
                    _dBContext.Customers.Add(objectToSave);
                }
                else
                {
                    objectToSave = await _dBContext.Customers.FindAsync(customerDto.CustomerId);
                    if (objectToSave == null)
                        return NotFound();

                    objectToSave.FirstName = customerDto.FirstName;
                    objectToSave.LastName1 = customerDto.LastName1;
                    objectToSave.LastName2 = Utils.EmptyToNull(customerDto.LastName2);
                    objectToSave.Address = Utils.EmptyToNull(customerDto.Address);
                    objectToSave.CellPhoneNumber = customerDto.CellPhoneNumber;
                    objectToSave.WorkPhoneNumber = Utils.EmptyToNull(customerDto.WorkPhoneNumber);
                    objectToSave.Email = Utils.EmptyToNull(customerDto.Email);
                    objectToSave.Notes = Utils.EmptyToNull(customerDto.Notes);

                    _dBContext.Entry(objectToSave).State = EntityState.Modified;
                }

                await _dBContext.SaveChangesAsync();
                return Ok(objectToSave);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveCustomer.ApiError");

                // Return a 500 Internal Server Error with a custom message
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }


    }
}
