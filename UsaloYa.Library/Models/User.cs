using System;
using System.Collections.Generic;

namespace UsaloYa.Library.Models;

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

    public bool? IsEnabled { get; set; }

    public int StatusId { get; set; }

    public int? CreatedBy { get; set; }

    public int? LastUpdateBy { get; set; }

    public DateTime? CreationDate { get; set; }

    public int? RoleId { get; set; }

    public string? DeviceId { get; set; }

    public Guid? SessionToken { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<Company> CompanyCreatedByNavigations { get; set; } = new List<Company>();

    public virtual ICollection<Company> CompanyLastUpdateByNavigations { get; set; } = new List<Company>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual ICollection<User> InverseCreatedByNavigation { get; set; } = new List<User>();

    public virtual ICollection<User> InverseLastUpdateByNavigation { get; set; } = new List<User>();

    public virtual User? LastUpdateByNavigation { get; set; }

    public virtual ICollection<Renta> Renta { get; set; } = new List<Renta>();

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
