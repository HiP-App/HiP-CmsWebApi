using Microsoft.EntityFrameworkCore.Migrations;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Migrations
{
    public partial class RemoveStudentDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentDetails",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    CurrentDegree = table.Column<string>(nullable: true),
                    CurrentSemester = table.Column<short>(nullable: false),
                    Discipline = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentDetails", x => x.UserId);
                });
        }
    }
}
