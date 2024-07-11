using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameServer.Migrations
{
    /// <inheritdoc />
    public partial class AddPosInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurMap",
                table: "Player",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "PosX",
                table: "Player",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "PosY",
                table: "Player",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "PosZ",
                table: "Player",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurMap",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "PosX",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "PosY",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "PosZ",
                table: "Player");
        }
    }
}
