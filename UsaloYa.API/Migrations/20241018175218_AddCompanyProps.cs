using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsaloYa.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Company",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Company",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastUpdateBy",
                table: "Company",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentsJson",
                table: "Company",
                type: "xml",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Company_CreatedBy",
                table: "Company",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Company_LastUpdateBy",
                table: "Company",
                column: "LastUpdateBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Company_Users_CreatedBy",
                table: "Company",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Company_Users_LastUpdateBy",
                table: "Company",
                column: "LastUpdateBy",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Company_Users_CreatedBy",
                table: "Company");

            migrationBuilder.DropForeignKey(
                name: "FK_Company_Users_LastUpdateBy",
                table: "Company");

            migrationBuilder.DropIndex(
                name: "IX_Company_CreatedBy",
                table: "Company");

            migrationBuilder.DropIndex(
                name: "IX_Company_LastUpdateBy",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "LastUpdateBy",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "PaymentsJson",
                table: "Company");
        }
    }
}
