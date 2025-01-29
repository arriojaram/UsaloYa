namespace UsaloYa.API.DTO
{
    public class Product4ListDto
    {
        public string Sku { get; set; }
        public string Description { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public bool Discontinued { get; set; }
        public int CompanyId { get; set; }
    }

    public class Product4InventariotDto : Product4ListDto
    {
        public int? UnitsInStock { get; set; }
        public int? UnitsInVentario { get; set; }
        public int? InVentarioAlertLevel { get; set; } //3:Normal, 2:Warning, 1:Critial
        public int? AlertaStockNumProducts { get; set; }
    }
}
