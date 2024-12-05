using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class passWord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Account_LoginProviderUserId_LoginProviderType",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "LoginProviderUserId",
                table: "Account");

            migrationBuilder.AddColumn<string>(
                name: "AccountName",
                table: "Account",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Account",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Account_AccountName_LoginProviderType",
                table: "Account",
                columns: new[] { "AccountName", "LoginProviderType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "AccountName",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Account");

            migrationBuilder.AddColumn<string>(
                name: "LoginProviderUserId",
                table: "Account",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Account_LoginProviderUserId_LoginProviderType",
                table: "Account",
                columns: new[] { "LoginProviderUserId", "LoginProviderType" },
                unique: true);
        }
    }
}
