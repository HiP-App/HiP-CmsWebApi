using Microsoft.EntityFrameworkCore.Migrations;

namespace Api.Migrations
{
    public partial class TopicReview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "LayerRelationRules",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "LayerRelationRules",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AnnotationTagRelations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "AnnotationTagRelations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "LayerRelationRules");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "LayerRelationRules");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "AnnotationTagRelations");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "AnnotationTagRelations");
        }
    }
}
