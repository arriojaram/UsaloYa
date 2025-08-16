namespace UsaloYa.Library.Models;

public partial class CashCount
{
    public long CashCountId { get; set; }

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

    public virtual ICollection<CashOutput> CashOutput { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
