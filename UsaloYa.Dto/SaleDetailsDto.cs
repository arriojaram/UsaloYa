namespace UsaloYa.Dto
{
    public struct SaleDetailsDto
    {
        public int SaleId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        public int PriceLevel { get; set; }
        public string Folio { get; set; }
    }
}
