using System.ComponentModel.DataAnnotations.Schema;

namespace UsaloYa.API.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }

        public string? Address { get; set; }

        public string? Notes { get; set; }

        public string FirstName { get; set; } = null!;
        
        public string LastName1 { get; set; } = null!;

        public string? LastName2 { get; set; } = null!;

        public string? WorkPhoneNumber { get; set; }

        public string? CellPhoneNumber { get; set; }

        public string? Email { get; set; } 

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column(TypeName = "varchar(150)")]
        public string FullName { get; private set; }

        public int CompanyId { get; set; }

        public Company Company { get; set; } = null!;

        public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}
