using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace UsaloYa.API.Models;

public partial class DBContext : DbContext
{
    public DBContext()
    {
    }

    public DBContext(DbContextOptions<DBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<PlanRenta> PlanRentas { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Renta> Rentas { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<SaleDetail> SaleDetails { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("Company");

            entity.HasIndex(e => e.CreatedBy, "IX_Company_CreatedBy");

            entity.HasIndex(e => e.LastUpdateBy, "IX_Company_LastUpdateBy");

            entity.Property(e => e.Address)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.CelphoneNumber)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CreationDate).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.ExpirationDate).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.OwnerInfo).HasMaxLength(500);
            entity.Property(e => e.PaymentsJson).HasColumnType("xml");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.CompanyCreatedByNavigations).HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.LastUpdateByNavigation).WithMany(p => p.CompanyLastUpdateByNavigations).HasForeignKey(d => d.LastUpdateBy);

            entity.HasOne(d => d.Plan).WithMany(p => p.Companies)
               .HasForeignKey(d => d.PlanId)
               .HasConstraintName("FK_Company_PlanRentas");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK_Customer_Id");

            entity.HasIndex(e => e.CompanyId, "IX_Customer_CompanyId");

            entity.HasIndex(e => e.Email, "IX_Customer_Email");

            entity.HasIndex(e => new { e.FirstName, e.LastName1, e.LastName2 }, "IX_Customer_Name");

            entity.HasIndex(e => new { e.CellPhoneNumber, e.WorkPhoneNumber }, "IX_Customer_Phone");

            entity.Property(e => e.Address)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.CellPhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(35)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FullName)
                .HasMaxLength(152)
                .IsUnicode(false)
                .HasComputedColumnSql("(concat([FirstName],' ',[LastName1],coalesce(' '+[LastName2],'')))", false);
            entity.Property(e => e.LastName1)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName2)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Notes)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.WorkPhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.Company).WithMany(p => p.Customers)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Customer_Company");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasIndex(e => e.CompanyId, "IX_Groups_CompanyId");

            entity.Property(e => e.Description)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Permissions).HasColumnType("xml");

            entity.HasOne(d => d.Company).WithMany(p => p.Groups)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Groups_Company");
        });

        modelBuilder.Entity<PlanRenta>(entity =>
        {
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.Price)
                .HasDefaultValueSql("((1))")
                .HasColumnType("decimal(10, 0)");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(e => new { e.Name, e.Description }, "IX_Products");

            entity.HasIndex(e => new { e.CompanyId, e.Barcode, e.Sku }, "IX_Products_Barcode_SKU")
                .IsUnique()
                .HasFilter("([Barcode] IS NOT NULL AND [SKU] IS NOT NULL)");

            entity.HasIndex(e => e.CompanyId, "IX_Products_CompanyId");

            entity.Property(e => e.Barcode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Brand)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.BuyPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Color)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DateAdded)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateModified)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ImgUrl)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Size)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SKU");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UnitPrice1).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UnitPrice2).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UnitPrice3).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Weight).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Company).WithMany(p => p.Products)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Company");
        });

        modelBuilder.Entity<Renta>(entity =>
        {
            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ReferenceDate).HasColumnType("datetime");
            entity.Property(e => e.TipoRentaDesc)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.AddedByUser).WithMany(p => p.Renta)
                .HasForeignKey(d => d.AddedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Rentas_Users");

            entity.HasOne(d => d.Company).WithMany(p => p.Renta)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Rentas_Company");
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasIndex(e => e.CompanyId, "IX_Sales_CompanyId");

            entity.HasIndex(e => e.CustomerId, "IX_Sales_CustomerId");

            entity.HasIndex(e => e.UserId, "IX_Sales_UserId");

            entity.Property(e => e.Notes)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SaleDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValueSql("('Completada')");
            entity.Property(e => e.Tax).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TotalSale).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Company).WithMany(p => p.Sales)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sale_Company");

            entity.HasOne(d => d.Customer).WithMany(p => p.Sales)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Sales_Customers");

            entity.HasOne(d => d.User).WithMany(p => p.Sales)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sales_Users");
        });

        modelBuilder.Entity<SaleDetail>(entity =>
        {
            entity.HasKey(e => new { e.SaleId, e.ProductId });

            entity.HasIndex(e => e.ProductId, "IX_SaleDetails_ProductId");

            entity.Property(e => e.BuyPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Product).WithMany(p => p.SaleDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SaleDetails_Products");

            entity.HasOne(d => d.Sale).WithMany(p => p.SaleDetails)
                .HasForeignKey(d => d.SaleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sale_SaleDetails");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK_User_Id");

            entity.HasIndex(e => e.CompanyId, "IX_Users_CompanyId");

            entity.HasIndex(e => e.CreatedBy, "IX_Users_CreatedBy");

            entity.HasIndex(e => e.GroupId, "IX_Users_GroupId");

            entity.HasIndex(e => e.LastUpdateBy, "IX_Users_LastUpdateBy");

            entity.HasIndex(e => e.UserName, "IX_Users_UserName").IsUnique();

            entity.Property(e => e.CreationDate).HasColumnType("datetime");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.IsEnabled)
                .IsRequired()
                .HasDefaultValueSql("(CONVERT([bit],(0)))");
            entity.Property(e => e.LastAccess).HasColumnType("datetime");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Token)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Company).WithMany(p => p.Users)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Company");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InverseCreatedByNavigation).HasForeignKey(d => d.CreatedBy);

            entity.HasOne(d => d.Group).WithMany(p => p.Users)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Groups");

            entity.HasOne(d => d.LastUpdateByNavigation).WithMany(p => p.InverseLastUpdateByNavigation).HasForeignKey(d => d.LastUpdateBy);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
