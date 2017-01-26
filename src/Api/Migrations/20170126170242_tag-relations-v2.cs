using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Api.Migrations
{
    public partial class tagrelationsv2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "TagRelations",
                columns: table => new
                {
                    FirstTagId = table.Column<int>(nullable: false),
                    SecondTagId = table.Column<int>(nullable: false),
                    ArrowStyle = table.Column<string>(nullable: true),
                    Color = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagRelations", x => new { x.FirstTagId, x.SecondTagId });
                    table.ForeignKey(
                        name: "FK_TagRelations_AnnotationTags_FirstTagId",
                        column: x => x.FirstTagId,
                        principalTable: "AnnotationTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagRelations_AnnotationTags_SecondTagId",
                        column: x => x.SecondTagId,
                        principalTable: "AnnotationTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnnotationTagInstances_TagModelId",
                table: "AnnotationTagInstances",
                column: "TagModelId");

            migrationBuilder.CreateIndex(
                name: "IX_TagRelations_SecondTagId",
                table: "TagRelations",
                column: "SecondTagId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnnotationTagInstances");

            migrationBuilder.DropTable(
                name: "TagRelations");
        }
    }
}
