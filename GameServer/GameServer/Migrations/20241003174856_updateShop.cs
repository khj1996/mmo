using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameServer.Migrations
{
    /// <inheritdoc />
    public partial class updateShop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopProduct_Shop_ShopDbId",
                table: "ShopProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShopProduct",
                table: "ShopProduct");

            migrationBuilder.DropIndex(
                name: "IX_ShopProduct_ShopProductDbId",
                table: "ShopProduct");

            migrationBuilder.AlterColumn<int>(
                name: "ShopDbId",
                table: "ShopProduct",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ShopProduct");
            
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "ShopProduct",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShopProduct",
                table: "ShopProduct",
                columns: new[] { "ShopProductDbId", "ShopDbId" });

            migrationBuilder.CreateIndex(
                name: "IX_ShopProduct_ShopProductDbId_ShopDbId",
                table: "ShopProduct",
                columns: new[] { "ShopProductDbId", "ShopDbId" },
                unique: true);

            migrationBuilder.DropIndex(
                name: "IX_Item_ItemDbId_OwnerDbId",
                table: "Item");
            
            migrationBuilder.CreateIndex(
                name: "IX_Item_ItemDbId_OwnerDbId",
                table: "Item",
                columns: new[] { "ItemDbId", "OwnerDbId" },
                unique: true,
                filter: "[OwnerDbId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopProduct_Shop_ShopDbId",
                table: "ShopProduct",
                column: "ShopDbId",
                principalTable: "Shop",
                principalColumn: "ShopDbId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopProduct_Shop_ShopDbId",
                table: "ShopProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShopProduct",
                table: "ShopProduct");

            migrationBuilder.DropIndex(
                name: "IX_ShopProduct_ShopProductDbId_ShopDbId",
                table: "ShopProduct");

            migrationBuilder.DropIndex(
                name: "IX_Item_ItemDbId_OwnerDbId",
                table: "Item");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ShopProduct");

            migrationBuilder.AlterColumn<int>(
                name: "ShopDbId",
                table: "ShopProduct",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShopProduct",
                table: "ShopProduct",
                column: "ShopProductDbId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopProduct_ShopProductDbId",
                table: "ShopProduct",
                column: "ShopProductDbId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ShopProduct_Shop_ShopDbId",
                table: "ShopProduct",
                column: "ShopDbId",
                principalTable: "Shop",
                principalColumn: "ShopDbId");
        }
    }
}
