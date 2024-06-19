using System;
using System.Collections.Generic;

namespace UsaloYa.API.Models;

public partial class Group
{
    public int GroupId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string Permissions { get; set; } = null!;

    public int CompanyId { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
