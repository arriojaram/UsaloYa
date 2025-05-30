using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsaloYa.API.Config;
using UsaloYa.API.Security;
using UsaloYa.Dto;
using UsaloYa.Dto.Enums;
using UsaloYa.Dto.Utils;
using UsaloYa.Library.Models;
using UsaloYa.Services;
using UsaloYa.Services.interfaces;


namespace UsaloYa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(AccessValidationFilter))]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly ICustomerService _customerService;
        private readonly DBContext _dBContext;
        private readonly IConfiguration _config;

        public CustomerController(DBContext dBContext, ICustomerService customerService, ILogger<CustomerController> logger, IConfiguration config)
        {
            _logger = logger;
            _customerService = customerService;
            _dBContext = dBContext;
            _config = config;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] int companyId, string nameOrPhoneOrEmail = "-1")
        {
            try
            {
                var customers = await _customerService.GetAllCustomers(companyId, nameOrPhoneOrEmail);
                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAll.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpGet("GetCustomer")]
        public async Task<IActionResult> GetCustomer(int customerId)
        {
            try
            {
                var customer = await _customerService.GetCustomerById(customerId);
                return Ok(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCustomer.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpPost("SaveCustomer")]
        public async Task<IActionResult> SaveCustomer([FromHeader] string RequestorId, [FromBody] CustomerDto customerDto)
        {
            try
            {
                var user = await HeaderValidatorService.ValidateRequestor(RequestorId, Role.User, _dBContext);
                if (user.UserId <= 0)
                    return Unauthorized(AppConfig.NO_AUTORIZADO);

                var savedCustomer = await _customerService.SaveCustomer(customerDto);
                return Ok(savedCustomer);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "SaveCustomer.ValidationError");
                return Conflict(new { message = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveCustomer.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }
    }
}
