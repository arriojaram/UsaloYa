using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsaloYa.Dto.Enums;
using UsaloYa.Dto;

namespace UsaloYa.Services.interfaces
{
    public interface ISaleService
    {
        Task<int> AddSale(SaleDto sale);
        Task<bool> AddProductsToSale(int saleId, List<SaleDetailsDto> saleDetails);
        Task<bool> UpdateStock(int productId, int selledItems);
        Task<bool> UpdateTotalSale(int saleId, decimal totalSale);
        Task<bool> UpdateSaleStatus(int saleId, SaleStatus status);
        Task<bool> UpdateStockAfterStatusChange(int saleId, int companyId, bool isCancelAction);
    }
}
