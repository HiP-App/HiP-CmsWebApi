using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Migrations
{
    public partial class TagRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnnotationTagRelations_AnnotationTags_FirstTagId",
                table: "AnnotationTagRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_AnnotationTagRelations_AnnotationTags_SecondTagId",
                table: "AnnotationTagRelations");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AnnotationTagRelations");

            migrationBuilder.RenameColumn(
                name: "SecondTagId",
                table: "AnnotationTagRelations",
                newName: "TargetTagId");

            migrationBuilder.RenameColumn(
                name: "FirstTagId",
                table: "AnnotationTagRelations",
                newName: "SourceTagId");

            migrationBuilder.RenameIndex(
                name: "IX_AnnotationTagRelations_SecondTagId",
                table: "AnnotationTagRelations",
                newName: "IX_AnnotationTagRelations_TargetTagId");

            migrationBuilder.CreateTable(
                name: "TagRelationRules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ArrowStyle = table.Column<string>(nullable: true),
                    Color = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    SourceTagId = table.Column<int>(nullable: false),
                    TargetTagId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagRelationRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TagRelationRules_AnnotationTags_SourceTagId",
                        column: x => x.SourceTagId,
                        principalTable: "AnnotationTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagRelationRules_AnnotationTags_TargetTagId",
                        column: x => x.TargetTagId,
                        principalTable: "AnnotationTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TagRelationRules_SourceTagId",
                table: "TagRelationRules",
                column: "SourceTagId");

            migrationBuilder.CreateIndex(
                name: "IX_TagRelationRules_TargetTagId",
                table: "TagRelationRules",
                column: "TargetTagId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnnotationTagRelations_AnnotationTagInstances_SourceTagId",
                table: "AnnotationTagRelations",
                column: "SourceTagId",
                principalTable: "AnnotationTagInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnnotationTagRelations_AnnotationTagInstances_TargetTagId",
                table: "AnnotationTagRelations",
                column: "TargetTagId",
                principalTable: "AnnotationTagInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnnotationTagRelations_AnnotationTagInstances_SourceTagId",
                table: "AnnotationTagRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_AnnotationTagRelations_AnnotationTagInstances_TargetTagId",
                table: "AnnotationTagRelations");

            migrationBuilder.DropTable(
                name: "TagRelationRules");

            migrationBuilder.RenameColumn(
                name: "TargetTagId",
                table: "AnnotationTagRelations",
                newName: "SecondTagId");

            migrationBuilder.RenameColumn(
                name: "SourceTagId",
                table: "AnnotationTagRelations",
                newName: "FirstTagId");

            migrationBuilder.RenameIndex(
                name: "IX_AnnotationTagRelations_TargetTagId",
                table: "AnnotationTagRelations",
                newName: "IX_AnnotationTagRelations_SecondTagId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AnnotationTagRelations",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AnnotationTagRelations_AnnotationTags_FirstTagId",
                table: "AnnotationTagRelations",
                column: "FirstTagId",
                principalTable: "AnnotationTags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnnotationTagRelations_AnnotationTags_SecondTagId",
                table: "AnnotationTagRelations",
                column: "SecondTagId",
                principalTable: "AnnotationTags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
