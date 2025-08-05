using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsaloYa.Library.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFinalCashColumnType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "FinalCash",
                table: "CashCount",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nchar(10)",
                oldFixedLength: true,
                oldMaxLength: 10,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FinalCash",
                table: "CashCount",
                type: "nchar(10)",
                fixedLength: true,
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }
    }
}
