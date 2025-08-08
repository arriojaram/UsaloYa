using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsaloYa.Library.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMaxDaysToRefundColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxDaysToRefund",
                table: "Company");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxDaysToRefund",
                table: "Company",
                type: "int",
                nullable: true);
        }
    }
}
