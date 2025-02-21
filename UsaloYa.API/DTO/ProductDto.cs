namespace UsaloYa.API.DTO
{
    public struct ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public string? Categoria { get; set; } // Usado para la importacion de producto

        public decimal? BuyPrice { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? UnitPrice1 { get; set; }
        public decimal? UnitPrice2 { get; set; }
        public decimal? UnitPrice3 { get; set; }

        public int UnitsInStock { get; set; }
        public bool Discontinued { get; set; }
       
        public string? SKU { get; set; }
        public string Barcode { get; set; }
        
        public int? LowInventoryStart { get; set; }
        public bool? IsInventarioUpdated { get; set; }

        public int CompanyId { get; set; }
        public bool? UpdateProduct { get; set; }
        public UpdateSettings? UpdateSettings { get; set; }
    }

    public class UpdateSettings
    {
        public bool UpdateNombre { get; set; }
        public bool UpdateCategoria { get; set; }
        public bool UpdatePrecioUnitario { get; set; }
        public bool UpdatePrecio1 { get; set; }
        public bool UpdatePrecio2 { get; set; }
        public bool UpdatePrecio3 { get; set; }
        public bool UpdateStock { get; set; }
        public bool UpdateAlertaExistencias { get; set; }
    }
}
