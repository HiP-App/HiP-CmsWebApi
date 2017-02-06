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
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "TagRelations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TagRelations_Id",
                table: "TagRelations",
                column: "Id");

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
                    SourceLayerId = table.Column<int>(nullable: false),
                    TargetLayerId = table.Column<int>(nullable: false),
                    ArrowStyle = table.Column<string>(nullable: true),
                    Color = table.Column<string>(nullable: true),
                    Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LayerRelationRules", x => new { x.SourceLayerId, x.TargetLayerId });
                    table.UniqueConstraint("AK_LayerRelationRules_Id", x => x.Id);
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
                name: "IX_LayerRelationRules_TargetLayerId",
                table: "LayerRelationRules",
                column: "TargetLayerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LayerRelationRules");

            migrationBuilder.DropTable(
                name: "Layers");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_TagRelations_Id",
                table: "TagRelations");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TagRelations");
        }
    }
}
