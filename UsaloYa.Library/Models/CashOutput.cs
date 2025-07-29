using System;
using System.Collections.Generic;

namespace UsaloYa.Library.Models;

public partial class CashOutput
{
    public int OutputId { get; set; }

    public decimal Balance { get; set; }

    public string Reason { get; set; } = null!;

    public DateTime ReferenceDate { get; set; }

    public int UserId { get; set; }

    public bool IsCashCount { get; set; }

    public long CashCountId { get; set; }

    public virtual User User { get; set; } = null!;
}
