using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameServer.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemData",
                columns: table => new
                {
                    ItemDataDbId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemTemplateId = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    maxCount = table.Column<int>(type: "int", nullable: false),
                    value = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemData", x => x.ItemDataDbId);
                });

            migrationBuilder.CreateTable(
                name: "Monster",
                columns: table => new
                {
                    MonsterDbId = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    MaxHp = table.Column<int>(type: "int", nullable: false),
                    Attack = table.Column<int>(type: "int", nullable: false),
                    Speed = table.Column<float>(type: "real", nullable: false),
                    TotalExp = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monster", x => x.MonsterDbId);
                });

            migrationBuilder.CreateTable(
                name: "MonsterReward",
                columns: table => new
                {
                    MonsterRewardDbId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    probability = table.Column<int>(type: "int", nullable: false),
                    itemId = table.Column<int>(type: "int", nullable: false),
                    count = table.Column<int>(type: "int", nullable: false),
                    MonsterDbId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonsterReward", x => x.MonsterRewardDbId);
                    table.ForeignKey(
                        name: "FK_MonsterReward_Monster_MonsterDbId",
                        column: x => x.MonsterDbId,
                        principalTable: "Monster",
                        principalColumn: "MonsterDbId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemData_ItemDataDbId",
                table: "ItemData",
                column: "ItemDataDbId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Monster_MonsterDbId",
                table: "Monster",
                column: "MonsterDbId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MonsterReward_MonsterDbId",
                table: "MonsterReward",
                column: "MonsterDbId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterReward_MonsterRewardDbId",
                table: "MonsterReward",
                column: "MonsterRewardDbId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemData");

            migrationBuilder.DropTable(
                name: "MonsterReward");

            migrationBuilder.DropTable(
                name: "Monster");
        }
    }
}
