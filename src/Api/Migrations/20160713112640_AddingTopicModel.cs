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
                    TopicId = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    Deadline = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Role = table.Column<string>(nullable: false),
                    Status = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.TopicId);
                });

            migrationBuilder.CreateTable(
                name: "UserTopics",
                columns: table => new
                {
                    SupervisorId = table.Column<int>(nullable: false),
                    SupervisorTopicId = table.Column<int>(nullable: false),
                    StudentId = table.Column<int>(nullable: false),
                    StudentTopicId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTopics", x => new { x.SupervisorId, x.SupervisorTopicId });
                    table.UniqueConstraint("AK_UserTopics_StudentId_StudentTopicId", x => new { x.StudentId, x.StudentTopicId });
                    table.ForeignKey(
                        name: "FK_UserTopics_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTopics_Topics_StudentTopicId",
                        column: x => x.StudentTopicId,
                        principalTable: "Topics",
                        principalColumn: "TopicId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTopics_Users_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTopics_Topics_SupervisorTopicId",
                        column: x => x.SupervisorTopicId,
                        principalTable: "Topics",
                        principalColumn: "TopicId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserTopics_StudentId",
                table: "UserTopics",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTopics_StudentTopicId",
                table: "UserTopics",
                column: "StudentTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTopics_SupervisorId",
                table: "UserTopics",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTopics_SupervisorTopicId",
                table: "UserTopics",
                column: "SupervisorTopicId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserTopics");

            migrationBuilder.DropTable(
                name: "Topics");
        }
    }
}
