using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameServer.Migrations
{
    /// <inheritdoc />
    public partial class AddShop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Shop",
                columns: table => new
                {
                    ShopDbId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shop", x => x.ShopDbId);
                });

            migrationBuilder.CreateTable(
                name: "ShopProduct",
                columns: table => new
                {
                    ShopProductDbId = table.Column<int>(type: "int", nullable: false),
                    PId = table.Column<int>(type: "int", nullable: false),
                    CType = table.Column<int>(type: "int", nullable: false),
                    CAmount = table.Column<int>(type: "int", nullable: false),
                    ShopDbId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopProduct", x => x.ShopProductDbId);
                    table.ForeignKey(
                        name: "FK_ShopProduct_Shop_ShopDbId",
                        column: x => x.ShopDbId,
                        principalTable: "Shop",
                        principalColumn: "ShopDbId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shop_ShopDbId",
                table: "Shop",
                column: "ShopDbId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShopProduct_ShopDbId",
                table: "ShopProduct",
                column: "ShopDbId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopProduct_ShopProductDbId",
                table: "ShopProduct",
                column: "ShopProductDbId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShopProduct");

            migrationBuilder.DropTable(
                name: "Shop");
        }
    }
}
