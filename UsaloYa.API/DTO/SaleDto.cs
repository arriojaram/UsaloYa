namespace UsaloYa.API.DTO
{
    public struct SaleDto
    {
        public int SaleId { get; set; }
        public int? CustomerId { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Tax { get; set; }
        
        public string Notes { get; set; }
        public int UserId { get; set; }
        public int CompanyId { get; set; }

        public List<SaleDetailsDto> SaleDetailsList { get; set; }
    }

   
}
