using System;
using System.Collections.Generic;

namespace UsaloYa.API.Models;

public partial class PlanRenta
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Notes { get; set; }

    public decimal Price { get; set; }

    public int StatusId { get; set; }

    public virtual ICollection<Company> Companies { get; set; } = new List<Company>();
}
