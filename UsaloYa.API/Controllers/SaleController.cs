using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using UsaloYa.API.DTO;
using UsaloYa.API.Models;
using UsaloYa.API.Security;
using UsaloYa.API.Utils;
using static UsaloYa.API.Enumerations;

namespace UsaloYa.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaleController : Controller
    {
        private readonly ILogger<SaleController> _logger;
        private readonly DBContext _dBContext;

        public SaleController(DBContext dBContext, ILogger<SaleController> logger)
        {
            _logger = logger;
            _dBContext = dBContext;
        }

        [HttpPost("AddSale")]
        public async Task<IActionResult> AddSale([FromBody] SaleDto sale)
        {
            try
            {
                if (sale.Equals(default(SaleDto)) || sale.CompanyId == 0)
                {
                    return BadRequest(new { Message = "$_Empresa_O_Venta_Invalida" });
                }
                if(sale.CustomerId == 0) 
                    sale.CustomerId = null;

                var newSale = new Sale() 
                {
                    CompanyId = sale.CompanyId,
                    CustomerId = sale.CustomerId,
                    Notes = sale.Notes,
                    PaymentMethod = sale.PaymentMethod,
                    SaleDate = Util.GetMxDateTime(),
                    Status = "Abierta",
                    Tax = sale.Tax,
                    UserId = sale.UserId,
                    TotalSale = 0
                };

                _dBContext.Sales.Add(newSale);
                await _dBContext.SaveChangesAsync();

                if(sale.SaleDetailsList != null) 
                    return await AddProductsToSale(newSale.SaleId, sale.SaleDetailsList);
                
                return Ok(sale.SaleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddSale.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [NonAction]
        private async Task<IActionResult> AddProductsToSale(int saleId, List<SaleDetailsDto> saleDetails)
        {
            decimal totalSale = 0;
            try
            {
                foreach (var detail in saleDetails)
                {
                    totalSale += detail.TotalPrice;

                    var product = new SaleDetail()
                    {
                        SaleId = saleId,
                        ProductId = detail.ProductId,
                        Quantity = detail.Quantity,
                        TotalPrice = detail.TotalPrice,
                        UnitPrice = detail.UnitPrice,
                        PriceLevel = detail.PriceLevel
                    };

                    _dBContext.SaleDetails.Add(product);
                    await UpdateStock(product.ProductId, product.Quantity);
                }

                // Commit DB changes
                if (saleDetails.Count > 0)
                {
                    await _dBContext.SaveChangesAsync();

                    if (totalSale > 0)
                        await UpdateTotalSale(saleId, totalSale);

                }

                return Ok(saleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddSaleDetails.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        /// <summary>
        /// Updates the stock of the product information. This function works together with the function
        /// AddProductsToSale where the DB commit action is executed.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="selledItems"></param>
        /// <returns></returns>
        [NonAction]
        private async Task UpdateStock(int productId, int selledItems)
        {
            try
            {
                var existingProduct = await _dBContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId);

                if (existingProduct != null)
                {
                    existingProduct.UnitsInStock = existingProduct.UnitsInStock - selledItems;
                    _dBContext.Entry(existingProduct).State = EntityState.Modified;
                    await _dBContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateStock.ApiError");
            }
        }

        [NonAction]
        private async Task<IActionResult> UpdateTotalSale(int saleId, decimal totalSale)
        {
            try
            {
                var sale = await _dBContext.Sales.FindAsync(saleId);
                if (sale == null)
                {
                    return NotFound();
                }

                sale.SaleDate = Utils.Util.GetMxDateTime();
                sale.Status = SaleStatus.Completada.ToString();
                
                sale.TotalSale = totalSale;
                _dBContext.Entry(sale).State = EntityState.Modified;
                await _dBContext.SaveChangesAsync();
                return Ok();    
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateTotalSale.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [HttpPost("UpdateSaleStatus")]
        [ServiceFilter(typeof(AccessValidationFilter))]
        public async Task<IActionResult> UpdateSaleStatus([FromBody] UpdateSaleDto saleStatus)
        {
            try
            {
                var sale = await _dBContext.Sales.FindAsync(saleStatus.SaleId);
                if (sale == null)
                {
                    return NotFound();
                }

                sale.Status = saleStatus.Status.ToString();
                _dBContext.Entry(sale).State = EntityState.Modified;
                await _dBContext.SaveChangesAsync();

                await UpdateStockAfterStatusChange(sale.SaleId, saleStatus.CompanyId, saleStatus.Status == SaleStatus.Cancelada);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateTotalSale.ApiError");
                return StatusCode(500, new { message = "$_Excepcion_Ocurrida" });
            }
        }

        [NonAction]
        public async Task<bool> UpdateStockAfterStatusChange(int saleId, int companyId, bool isCancelAction)
        {
            try
            {
                var saleProds = await _dBContext.SaleDetails
                                .Include(d => d.Product)
                            .Where(s => s.SaleId == saleId && s.Sale.CompanyId == companyId)
                            .Select(r => new
                            {
                                r.Product.ProductId,
                                r.Quantity,
                            }).ToListAsync();
                
                foreach (var item in saleProds)
                {
                    var prod = await _dBContext.Products.FindAsync(item.ProductId);
                    if (prod != null)
                    {
                        if(isCancelAction)
                            prod.UnitsInStock = prod.UnitsInStock + item.Quantity;
                        else
                            prod.UnitsInStock = prod.UnitsInStock - item.Quantity;

                        _dBContext.Entry(prod).State = EntityState.Modified;
                    }

                    await _dBContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateStockAfterStatusChange.ApiError");
            }
            return true;
        }
    }
}
