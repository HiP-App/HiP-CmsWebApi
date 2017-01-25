using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Api.Migrations
{
    public partial class Documents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Legal",
                table: "TopicAttatchments");

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    TopicId = table.Column<int>(nullable: false),
                    Content = table.Column<string>(maxLength: 65536, nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdaterId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.TopicId);
                    table.ForeignKey(
                        name: "FK_Documents_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Documents_Users_UpdaterId",
                        column: x => x.UpdaterId,
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
                name: "IX_Documents_UpdaterId",
                table: "Documents",
                column: "UpdaterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Legals");

            migrationBuilder.AddColumn<string>(
                name: "Legal",
                table: "TopicAttatchments",
                nullable: true);
        }
    }
}
