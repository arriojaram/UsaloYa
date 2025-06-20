﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UsaloYa.Library.Models;

#nullable disable

namespace UsaloYa.Library.Migrations
{
    [DbContext(typeof(DBContext))]
    partial class DBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Group", b =>
                {
                    b.Property<int>("GroupId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GroupId"));

                    b.Property<int>("CompanyId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasMaxLength(250)
                        .IsUnicode(false)
                        .HasColumnType("varchar(250)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Permissions")
                        .IsRequired()
                        .HasColumnType("xml");

                    b.HasKey("GroupId");

                    b.HasIndex(new[] { "CompanyId" }, "IX_Groups_CompanyId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("UsaloYa.Library.Models.Company", b =>
                {
                    b.Property<int>("CompanyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CompanyId"));

                    b.Property<string>("Address")
                        .HasMaxLength(250)
                        .IsUnicode(false)
                        .HasColumnType("varchar(250)");

                    b.Property<string>("CelphoneNumber")
                        .HasMaxLength(10)
                        .IsUnicode(false)
                        .HasColumnType("varchar(10)");

                    b.Property<int?>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreationDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Email")
                        .HasMaxLength(30)
                        .IsUnicode(false)
                        .HasColumnType("varchar(30)");

                    b.Property<DateTime?>("ExpirationDate")
                        .HasColumnType("datetime");

                    b.Property<int?>("LastUpdateBy")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("OwnerInfo")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("PaymentsJson")
                        .HasColumnType("xml");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(10)
                        .IsUnicode(false)
                        .HasColumnType("varchar(10)");

                    b.Property<int?>("PlanId")
                        .HasColumnType("int");

                    b.Property<int>("StatusId")
                        .HasColumnType("int");

                    b.HasKey("CompanyId");

                    b.HasIndex("PlanId");

                    b.HasIndex(new[] { "CreatedBy" }, "IX_Company_CreatedBy");

                    b.HasIndex(new[] { "LastUpdateBy" }, "IX_Company_LastUpdateBy");

                    b.ToTable("Company", (string)null);
                });

            modelBuilder.Entity("UsaloYa.Library.Models.Customer", b =>
                {
                    b.Property<int>("CustomerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CustomerId"));

                    b.Property<string>("Address")
                        .HasMaxLength(300)
                        .IsUnicode(false)
                        .HasColumnType("varchar(300)");

                    b.Property<string>("CellPhoneNumber")
                        .HasMaxLength(15)
                        .IsUnicode(false)
                        .HasColumnType("varchar(15)");

                    b.Property<int>("CompanyId")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .HasMaxLength(35)
                        .IsUnicode(false)
                        .HasColumnType("varchar(35)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasMaxLength(152)
                        .IsUnicode(false)
                        .HasColumnType("varchar(150)")
                        .HasComputedColumnSql("(concat([FirstName],' ',[LastName1],coalesce(' '+[LastName2],'')))", false);

                    b.Property<string>("LastName1")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("LastName2")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Notes")
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("varchar(500)");

                    b.Property<string>("WorkPhoneNumber")
                        .HasMaxLength(15)
                        .IsUnicode(false)
                        .HasColumnType("varchar(15)");

                    b.HasKey("CustomerId")
                        .HasName("PK_Customer_Id");

                    b.HasIndex(new[] { "CompanyId" }, "IX_Customer_CompanyId");

                    b.HasIndex(new[] { "Email" }, "IX_Customer_Email");

                    b.HasIndex(new[] { "FirstName", "LastName1", "LastName2" }, "IX_Customer_Name");

                    b.HasIndex(new[] { "CellPhoneNumber", "WorkPhoneNumber" }, "IX_Customer_Phone");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("UsaloYa.Library.Models.PlanRenta", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Notes")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("NumUsers")
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(10, 0)")
                        .HasDefaultValueSql("((1))");

                    b.Property<int>("StatusId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("PlanRentas");
                });

            modelBuilder.Entity("UsaloYa.Library.Models.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductId"));

                    b.Property<int?>("AlertaStockNumProducts")
                        .HasColumnType("int");

                    b.Property<string>("Barcode")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Brand")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<decimal?>("BuyPrice")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<int?>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Color")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<int>("CompanyId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateAdded")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<DateTime>("DateModified")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("varchar(500)");

                    b.Property<bool>("Discontinued")
                        .HasColumnType("bit");

                    b.Property<string>("ImgUrl")
                        .HasMaxLength(250)
                        .IsUnicode(false)
                        .HasColumnType("varchar(250)");

                    b.Property<int?>("InVentario")
                        .HasColumnType("int")
                        .HasComment("Valor utilizado para guardar informacion temporal del inventario del producto");

                    b.Property<bool?>("IsInVentarioUpdated")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .IsUnicode(false)
                        .HasColumnType("varchar(250)");

                    b.Property<string>("Size")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Sku")
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("SKU");

                    b.Property<int?>("SupplierId")
                        .HasColumnType("int");

                    b.Property<decimal?>("UnitPrice")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<decimal?>("UnitPrice1")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<decimal?>("UnitPrice2")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<decimal?>("UnitPrice3")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<int>("UnitsInStock")
                        .HasColumnType("int");

                    b.Property<decimal?>("Weight")
                        .HasColumnType("decimal(10, 2)");

                    b.HasKey("ProductId");

                    b.HasIndex("CategoryId");

                    b.HasIndex(new[] { "Name", "Description" }, "IX_Products");

                    b.HasIndex(new[] { "CompanyId", "Barcode", "Sku" }, "IX_Products_Barcode_SKU")
                        .IsUnique()
                        .HasFilter("([Barcode] IS NOT NULL AND [SKU] IS NOT NULL)");

                    b.HasIndex(new[] { "CompanyId" }, "IX_Products_CompanyId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("UsaloYa.Library.Models.ProductCategory", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoryId"));

                    b.Property<int>("CompanyId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("varchar(500)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.HasKey("CategoryId");

                    b.HasIndex("CompanyId");

                    b.ToTable("ProductCategory", (string)null);
                });

            modelBuilder.Entity("UsaloYa.Library.Models.Question", b =>
                {
                    b.Property<int>("QuestionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("QuestionId"));

                    b.Property<int?>("IdUser")
                        .HasColumnType("int");

                    b.Property<string>("QuestionName")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<bool>("Reply")
                        .HasColumnType("bit");

                    b.HasKey("QuestionId");

                    b.HasIndex("IdUser");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("UsaloYa.Library.Models.Renta", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AddedByUserId")
                        .HasColumnType("int");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<int>("CompanyId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ExpirationDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Notas")
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("varchar(500)");

                    b.Property<DateTime>("ReferenceDate")
                        .HasColumnType("datetime");

                    b.Property<int>("StatusId")
                        .HasColumnType("int");

                    b.Property<string>("TipoRentaDesc")
                        .HasMaxLength(15)
                        .IsUnicode(false)
                        .HasColumnType("varchar(15)");

                    b.HasKey("Id");

                    b.HasIndex("AddedByUserId");

                    b.HasIndex("CompanyId");

                    b.ToTable("Rentas");
                });

            modelBuilder.Entity("UsaloYa.Library.Models.Sale", b =>
                {
                    b.Property<int>("SaleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SaleId"));

                    b.Property<int>("CompanyId")
                        .HasColumnType("int");

                    b.Property<int?>("CustomerId")
                        .HasColumnType("int");

                    b.Property<string>("Folio")
                        .HasMaxLength(11)
                        .IsUnicode(false)
                        .HasColumnType("varchar(11)");

                    b.Property<string>("Notes")
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("varchar(500)");

                    b.Property<string>("PaymentMethod")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<DateTime>("SaleDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Status")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)")
                        .HasDefaultValueSql("('Completada')");

                    b.Property<decimal>("Tax")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<decimal>("TotalSale")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("SaleId");

                    b.HasIndex(new[] { "CompanyId" }, "IX_Sales_CompanyId");

                    b.HasIndex(new[] { "CustomerId" }, "IX_Sales_CustomerId");

                    b.HasIndex(new[] { "UserId" }, "IX_Sales_UserId");

                    b.ToTable("Sales");
                });

            modelBuilder.Entity("UsaloYa.Library.Models.SaleDetail", b =>
                {
                    b.Property<int>("SaleId")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<decimal>("BuyPrice")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<int?>("PriceLevel")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(10, 2)");

                    b.HasKey("SaleId", "ProductId");

                    b.HasIndex(new[] { "ProductId" }, "IX_SaleDetails_ProductId");

                    b.ToTable("SaleDetails");
                });

            modelBuilder.Entity("UsaloYa.Library.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("CodeVerification")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<int>("CompanyId")
                        .HasColumnType("int");

                    b.Property<int?>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreationDate")
                        .HasColumnType("datetime");

                    b.Property<string>("DeviceId")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.Property<bool?>("IsEnabled")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("(CONVERT([bit],(0)))");

                    b.Property<bool?>("IsVerifiedCode")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("(CONVERT([bit],(0)))");

                    b.Property<DateTime?>("LastAccess")
                        .HasColumnType("datetime");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.Property<int?>("LastUpdateBy")
                        .HasColumnType("int");

                    b.Property<int?>("RoleId")
                        .HasColumnType("int");

                    b.Property<Guid?>("SessionToken")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("StatusId")
                        .HasColumnType("int");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.HasKey("UserId")
                        .HasName("PK_User_Id");

                    b.HasIndex(new[] { "CompanyId" }, "IX_Users_CompanyId");

                    b.HasIndex(new[] { "CreatedBy" }, "IX_Users_CreatedBy");

                    b.HasIndex(new[] { "GroupId" }, "IX_Users_GroupId");

                    b.HasIndex(new[] { "LastUpdateBy" }, "IX_Users_LastUpdateBy");

                    b.HasIndex(new[] { "UserName" }, "IX_Users_UserName")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Group", b =>
                {
                    b.HasOne("UsaloYa.Library.Models.Company", "Company")
                        .WithMany("Groups")
                        .HasForeignKey("CompanyId")
                        .IsRequired()
                        .HasConstraintName("FK_Groups_Company");

                    b.Navigation("Company");
                });

            modelBuilder.Entity("UsaloYa.Library.Models.Company", b =>
                {
                    b.HasOne("UsaloYa.Library.Models.User", "CreatedByNavigation")
                        .WithMany("CompanyCreatedByNavigations")
                        .HasForeignKey("CreatedBy");

                    b.HasOne("UsaloYa.Library.Models.User", "LastUpdateByNavigation")
                        .WithMany("CompanyLastUpdateByNavigations")
                        .HasForeignKey("LastUpdateBy");

                    b.HasOne("UsaloYa.Library.Models.PlanRenta", "Plan")
                        .WithMany("Companies")
                        .HasForeignKey("PlanId")
                        .HasConstraintName("FK_Company_PlanRentas");

                    b.Navigation("CreatedByNavigation");

                    b.Navigation("LastUpdateByNavigation");

                    b.Navigation("Plan");
                });

            modelBuilder.Entity("UsaloYa.Library.Models.Customer", b =>
                {
                    b.HasOne("UsaloYa.Library.Models.Company", "Company")
                        .WithMany("Customers")
                        .HasForeignKey("CompanyId")
                        .IsRequired()
                        .HasConstraintName("FK_Customer_Company");

                    b.Navigation("Company");
                });

            modelBuilder.Entity("UsaloYa.Library.Models.Product", b =>
                {
                    b.HasOne("UsaloYa.Library.Models.ProductCategory", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("FK_Products_ProductCategory");

                    b.HasOne("UsaloYa.Library.Models.Company", "Company")
                        .WithMany("Products")
                        .HasForeignKey("CompanyId")
                        .IsRequired()
                        .HasConstraintName("FK_Products_Company");

                    b.Navigation("Category");

                    b.Navigation("Company");
                });

            modelBuilder.Entity("UsaloYa.Library.Models.ProductCategory", b =>
                {
                    b.HasOne("UsaloYa.Library.Models.Company", "Company")
                        .WithMany("ProductCategories")
                        .HasForeignKey("CompanyId")
                        .IsRequired()
                        .HasConstraintName("FK_ProductCategory_Company");

                    b.Navigation("Company");
                });

            modelBuilder.Entity("UsaloYa.Library.Models.Question", b =>
                {
                    b.HasOne("UsaloYa.Library.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("IdUser")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("User");
                });

            modelBuilder.Entity("UsaloYa.Library.Models.Renta", b =>
                {
                    b.HasOne("UsaloYa.Library.Models.User", "AddedByUser")
                        .WithMany("Renta")
                        .HasForeignKey("AddedByUserId")
                        .IsRequired()
                        .HasConstraintName("FK_Rentas_Users");

                    b.HasOne("UsaloYa.Library.Models.Company", "Company")
                        .WithMany("Renta")
                        .HasForeignKey("CompanyId")
                        .IsRequired()
                        .HasConstraintName("FK_Rentas_Company");

                    b.Navigation("AddedByUser");

                    b.Navigation("Company");
                });

            modelBuilder.Entity("UsaloYa.Library.Models.Sale", b =>
                {
                    b.HasOne("UsaloYa.Library.Models.Company", "Company")
                        .WithMany("Sales")
                        .HasForeignKey("CompanyId")
                        .IsRequired()
                        .HasConstraintName("FK_Sale_Company");

                    b.HasOne("UsaloYa.Library.Models.Customer", "Customer")
                        .WithMany("Sales")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .HasConstraintName("FK_Sales_Customers");

                    b.HasOne("UsaloYa.Library.Models.User", "User")
                        .WithMany("Sales")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("FK_Sales_Users");

                    b.Navigation("Company");

                    b.Navigation("Customer");

                    b.Navigation("User");
                });

            modelBuilder.Entity("UsaloYa.Library.Models.SaleDetail", b =>
                {
                    b.HasOne("UsaloYa.Library.Models.Product", "Product")
                        .WithMany("SaleDetails")
                        .HasForeignKey("ProductId")
                        .IsRequired()
                        .HasConstraintName("FK_SaleDetails_Products");

                    b.HasOne("UsaloYa.Library.Models.Sale", "Sale")
                        .WithMany("SaleDetails")
                        .HasForeignKey("SaleId")
                        .IsRequired()
                        .HasConstraintName("FK_Sale_SaleDetails");

                    b.Navigation("Product");

                    b.Navigation("Sale");
                });

            modelBuilder.Entity("UsaloYa.Library.Models.User", b =>
                {
                    b.HasOne("UsaloYa.Library.Models.Company", "Company")
                        .WithMany("Users")
                        .HasForeignKey("CompanyId")
                        .IsRequired()
                        .HasConstraintName("FK_Users_Company");

                    b.HasOne("UsaloYa.Library.Models.User", "CreatedByNavigation")
                        .WithMany("InverseCreatedByNavigation")
                        .HasForeignKey("CreatedBy");

                    b.HasOne("Group", "Group")
                        .WithMany("Users")
                        .HasForeignKey("GroupId")
                        .IsRequired()
                        .HasConstraintName("FK_Users_Groups");

                    b.HasOne("UsaloYa.Library.Models.User", "LastUpdateByNavigation")
                        .WithMany("InverseLastUpdateByNavigation")
                        .HasForeignKey("LastUpdateBy");

                    b.Navigation("Company");

                    b.Navigation("CreatedByNavigation");

                    b.Navigation("Group");

                    b.Navigation("LastUpdateByNavigation");
                });

            modelBuilder.Entity("Group", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("UsaloYa.Library.Models.Company", b =>
                {
                    b.Navigation("Customers");

                    b.Navigation("Groups");

                    b.Navigation("ProductCategories");

                    b.Navigation("Products");

                    b.Navigation("Renta");

                    b.Navigation("Sales");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("UsaloYa.Library.Models.Customer", b =>
                {
                    b.Navigation("Sales");
                });

            modelBuilder.Entity("UsaloYa.Library.Models.PlanRenta", b =>
                {
                    b.Navigation("Companies");
                });

            modelBuilder.Entity("UsaloYa.Library.Models.Product", b =>
                {
                    b.Navigation("SaleDetails");
                });

            modelBuilder.Entity("UsaloYa.Library.Models.ProductCategory", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("UsaloYa.Library.Models.Sale", b =>
                {
                    b.Navigation("SaleDetails");
                });

            modelBuilder.Entity("UsaloYa.Library.Models.User", b =>
                {
                    b.Navigation("CompanyCreatedByNavigations");

                    b.Navigation("CompanyLastUpdateByNavigations");

                    b.Navigation("InverseCreatedByNavigation");

                    b.Navigation("InverseLastUpdateByNavigation");

                    b.Navigation("Renta");

                    b.Navigation("Sales");
                });
#pragma warning restore 612, 618
        }
    }
}
