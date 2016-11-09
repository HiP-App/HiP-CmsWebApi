using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Api.Migrations
{
    public partial class AnnotationTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnnotationTags",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    Description = table.Column<string>(nullable: true),
                    Icon = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Layer = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    ParentTagId = table.Column<int>(nullable: true),
                    ShortName = table.Column<string>(nullable: false),
                    Style = table.Column<string>(nullable: true),
                    UsageCounter = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnotationTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnnotationTags_AnnotationTags_ParentTagId",
                        column: x => x.ParentTagId,
                        principalTable: "AnnotationTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnnotationTags_ParentTagId",
                table: "AnnotationTags",
                column: "ParentTagId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnnotationTags");
        }
    }
}
