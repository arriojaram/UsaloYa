using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsaloYa.API.Security;
using UsaloYa.Dto.Enums;
using UsaloYa.Dto;
using UsaloYa.Services.interfaces;

namespace UsaloYa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaleController : ControllerBase
    {
        private readonly ILogger<SaleController> _logger;
        private readonly ISaleService _saleService;

        public SaleController(ISaleService saleService, ILogger<SaleController> logger)
        {
            _logger = logger;
            _saleService = saleService;
        }

        [HttpPost("AddSale")]
        public async Task<IActionResult> AddSale([FromBody] SaleDto sale)
        {
            try
            {
                if (sale.CompanyId == 0)
                    return BadRequest(new { Message = "$_Empresa_O_Venta_Invalida" });

                int saleId = await _saleService.AddSale(sale);

                if (sale.SaleDetailsList != null)
                    await _saleService.AddProductsToSale(saleId, sale.SaleDetailsList);

                return Ok(saleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddSale.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpPost("UpdateSaleStatus")]
        public async Task<IActionResult> UpdateSaleStatus([FromBody] UpdateSaleDto saleStatus)
        {
            try
            {
                var updated = await _saleService.UpdateSaleStatus(saleStatus.SaleId, saleStatus.Status);
                if (!updated) return NotFound();

                await _saleService.UpdateStockAfterStatusChange(saleStatus.SaleId, saleStatus.CompanyId, saleStatus.Status == SaleStatus.Cancelada);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateSaleStatus.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }
    }
}
