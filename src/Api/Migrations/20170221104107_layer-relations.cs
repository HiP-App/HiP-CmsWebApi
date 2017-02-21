using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Api.Migrations
{
    public partial class layerrelations : Migration
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
                name: "AnnotationTagRelations",
                columns: table => new
                {
                    FirstTagId = table.Column<int>(nullable: false),
                    SecondTagId = table.Column<int>(nullable: false),
                    ArrowStyle = table.Column<string>(nullable: true),
                    Color = table.Column<string>(nullable: true),
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnotationTagRelations", x => new { x.FirstTagId, x.SecondTagId });
                    table.UniqueConstraint("AK_AnnotationTagRelations_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnnotationTagRelations_AnnotationTags_FirstTagId",
                        column: x => x.FirstTagId,
                        principalTable: "AnnotationTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnnotationTagRelations_AnnotationTags_SecondTagId",
                        column: x => x.SecondTagId,
                        principalTable: "AnnotationTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Layers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Layers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LayerRelationRules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ArrowStyle = table.Column<string>(nullable: true),
                    Color = table.Column<string>(nullable: true),
                    SourceLayerId = table.Column<int>(nullable: false),
                    TargetLayerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LayerRelationRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LayerRelationRules_Layers_SourceLayerId",
                        column: x => x.SourceLayerId,
                        principalTable: "Layers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LayerRelationRules_Layers_TargetLayerId",
                        column: x => x.TargetLayerId,
                        principalTable: "Layers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnnotationTagInstances_TagModelId",
                table: "AnnotationTagInstances",
                column: "TagModelId");

            migrationBuilder.CreateIndex(
                name: "IX_AnnotationTagRelations_SecondTagId",
                table: "AnnotationTagRelations",
                column: "SecondTagId");

            migrationBuilder.CreateIndex(
                name: "IX_LayerRelationRules_SourceLayerId",
                table: "LayerRelationRules",
                column: "SourceLayerId");

            migrationBuilder.CreateIndex(
                name: "IX_LayerRelationRules_TargetLayerId",
                table: "LayerRelationRules",
                column: "TargetLayerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnnotationTagInstances");

            migrationBuilder.DropTable(
                name: "AnnotationTagRelations");

            migrationBuilder.DropTable(
                name: "LayerRelationRules");

            migrationBuilder.DropTable(
                name: "Layers");
        }
    }
}
