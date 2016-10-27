using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Api.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    Email = table.Column<string>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    ProfilePicture = table.Column<string>(nullable: true),
                    Role = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    CreatedAt = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    CreatedById = table.Column<int>(nullable: false),
                    Deadline = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Requirements = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Topics_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssociatedTopics",
                columns: table => new
                {
                    ParentTopicId = table.Column<int>(nullable: false),
                    ChildTopicId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssociatedTopics", x => new { x.ParentTopicId, x.ChildTopicId });
                    table.ForeignKey(
                        name: "FK_AssociatedTopics_Topics_ChildTopicId",
                        column: x => x.ChildTopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssociatedTopics_Topics_ParentTopicId",
                        column: x => x.ParentTopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TopicAttatchments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    Description = table.Column<string>(nullable: true),
                    Legal = table.Column<string>(nullable: true),
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
                name: "TopicUsers",
                columns: table => new
                {
                    TopicId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    Role = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicUsers", x => new { x.TopicId, x.UserId, x.Role });
                    table.ForeignKey(
                        name: "FK_TopicUsers_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TopicUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssociatedTopics_ChildTopicId",
                table: "AssociatedTopics",
                column: "ChildTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_AssociatedTopics_ParentTopicId",
                table: "AssociatedTopics",
                column: "ParentTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_CreatedById",
                table: "Topics",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TopicAttatchments_TopicId",
                table: "TopicAttatchments",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_TopicAttatchments_UserId",
                table: "TopicAttatchments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TopicUsers_TopicId",
                table: "TopicUsers",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_TopicUsers_UserId",
                table: "TopicUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssociatedTopics");

            migrationBuilder.DropTable(
                name: "TopicAttatchments");

            migrationBuilder.DropTable(
                name: "TopicUsers");

            migrationBuilder.DropTable(
                name: "Topics");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
