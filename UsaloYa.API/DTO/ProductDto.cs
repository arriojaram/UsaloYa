namespace UsaloYa.API.DTO
{
    public struct ProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? CategoryId { get; set; }
        public int? SupplierId { get; set; }
        public decimal? UnitPrice { get; set; }
        public int UnitsInStock { get; set; }
        public bool Discontinued { get; set; }
        public string ImgUrl { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
        public decimal? Weight { get; set; }
        public string SKU { get; set; }
        public string Barcode { get; set; }
        public string Brand { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public int CompanyId { get; set; }
    }
}
