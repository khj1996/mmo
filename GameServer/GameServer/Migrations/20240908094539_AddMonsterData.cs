using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameServer.Migrations
{
    /// <inheritdoc />
    public partial class AddMonsterData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Monster",
                columns: table => new
                {
                    MonsterDataDbid = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    MaxHp = table.Column<int>(type: "int", nullable: false),
                    Attack = table.Column<int>(type: "int", nullable: false),
                    Speed = table.Column<float>(type: "real", nullable: false),
                    TotalExp = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monster", x => x.MonsterDataDbid);
                });

            migrationBuilder.CreateTable(
                name: "RewardDatas",
                columns: table => new
                {
                    RewardDataDbid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    probability = table.Column<int>(type: "int", nullable: false),
                    itemId = table.Column<int>(type: "int", nullable: false),
                    count = table.Column<int>(type: "int", nullable: false),
                    OwnerDbId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardDatas", x => x.RewardDataDbid);
                    table.ForeignKey(
                        name: "FK_RewardDatas_Monster_OwnerDbId",
                        column: x => x.OwnerDbId,
                        principalTable: "Monster",
                        principalColumn: "MonsterDataDbid");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Monster_MonsterDataDbid",
                table: "Monster",
                column: "MonsterDataDbid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RewardDatas_OwnerDbId",
                table: "RewardDatas",
                column: "OwnerDbId");

            migrationBuilder.CreateIndex(
                name: "IX_RewardDatas_RewardDataDbid",
                table: "RewardDatas",
                column: "RewardDataDbid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RewardDatas");

            migrationBuilder.DropTable(
                name: "Monster");
        }
    }
}
