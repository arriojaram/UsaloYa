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
            migrationBuilder.AlterColumn<int>(
                name: "Folio",
                table: "Sales",
                type: "int",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(11)",
                oldUnicode: false,
                oldMaxLength: 11,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "SaleDetails",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitsInStock",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "InVentario",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true,
                comment: "Valor utilizado para guardar informacion temporal del inventario del producto",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Valor utilizado para guardar informacion temporal del inventario del producto");

            migrationBuilder.AddColumn<int>(
                name: "Measure",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Measure",
                table: "Products");

            migrationBuilder.AlterColumn<string>(
                name: "Folio",
                table: "Sales",
                type: "varchar(11)",
                unicode: false,
                maxLength: 11,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldUnicode: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "SaleDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "UnitsInStock",
                table: "Products",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "InVentario",
                table: "Products",
                type: "int",
                nullable: true,
                comment: "Valor utilizado para guardar informacion temporal del inventario del producto",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true,
                oldComment: "Valor utilizado para guardar informacion temporal del inventario del producto");
        }
    }
}
