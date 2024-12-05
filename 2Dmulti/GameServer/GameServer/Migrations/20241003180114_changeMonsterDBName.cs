using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameServer.Migrations
{
    /// <inheritdoc />
    public partial class changeMonsterDBName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RewardData");

            migrationBuilder.RenameColumn(
                name: "MonsterDataDbid",
                table: "Monster",
                newName: "MonsterDbid");

            migrationBuilder.RenameIndex(
                name: "IX_Monster_MonsterDataDbid",
                table: "Monster",
                newName: "IX_Monster_MonsterDbid");

            migrationBuilder.CreateTable(
                name: "MonsterReward",
                columns: table => new
                {
                    MonsterRewardDbid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    probability = table.Column<int>(type: "int", nullable: false),
                    itemId = table.Column<int>(type: "int", nullable: false),
                    count = table.Column<int>(type: "int", nullable: false),
                    MonsterDbId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonsterReward", x => x.MonsterRewardDbid);
                    table.ForeignKey(
                        name: "FK_MonsterReward_Monster_MonsterDbId",
                        column: x => x.MonsterDbId,
                        principalTable: "Monster",
                        principalColumn: "MonsterDbid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MonsterReward_MonsterDbId",
                table: "MonsterReward",
                column: "MonsterDbId");

            migrationBuilder.CreateIndex(
                name: "IX_MonsterReward_MonsterRewardDbid",
                table: "MonsterReward",
                column: "MonsterRewardDbid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonsterReward");

            migrationBuilder.RenameColumn(
                name: "MonsterDbid",
                table: "Monster",
                newName: "MonsterDataDbid");

            migrationBuilder.RenameIndex(
                name: "IX_Monster_MonsterDbid",
                table: "Monster",
                newName: "IX_Monster_MonsterDataDbid");

            migrationBuilder.CreateTable(
                name: "RewardData",
                columns: table => new
                {
                    RewardDataDbid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerDbId = table.Column<int>(type: "int", nullable: true),
                    count = table.Column<int>(type: "int", nullable: false),
                    itemId = table.Column<int>(type: "int", nullable: false),
                    probability = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardData", x => x.RewardDataDbid);
                    table.ForeignKey(
                        name: "FK_RewardData_Monster_OwnerDbId",
                        column: x => x.OwnerDbId,
                        principalTable: "Monster",
                        principalColumn: "MonsterDataDbid");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RewardData_OwnerDbId",
                table: "RewardData",
                column: "OwnerDbId");

            migrationBuilder.CreateIndex(
                name: "IX_RewardData_RewardDataDbid",
                table: "RewardData",
                column: "RewardDataDbid",
                unique: true);
        }
    }
}
