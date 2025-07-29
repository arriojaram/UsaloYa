using System;
using System.Collections.Generic;

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

    public decimal? CashOutput { get; set; }

    public string? Notes { get; set; }

    public string? FinalCash { get; set; }
}
