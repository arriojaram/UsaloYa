using System.Security.Principal;

namespace UsaloYa.API.DTO
{
    public class StockTotalDto
    {
        public int ProductId { get; set; }
        public int CompanyId { get; set; }
        public decimal UnitPrice { get; set; }
        public int UnitsInStock { get; set; }
        public bool Discontinued { get; set; }
    }
}
