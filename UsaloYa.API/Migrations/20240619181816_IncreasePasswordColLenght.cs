using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsaloYa.API.Migrations
{
    /// <inheritdoc />
    public partial class IncreasePasswordColLenght : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                oldType: "char(50)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 50);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Users",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "Users",
                type: "char(50)",
                unicode: false,
                fixedLength: true,
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(500)",
                oldUnicode: false,
                oldFixedLength: true,
                oldMaxLength: 500);
        }
    }
}
