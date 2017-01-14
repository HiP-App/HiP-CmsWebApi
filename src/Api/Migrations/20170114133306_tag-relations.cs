using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Api.Migrations
{
    public partial class tagrelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TagRelations_AnnotationTags_FirstTagId",
                table: "TagRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_TagRelations_AnnotationTags_SecondTagId",
                table: "TagRelations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TagRelations",
                table: "TagRelations");

            migrationBuilder.DropIndex(
                name: "IX_TagRelations_FirstTagId",
                table: "TagRelations");

            migrationBuilder.DropColumn(
                name: "Relationid",
                table: "TagRelations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TagRelations",
                table: "TagRelations",
                columns: new[] { "FirstTagId", "SecondTagId" });

            migrationBuilder.CreateTable(
                name: "AnnotationTagInstances",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    TagModelId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnotationTagInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnnotationTagInstances_AnnotationTags_TagModelId",
                        column: x => x.TagModelId,
                        principalTable: "AnnotationTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnnotationTagInstances_TagModelId",
                table: "AnnotationTagInstances",
                column: "TagModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_TagRelations_AnnotationTagInstances_FirstTagId",
                table: "TagRelations",
                column: "FirstTagId",
                principalTable: "AnnotationTagInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagRelations_AnnotationTagInstances_SecondTagId",
                table: "TagRelations",
                column: "SecondTagId",
                principalTable: "AnnotationTagInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TagRelations_AnnotationTagInstances_FirstTagId",
                table: "TagRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_TagRelations_AnnotationTagInstances_SecondTagId",
                table: "TagRelations");

            migrationBuilder.DropTable(
                name: "AnnotationTagInstances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TagRelations",
                table: "TagRelations");

            migrationBuilder.AddColumn<int>(
                name: "Relationid",
                table: "TagRelations",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TagRelations",
                table: "TagRelations",
                column: "Relationid");

            migrationBuilder.CreateIndex(
                name: "IX_TagRelations_FirstTagId",
                table: "TagRelations",
                column: "FirstTagId");

            migrationBuilder.AddForeignKey(
                name: "FK_TagRelations_AnnotationTags_FirstTagId",
                table: "TagRelations",
                column: "FirstTagId",
                principalTable: "AnnotationTags",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TagRelations_AnnotationTags_SecondTagId",
                table: "TagRelations",
                column: "SecondTagId",
                principalTable: "AnnotationTags",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
