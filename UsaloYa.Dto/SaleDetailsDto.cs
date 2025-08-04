namespace UsaloYa.Dto
{
    public struct SaleDetailsDto
    {
        public int SaleId { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public int Measure { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        public int PriceLevel { get; set; }    
    }
}
