using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 

namespace HRCandidateManagement.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candidates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FullName = table.Column<string>(type: "TEXT", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    ContactNumber = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CandidateSkill",
                columns: table => new
                {
                    CandidatesId = table.Column<int>(type: "INTEGER", nullable: false),
                    SkillsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateSkill", x => new { x.CandidatesId, x.SkillsId });
                    table.ForeignKey(
                        name: "FK_CandidateSkill_Candidates_CandidatesId",
                        column: x => x.CandidatesId,
                        principalTable: "Candidates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CandidateSkill_Skills_SkillsId",
                        column: x => x.SkillsId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Candidates",
                columns: new[] { "Id", "ContactNumber", "DateOfBirth", "Email", "FullName" },
                values: new object[,]
                {
                    { 1, "066-1234-567", new DateOnly(2002, 3, 14), "markomarkovic@gmail.com", "Marko Markovic" },
                    { 2, "066-2345-678", new DateOnly(2003, 7, 22), "zarkozarkovic@gmail.com", "Žarko Žarkovic" },
                    { 3, "066-3456-789", new DateOnly(2002, 11, 5), "nikolanikolic@gmail.com", "Nikola Nikolic" },
                    { 4, "066-4567-890", new DateOnly(2003, 1, 18), "jovanajovanovic@gmail.com", "Jovana Jovanovic" },
                    { 5, "066-5678-901", new DateOnly(2002, 9, 30), "stefanstefanovic@gmail.com", "Stefan Stefanovic" }
                });

            migrationBuilder.InsertData(
                table: "Skills",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Java" },
                    { 2, "C#" },
                    { 3, "Database Design" },
                    { 4, "English" },
                    { 5, "Russian" },
                    { 6, "German" },
                    { 7, "Python" },
                    { 8, "SQL" }
                });

            migrationBuilder.InsertData(
                table: "CandidateSkill",
                columns: new[] { "CandidatesId", "SkillsId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 1, 3 },
                    { 1, 4 },
                    { 2, 2 },
                    { 2, 3 },
                    { 2, 5 },
                    { 3, 1 },
                    { 3, 4 },
                    { 3, 6 },
                    { 4, 7 },
                    { 4, 8 },
                    { 5, 2 },
                    { 5, 7 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_Email",
                table: "Candidates",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CandidateSkill_SkillsId",
                table: "CandidateSkill",
                column: "SkillsId");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_Name",
                table: "Skills",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CandidateSkill");

            migrationBuilder.DropTable(
                name: "Candidates");

            migrationBuilder.DropTable(
                name: "Skills");
        }
    }
}
