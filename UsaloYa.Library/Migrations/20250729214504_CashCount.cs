using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsaloYa.Library.Migrations
{
    /// <inheritdoc />
    public partial class CashCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Users_IdUser",
                table: "Questions");

            migrationBuilder.AlterColumn<int>(
                name: "Folio",
                table: "Sales",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(11)",
                oldUnicode: false,
                oldMaxLength: 11,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "SaleDetails",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitsInStock",
                table: "Products",
                type: "decimal(10,2)",
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
                defaultValueSql: "((1))");

            migrationBuilder.CreateTable(
                name: "CashCount",
                columns: table => new
                {
                    CashCountId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    ReferenceDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    InitialBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Cash = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CredictCard = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Spei = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CashOutput = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Notes = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    FinalCash = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashCount", x => x.CashCountId);
                });

            migrationBuilder.CreateTable(
                name: "CashOutput",
                columns: table => new
                {
                    OutputId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Reason = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    ReferenceDate = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IsCashCount = table.Column<bool>(type: "bit", nullable: false),
                    CashCountId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashOutput", x => x.OutputId);
                    table.ForeignKey(
                        name: "FK_CashOutput_CashCount",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CashOutput_UserId",
                table: "CashOutput",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Users_IdUser",
                table: "Questions",
                column: "IdUser",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Users_IdUser",
                table: "Questions");

            migrationBuilder.DropTable(
                name: "CashCount");

            migrationBuilder.DropTable(
                name: "CashOutput");

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
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "SaleDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<int>(
                name: "UnitsInStock",
                table: "Products",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Users_IdUser",
                table: "Questions",
                column: "IdUser",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
