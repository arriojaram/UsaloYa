namespace UsaloYa.API.DTO
{
    public class RentDto
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public DateTime ReferenceDate { get; set; }
        public Decimal Amount { get; set; }
        public int AddedByUserId { get; set; }
        public int StatusId { get; set; }
        public string? TipoRentaDesc { get; set; }

        public string? ByUserName { get; set; }
        public string? StatusIdUI { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string? Notas { get; set; }
    }
}
