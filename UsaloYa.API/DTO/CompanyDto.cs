namespace UsaloYa.API.DTO
{
    public class CompanyDto
    {
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? LastUpdateBy { get; set; }
        public string? PaymentsJson { get; set; }
        public int StatusId { get; set; }
        
        public DateTime? ExpirationDate { get; set; }
        public string? CreatedByUserName { get; set; }
        public string? CreatedByFullName { get; set; }

        public string? LastUpdateByUserName { get; set; }

        public string? TelNumber { get; set; }
        public string? CelNumber { get; set; }
        public string? Email { get; set; }
        public string? OwnerInfo { get; set; }

        public int? PlanId { get; set; }
        public string? PlanIdUI { get; set; }
        public decimal? PlanPrice { get; set; }
        public int? PlanNumUsers { get; set; }
    }

    
}
