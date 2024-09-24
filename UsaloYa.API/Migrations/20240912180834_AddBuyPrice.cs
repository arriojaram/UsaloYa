using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsaloYa.API.Migrations
{
    /// <inheritdoc />
    public partial class AddBuyPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_Barcode_SKU",
                table: "Products");

            migrationBuilder.AddColumn<decimal>(
                name: "BuyPrice",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Barcode_SKU",
                table: "Products",
                columns: new[] { "CompanyId", "Barcode", "SKU" },
                unique: true,
                filter: "([Barcode] IS NOT NULL AND [SKU] IS NOT NULL)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_Barcode_SKU",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "BuyPrice",
                table: "Products");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Barcode_SKU",
                table: "Products",
                columns: new[] { "Barcode", "SKU" },
                unique: true,
                filter: "([Barcode] IS NOT NULL AND [SKU] IS NOT NULL)");
        }
    }
}
