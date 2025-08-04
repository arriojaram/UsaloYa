using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsaloYa.Library.Migrations
{
    /// <inheritdoc />
    public partial class RefundTableAddCanRefundedAndMaxDaysToRefund : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanRefunded",
                table: "Products",
                type: "bit",
                nullable: true,
                defaultValueSql: "(CONVERT([bit],(0)))");

            migrationBuilder.AddColumn<int>(
                name: "MaxDaysToRefund",
                table: "Company",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Refunds",
                columns: table => new
                {
                    SaleId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RefundDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    RefundMethod = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Reason = table.Column<string>(type: "text", unicode: false, nullable: false),
                    Measure = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    UnitPriceRefund = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    RefundAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Refunds", x => x.SaleId);
                    table.ForeignKey(
                        name: "FK_Refunds_Sales",
                        column: x => x.SaleId,
                        principalTable: "Sales",
                        principalColumn: "SaleId");
                    table.ForeignKey(
                        name: "FK_Refunds_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_UserId",
                table: "Refunds",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Refunds");

            migrationBuilder.DropColumn(
                name: "CanRefunded",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MaxDaysToRefund",
                table: "Company");
        }
    }
}
