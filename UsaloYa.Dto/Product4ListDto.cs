namespace UsaloYa.Dto
{
    public class Product4ListDto
    {
        public string? Sku { get; set; }
        public string? Description { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public bool Discontinued { get; set; }
        public int CompanyId { get; set; }
        public int CategoryId { get; set; }
    }

    public class Product4InventariotDto : Product4ListDto
    {
        public string Barcode { get; set; }
        public decimal? UnitsInStock { get; set; }

        public int Measure { get; set; }
        public decimal? TotalCashStock { get; set; }
        public decimal? UnitsInVentario { get; set; }
        public int? InVentarioAlertLevel { get; set; } //3:Normal, 2:Warning, 1:Critial
        public int? AlertaStockNumProducts { get; set; }
        public decimal UnitPrice { get; set; }
        public string CategoryName { get; set; }
        public bool IsInVentarioUpdated { get; set; }
    }

    public class InventoryDto
    {
        public int TotalProducts { get; set; }
        public decimal TotalProductUnits { get; set; }
        public decimal TotalCash { get; set; }
        public List<Product4InventariotDto> Products { get; set; }
    }
}
