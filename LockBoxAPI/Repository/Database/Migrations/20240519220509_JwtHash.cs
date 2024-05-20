using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LockBoxAPI.Migrations
{
    /// <inheritdoc />
    public partial class JwtHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegisteredAccounts_AspNetUsers_UserId",
                table: "RegisteredAccounts");

            migrationBuilder.DropIndex(
                name: "IX_RegisteredAccounts_UserId",
                table: "RegisteredAccounts");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "RegisteredAccounts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "JwtHash",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JwtHash",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "RegisteredAccounts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_RegisteredAccounts_UserId",
                table: "RegisteredAccounts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RegisteredAccounts_AspNetUsers_UserId",
                table: "RegisteredAccounts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
