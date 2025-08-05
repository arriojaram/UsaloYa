using System;

namespace UsaloYa.Dto
{
    public class CashCountDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int StatusId { get; set; }
        public DateTime ReferenceDate { get; set; }
        public decimal InitialBalance { get; set; }
        public decimal Cash { get; set; }
        public decimal? CredictCard { get; set; }
        public decimal? Spei { get; set; }
        public decimal? CashOutputTotal { get; set; }
        public string? Notes { get; set; }
        public decimal? FinalCash { get; set; }
        
        // Propiedades adicionales para la UI
        public string? UserName { get; set; }
    }
} 