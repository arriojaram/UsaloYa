using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace UsaloYa.API.Models;

[Index(nameof(UserName), IsUnique = true)]
public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string Token { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public int CompanyId { get; set; }

    public int GroupId { get; set; }

    public DateTime? LastAccess { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual Group Group { get; set; } = null!;

    public virtual bool IsEnabled {get; set;} = true;
}
