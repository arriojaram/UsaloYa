using System;
using System.Collections.Generic;

namespace UsaloYa.API.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? CategoryId { get; set; }

    public int? SupplierId { get; set; }

    public decimal? UnitPrice { get; set; }

    public int UnitsInStock { get; set; }

    public bool Discontinued { get; set; }

    public string? ImgUrl { get; set; }

    public DateTime DateAdded { get; set; }

    public DateTime DateModified { get; set; }

    public decimal? Weight { get; set; }

    public string? Sku { get; set; }

    public string? Barcode { get; set; }

    public string? Brand { get; set; }

    public string? Color { get; set; }

    public string? Size { get; set; }

    public int CompanyId { get; set; }

    public decimal? BuyPrice { get; set; }

    public decimal? UnitPrice1 { get; set; }

    public decimal? UnitPrice2 { get; set; }

    public decimal? UnitPrice3 { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
}
