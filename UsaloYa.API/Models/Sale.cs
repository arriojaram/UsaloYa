using System;
using System.Collections.Generic;

namespace UsaloYa.API.Models;

public partial class Sale
{
    public int SaleId { get; set; }

    public int? CustomerId { get; set; }

    public DateTime SaleDate { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public decimal Tax { get; set; }

    public string Status { get; set; } = null!;

    public decimal TotalSale { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
}
