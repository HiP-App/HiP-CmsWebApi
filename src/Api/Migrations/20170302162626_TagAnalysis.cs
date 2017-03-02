using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Api.Migrations
{
    public partial class TagAnalysis : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsageCounter",
                table: "AnnotationTags");

            migrationBuilder.AddColumn<int>(
                name: "IdInDocument",
                table: "AnnotationTagInstances",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InDocumentTopicId",
                table: "AnnotationTagInstances",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PositionInDocument",
                table: "AnnotationTagInstances",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "AnnotationTagInstances",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnnotationTagInstances_InDocumentTopicId",
                table: "AnnotationTagInstances",
                column: "InDocumentTopicId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnnotationTagInstances_Documents_InDocumentTopicId",
                table: "AnnotationTagInstances",
                column: "InDocumentTopicId",
                principalTable: "Documents",
                principalColumn: "TopicId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnnotationTagInstances_Documents_InDocumentTopicId",
                table: "AnnotationTagInstances");

            migrationBuilder.DropIndex(
                name: "IX_AnnotationTagInstances_InDocumentTopicId",
                table: "AnnotationTagInstances");

            migrationBuilder.DropColumn(
                name: "IdInDocument",
                table: "AnnotationTagInstances");

            migrationBuilder.DropColumn(
                name: "InDocumentTopicId",
                table: "AnnotationTagInstances");

            migrationBuilder.DropColumn(
                name: "PositionInDocument",
                table: "AnnotationTagInstances");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "AnnotationTagInstances");

            migrationBuilder.AddColumn<int>(
                name: "UsageCounter",
                table: "AnnotationTags",
                nullable: false,
                defaultValue: 0);
        }
    }
}
