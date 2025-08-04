namespace UsaloYa.Dto
{
    public class RefundProductDto
    {
        public int ProductId { get; set; }
        public string Barcode { get; set; }
 
        public string? ProductName { get; set; }
        public string Reason { get; set; }
        public int Measure { get; set; }
        public decimal Quantity { get; set; }   
        public decimal UnitPriceRefund { get; set; }
        public decimal RefundAmount { get; set; }

    }
}
