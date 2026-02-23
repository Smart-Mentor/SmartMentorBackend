using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMentor.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCareerGoalRequiredSkills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CareerGoalRequiredSkills",
                columns: table => new
                {
                    CareerGoalId = table.Column<int>(type: "int", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false),
                    RequiredLevel = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CareerGoalRequiredSkills", x => new { x.CareerGoalId, x.SkillId });
                    table.ForeignKey(
                        name: "FK_CareerGoalRequiredSkills_CareerGoals_CareerGoalId",
                        column: x => x.CareerGoalId,
                        principalTable: "CareerGoals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CareerGoalRequiredSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CareerGoalRequiredSkills_SkillId",
                table: "CareerGoalRequiredSkills",
                column: "SkillId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CareerGoalRequiredSkills");
        }
    }
}
