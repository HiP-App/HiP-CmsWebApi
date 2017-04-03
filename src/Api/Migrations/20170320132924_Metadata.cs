using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Migrations
{
    public partial class Metadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Legals");

            migrationBuilder.DropTable(
                name: "TopicAttatchments");

            migrationBuilder.CreateTable(
                name: "TopicAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Path = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: false),
                    TopicId = table.Column<int>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TopicAttachments_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TopicAttachments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TopicAttachmentMetadata",
                columns: table => new
                {
                    TopicAttachmentId = table.Column<int>(nullable: false),
                    Copyright = table.Column<string>(nullable: true),
                    Creator = table.Column<string>(nullable: true),
                    Date = table.Column<string>(nullable: true),
                    Date2 = table.Column<string>(nullable: true),
                    Depth = table.Column<int>(nullable: false),
                    DetailedPosition = table.Column<string>(nullable: true),
                    Details = table.Column<string>(nullable: true),
                    Height = table.Column<int>(nullable: false),
                    Location = table.Column<string>(nullable: true),
                    Material = table.Column<string>(nullable: true),
                    Page = table.Column<int>(nullable: false),
                    Photographer = table.Column<string>(nullable: true),
                    PlaceOfManufacture = table.Column<string>(nullable: true),
                    PointOfOrigin = table.Column<string>(nullable: true),
                    Signature = table.Column<string>(nullable: true),
                    Source = table.Column<string>(nullable: true),
                    SubType = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Type = table.Column<string>(nullable: true),
                    Unit = table.Column<string>(nullable: true),
                    Width = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicAttachmentMetadata", x => x.TopicAttachmentId);
                    table.ForeignKey(
                        name: "FK_TopicAttachmentMetadata_TopicAttachments_TopicAttachmentId",
                        column: x => x.TopicAttachmentId,
                        principalTable: "TopicAttachments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TopicAttachments_TopicId",
                table: "TopicAttachments",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_TopicAttachments_UserId",
                table: "TopicAttachments",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TopicAttachmentMetadata");

            migrationBuilder.DropTable(
                name: "TopicAttachments");

            migrationBuilder.CreateTable(
                name: "TopicAttatchments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Description = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Path = table.Column<string>(nullable: true),
                    TopicId = table.Column<int>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicAttatchments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TopicAttatchments_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TopicAttatchments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Legals",
                columns: table => new
                {
                    TopicAttatchmentId = table.Column<int>(nullable: false),
                    Author = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    PublicationType = table.Column<string>(nullable: true),
                    PublishedDate = table.Column<string>(nullable: true),
                    Source = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Legals", x => x.TopicAttatchmentId);
                    table.ForeignKey(
                        name: "FK_Legals_TopicAttatchments_TopicAttatchmentId",
                        column: x => x.TopicAttatchmentId,
                        principalTable: "TopicAttatchments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TopicAttatchments_TopicId",
                table: "TopicAttatchments",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_TopicAttatchments_UserId",
                table: "TopicAttatchments",
                column: "UserId");
        }
    }
}
