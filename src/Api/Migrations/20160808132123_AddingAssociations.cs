using Microsoft.EntityFrameworkCore.Migrations;

namespace Api.Migrations
{
    public partial class AddingAssociations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TopicUsers_TopicId",
                table: "TopicUsers",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_TopicUsers_UserId",
                table: "TopicUsers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TopicUsers_Topics_TopicId",
                table: "TopicUsers",
                column: "TopicId",
                principalTable: "Topics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TopicUsers_Users_UserId",
                table: "TopicUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TopicUsers_Topics_TopicId",
                table: "TopicUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_TopicUsers_Users_UserId",
                table: "TopicUsers");

            migrationBuilder.DropIndex(
                name: "IX_TopicUsers_TopicId",
                table: "TopicUsers");

            migrationBuilder.DropIndex(
                name: "IX_TopicUsers_UserId",
                table: "TopicUsers");
        }
    }
}
