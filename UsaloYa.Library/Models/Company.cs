
namespace UsaloYa.Library.Models;

public partial class Company
{
    public int CompanyId { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreationDate { get; set; }

    public int? LastUpdateBy { get; set; }

    public string? PaymentsJson { get; set; }

    public int StatusId { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public string? PhoneNumber { get; set; }

    public string? CelphoneNumber { get; set; }

    public string? Email { get; set; }

    public string? OwnerInfo { get; set; }

    public int? PlanId { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual User? LastUpdateByNavigation { get; set; }

    public virtual PlanRenta? Plan { get; set; }

    public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<Renta> Renta { get; set; } = new List<Renta>();

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
