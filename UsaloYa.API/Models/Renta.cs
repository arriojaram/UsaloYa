using System;
using System.Collections.Generic;

namespace UsaloYa.API.Models;

public partial class Renta
{
    public int Id { get; set; }

    public int CompanyId { get; set; }

    public DateTime ReferenceDate { get; set; }

    public decimal Amount { get; set; }

    public int AddedByUserId { get; set; }

    public int StatusId { get; set; }

    public string? TipoRentaDesc { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public virtual User AddedByUser { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;
}
