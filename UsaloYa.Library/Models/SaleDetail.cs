using System;
using System.Collections.Generic;

namespace UsaloYa.Library.Models;

public partial class SaleDetail
{
    public int SaleId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }

    public decimal BuyPrice { get; set; }

    public int? PriceLevel { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Sale Sale { get; set; } = null!;
}
