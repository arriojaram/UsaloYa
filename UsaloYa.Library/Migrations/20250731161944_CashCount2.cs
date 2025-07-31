using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsaloYa.Library.Migrations
{
    /// <inheritdoc />
    public partial class CashCount2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CashOutput_CashCount",
                table: "CashOutput");

            migrationBuilder.RenameColumn(
                name: "CashOutput",
                table: "CashCount",
                newName: "CashOutputTotal");

            migrationBuilder.CreateIndex(
                name: "IX_CashOutput_CashCountId",
                table: "CashOutput",
                column: "CashCountId");

            migrationBuilder.CreateIndex(
                name: "IX_CashCount_UserId",
                table: "CashCount",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_CashCount",
                table: "CashCount",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CashCount_CashOutput",
                table: "CashOutput",
                column: "CashCountId",
                principalTable: "CashCount",
                principalColumn: "CashCountId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_CashOutput",
                table: "CashOutput",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_CashCount",
                table: "CashCount");

            migrationBuilder.DropForeignKey(
                name: "FK_CashCount_CashOutput",
                table: "CashOutput");

            migrationBuilder.DropForeignKey(
                name: "FK_User_CashOutput",
                table: "CashOutput");

            migrationBuilder.DropIndex(
                name: "IX_CashOutput_CashCountId",
                table: "CashOutput");

            migrationBuilder.DropIndex(
                name: "IX_CashCount_UserId",
                table: "CashCount");

            migrationBuilder.RenameColumn(
                name: "CashOutputTotal",
                table: "CashCount",
                newName: "CashOutput");

            migrationBuilder.AddForeignKey(
                name: "FK_CashOutput_CashCount",
                table: "CashOutput",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
