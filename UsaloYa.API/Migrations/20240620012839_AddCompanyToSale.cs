using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsaloYa.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyToSale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Sales",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Sales_CompanyId",
                table: "Sales",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sale_Company",
                table: "Sales",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sale_Company",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_CompanyId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Sales");
        }
    }
}
