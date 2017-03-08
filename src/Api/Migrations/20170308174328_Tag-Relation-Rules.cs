using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Api.Migrations
{
    public partial class TagRelationRules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnnotationTagRelations_AnnotationTags_FirstTagId",
                table: "AnnotationTagRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_AnnotationTagRelations_AnnotationTags_SecondTagId",
                table: "AnnotationTagRelations");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "AnnotationTagRelations",
                newName: "Title");

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
                name: "FK_AnnotationTagRelations_AnnotationTagInstances_FirstTagId",
                table: "AnnotationTagRelations",
                column: "FirstTagId",
                principalTable: "AnnotationTagInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnnotationTagRelations_AnnotationTagInstances_SecondTagId",
                table: "AnnotationTagRelations",
                column: "SecondTagId",
                principalTable: "AnnotationTagInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnnotationTagRelations_AnnotationTagInstances_FirstTagId",
                table: "AnnotationTagRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_AnnotationTagRelations_AnnotationTagInstances_SecondTagId",
                table: "AnnotationTagRelations");

            migrationBuilder.DropTable(
                name: "TagRelationRules");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "LayerRelationRules");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "LayerRelationRules");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "AnnotationTagRelations");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "AnnotationTagRelations",
                newName: "Name");

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
