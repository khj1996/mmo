using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameServer.Migrations
{
    /// <inheritdoc />
    public partial class changeForeingKeyName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RewardData_Monster_OwnerDbId",
                table: "RewardData");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopProduct_Shop_ShopDbId",
                table: "ShopProduct");

            migrationBuilder.DropColumn(
                name: "AccountDbId",
                table: "Player");

            migrationBuilder.AddForeignKey(
                name: "FK_RewardData_Monster_OwnerDbId",
                table: "RewardData",
                column: "OwnerDbId",
                principalTable: "Monster",
                principalColumn: "MonsterDataDbid");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopProduct_Shop_ShopDbId",
                table: "ShopProduct",
                column: "ShopDbId",
                principalTable: "Shop",
                principalColumn: "ShopDbId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RewardData_Monster_OwnerDbId",
                table: "RewardData");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopProduct_Shop_ShopDbId",
                table: "ShopProduct");

            migrationBuilder.AddColumn<int>(
                name: "AccountDbId",
                table: "Player",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_RewardData_Monster_OwnerDbId",
                table: "RewardData",
                column: "OwnerDbId",
                principalTable: "Monster",
                principalColumn: "MonsterDataDbid",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShopProduct_Shop_ShopDbId",
                table: "ShopProduct",
                column: "ShopDbId",
                principalTable: "Shop",
                principalColumn: "ShopDbId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
