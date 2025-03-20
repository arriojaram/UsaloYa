namespace UsaloYa.API.DTO
{
    public class LicenseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public decimal Price { get; set; }
        public int StatusId { get; set; }
        public int NumUsers { get; set; }
    }
}
