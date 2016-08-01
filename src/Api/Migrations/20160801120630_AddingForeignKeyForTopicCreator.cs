using Microsoft.EntityFrameworkCore.Migrations;

namespace Api.Migrations
{
    public partial class AddingForeignKeyForTopicCreator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Topics_CreatedById",
                table: "Topics",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_Users_CreatedById",
                table: "Topics",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Users_CreatedById",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Topics_CreatedById",
                table: "Topics");
        }
    }
}
