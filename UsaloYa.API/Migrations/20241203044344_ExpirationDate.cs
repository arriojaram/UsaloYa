using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsaloYa.API.Migrations
{
    /// <inheritdoc />
    public partial class ExpirationDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CompanyStatus",
                table: "Company",
                newName: "StatusId");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "Company",
                type: "datetime",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "Company");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "Company",
                newName: "CompanyStatus");
        }
    }
}
