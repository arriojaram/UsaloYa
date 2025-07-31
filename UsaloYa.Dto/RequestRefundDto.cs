namespace UsaloYa.Dto
{
    public class RequestRefundDto
    {
        public int SaleId { get; set; }
        public int UserId { get; set; }
        public string? Name { get; set; }
        public string RefundMethod { get; set; }
        public DateTime SaleDate { get; set; }
        
        public List<RefundProductDto> ProductRefundList { get; set; }

    }
}
