using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsaloYa.Dto;
using UsaloYa.Library.Models;

namespace UsaloYa.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product4ListDto>> FilterProducts(int pageNumber, int categoryId, int companyId);
        Task<IEnumerable<Product4ListDto>> SearchProduct4List(int pageNumber, string keyword, int companyId);
        Task<IEnumerable<ProductDto>> SearchProductFull(int pageNumber, string keyword, int companyId);
        Task<ProductDto?> GetProduct(int productId, int companyId);
        Task<bool> ImportProduct(string requestorId, ProductDto productDto, int companyId);
        Task<int> GetOrCreateProductCategory(string? categoria, int companyId);
        Task<Product?> AddProduct(string requestorId, ProductDto productDto, int companyId);
        Task<IEnumerable<Product4InventariotDto>> GetInventarioByCategoryId(int categoryId, int companyId, int pageNumber);
        Task<List<Product4InventariotDto>> GetInventarioByAlertId(int alertLevel, int companyId, int pageNumber);
        Task<List<Product4InventariotDto>> GetInventarioWithDiscrepancias(int companyId, int pageNumber);
        Task<InventoryDto> GetInventarioTop50(string keyword, int companyId, int pageNumber);
        Task<List<Product4InventariotDto>> GetInventarioItemsUpdated(int companyId, int pageNumber);
        Task<Product4InventariotDto> SetInVentario(string barcode, decimal quantity, int companyId);
        Task<bool> SetAllUnitsStock(int companyId);
        Task<bool> ResetAllInVentario(int companyId);
        Task<decimal> SetUnitsInStockByProductId(int productId, int companyId);
        Task<decimal> SetUnitsInStock(int productId, decimal unitsInStock, bool isHardReset, int companyId);
        Task<bool> AddUnitsInStockByProductId(List<SetStockDto> productsToAdd, int companyId);
    }
}
