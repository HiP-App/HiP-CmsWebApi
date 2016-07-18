using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Api.Migrations
{
    public partial class AddingTopicModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    Deadline = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Requirements = table.Column<string>(nullable: true),
                    ReviewerId = table.Column<int>(nullable: false),
                    Status = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Topics_Users_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentTopics",
                columns: table => new
                {
                    StudentId = table.Column<int>(nullable: false),
                    TopicId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentTopics", x => new { x.StudentId, x.TopicId });
                    table.ForeignKey(
                        name: "FK_StudentTopics_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentTopics_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupervisorTopics",
                columns: table => new
                {
                    SupervisorId = table.Column<int>(nullable: false),
                    TopicId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupervisorTopics", x => new { x.SupervisorId, x.TopicId });
                    table.ForeignKey(
                        name: "FK_SupervisorTopics_Users_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupervisorTopics_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentTopics_StudentId",
                table: "StudentTopics",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentTopics_TopicId",
                table: "StudentTopics",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_SupervisorTopics_SupervisorId",
                table: "SupervisorTopics",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_SupervisorTopics_TopicId",
                table: "SupervisorTopics",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_ReviewerId",
                table: "Topics",
                column: "ReviewerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentTopics");

            migrationBuilder.DropTable(
                name: "SupervisorTopics");

            migrationBuilder.DropTable(
                name: "Topics");
        }
    }
}
