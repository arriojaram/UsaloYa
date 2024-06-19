using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsaloYa.API.Migrations
{
    /// <inheritdoc />
    public partial class ReduceTokenLenght : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "Users",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(500)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 500);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "Users",
                type: "char(500)",
                unicode: false,
                fixedLength: true,
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100);
        }
    }
}
