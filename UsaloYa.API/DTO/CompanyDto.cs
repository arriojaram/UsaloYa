namespace UsaloYa.API.DTO
{
    public class CompanyDto
    {
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? LastUpdatedBy { get; set; }
        public string PaymentsJson { get; set; }
    }
}
