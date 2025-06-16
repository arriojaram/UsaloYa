using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsaloYa.Library.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlanRentas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(10,0)", nullable: false, defaultValueSql: "((1))"),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    NumUsers = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanRentas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    LastUpdateBy = table.Column<int>(type: "int", nullable: true),
                    PaymentsJson = table.Column<string>(type: "xml", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    PhoneNumber = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    CelphoneNumber = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    Email = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: true),
                    OwnerInfo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PlanId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.CompanyId);
                    table.ForeignKey(
                        name: "FK_Company_PlanRentas",
                        column: x => x.PlanId,
                        principalTable: "PlanRentas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "varchar(300)", unicode: false, maxLength: 300, nullable: true),
                    Notes = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    FirstName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    LastName1 = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    LastName2 = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    WorkPhoneNumber = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    CellPhoneNumber = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "varchar(35)", unicode: false, maxLength: 35, nullable: true),
                    FullName = table.Column<string>(type: "varchar(152)", unicode: false, maxLength: 152, nullable: false, computedColumnSql: "(concat([FirstName],' ',[LastName1],coalesce(' '+[LastName2],'')))", stored: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer_Id", x => x.CustomerId);
                    table.ForeignKey(
                        name: "FK_Customer_Company",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    Permissions = table.Column<string>(type: "xml", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.GroupId);
                    table.ForeignKey(
                        name: "FK_Groups_Company",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                });

            migrationBuilder.CreateTable(
                name: "ProductCategory",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategory", x => x.CategoryId);
                    table.ForeignKey(
                        name: "FK_ProductCategory_Company",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Token = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    FirstName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    LastAccess = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "(CONVERT([bit],(0)))"),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    LastUpdateBy = table.Column<int>(type: "int", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: true),
                    DeviceId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SessionToken = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CodeVerification = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    IsVerifiedCode = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "(CONVERT([bit],(0)))"),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Id", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Company",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_Users_Groups",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "GroupId");
                    table.ForeignKey(
                        name: "FK_Users_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Users_Users_LastUpdateBy",
                        column: x => x.LastUpdateBy,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    SupplierId = table.Column<int>(type: "int", nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    UnitsInStock = table.Column<int>(type: "int", nullable: false),
                    Discontinued = table.Column<bool>(type: "bit", nullable: false),
                    ImgUrl = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    DateAdded = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    DateModified = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Weight = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    SKU = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Barcode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Brand = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Color = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Size = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    BuyPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    UnitPrice1 = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    UnitPrice2 = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    UnitPrice3 = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    InVentario = table.Column<int>(type: "int", nullable: true, comment: "Valor utilizado para guardar informacion temporal del inventario del producto"),
                    AlertaStockNumProducts = table.Column<int>(type: "int", nullable: true),
                    IsInVentarioUpdated = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Products_Company",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_Products_ProductCategory",
                        column: x => x.CategoryId,
                        principalTable: "ProductCategory",
                        principalColumn: "CategoryId");
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Reply = table.Column<bool>(type: "bit", nullable: false),
                    IdUser = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.QuestionId);
                    table.ForeignKey(
                        name: "FK_Questions_Users_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Rentas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    ReferenceDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    AddedByUserId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    TipoRentaDesc = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    Notas = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rentas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rentas_Company",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_Rentas_Users",
                        column: x => x.AddedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Sales",
                columns: table => new
                {
                    SaleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    SaleDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    PaymentMethod = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Tax = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Status = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValueSql: "('Completada')"),
                    TotalSale = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Notes = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Folio = table.Column<string>(type: "varchar(11)", unicode: false, maxLength: 11, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales", x => x.SaleId);
                    table.ForeignKey(
                        name: "FK_Sale_Company",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_Sales_Customers",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Sales_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "SaleDetails",
                columns: table => new
                {
                    SaleId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    BuyPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PriceLevel = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleDetails", x => new { x.SaleId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_SaleDetails_Products",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_Sale_SaleDetails",
                        column: x => x.SaleId,
                        principalTable: "Sales",
                        principalColumn: "SaleId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Company_CreatedBy",
                table: "Company",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Company_LastUpdateBy",
                table: "Company",
                column: "LastUpdateBy");

            migrationBuilder.CreateIndex(
                name: "IX_Company_PlanId",
                table: "Company",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_CompanyId",
                table: "Customers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Email",
                table: "Customers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Name",
                table: "Customers",
                columns: new[] { "FirstName", "LastName1", "LastName2" });

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Phone",
                table: "Customers",
                columns: new[] { "CellPhoneNumber", "WorkPhoneNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_Groups_CompanyId",
                table: "Groups",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategory_CompanyId",
                table: "ProductCategory",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Products",
                table: "Products",
                columns: new[] { "Name", "Description" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_Barcode_SKU",
                table: "Products",
                columns: new[] { "CompanyId", "Barcode", "SKU" },
                unique: true,
                filter: "([Barcode] IS NOT NULL AND [SKU] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CompanyId",
                table: "Products",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_IdUser",
                table: "Questions",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_Rentas_AddedByUserId",
                table: "Rentas",
                column: "AddedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Rentas_CompanyId",
                table: "Rentas",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleDetails_ProductId",
                table: "SaleDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_CompanyId",
                table: "Sales",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_CustomerId",
                table: "Sales",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_UserId",
                table: "Sales",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CompanyId",
                table: "Users",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedBy",
                table: "Users",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Users_GroupId",
                table: "Users",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_LastUpdateBy",
                table: "Users",
                column: "LastUpdateBy");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Company_Users_CreatedBy",
                table: "Company",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Company_Users_LastUpdateBy",
                table: "Company",
                column: "LastUpdateBy",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Company_PlanRentas",
                table: "Company");

            migrationBuilder.DropForeignKey(
                name: "FK_Company_Users_CreatedBy",
                table: "Company");

            migrationBuilder.DropForeignKey(
                name: "FK_Company_Users_LastUpdateBy",
                table: "Company");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Rentas");

            migrationBuilder.DropTable(
                name: "SaleDetails");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Sales");

            migrationBuilder.DropTable(
                name: "ProductCategory");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "PlanRentas");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Company");
        }
    }
}
