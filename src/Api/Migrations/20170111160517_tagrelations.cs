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
            migrationBuilder.CreateTable(
                name: "TagRelations",
                columns: table => new
                {
                    Relationid = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    FirstTagId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    SecondTagId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagRelations", x => x.Relationid);
                    table.ForeignKey(
                        name: "FK_TagRelations_AnnotationTags_FirstTagId",
                        column: x => x.FirstTagId,
                        principalTable: "AnnotationTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TagRelations_AnnotationTags_SecondTagId",
                        column: x => x.SecondTagId,
                        principalTable: "AnnotationTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TagRelations_FirstTagId",
                table: "TagRelations",
                column: "FirstTagId");

            migrationBuilder.CreateIndex(
                name: "IX_TagRelations_SecondTagId",
                table: "TagRelations",
                column: "SecondTagId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TagRelations");
        }
    }
}
