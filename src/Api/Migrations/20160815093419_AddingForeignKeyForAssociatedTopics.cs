using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Api.Migrations
{
    public partial class AddingForeignKeyForAssociatedTopics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AssociatedTopics_ChildTopicId",
                table: "AssociatedTopics",
                column: "ChildTopicId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssociatedTopics_Topics_ChildTopicId",
                table: "AssociatedTopics",
                column: "ChildTopicId",
                principalTable: "Topics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssociatedTopics_Topics_ChildTopicId",
                table: "AssociatedTopics");

            migrationBuilder.DropIndex(
                name: "IX_AssociatedTopics_ChildTopicId",
                table: "AssociatedTopics");
        }
    }
}
