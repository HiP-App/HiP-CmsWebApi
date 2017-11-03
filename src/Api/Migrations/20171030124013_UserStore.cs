using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Linq;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Migrations
{
    public partial class UserStore : Migration
    {
        public const string AdminUserId = "auth0|5968ed8cdd1b3733ca94865d"; // Account "admin@hipapp.de"

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Users_UpdaterId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Users_UpdaterId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Users_UserId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentDetails_Users_UserId",
                table: "StudentDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Users_SubscriberId",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_TopicAttachments_Users_UserId",
                table: "TopicAttachments");

            migrationBuilder.DropForeignKey(
                name: "FK_TopicReviews_Users_ReviewerId",
                table: "TopicReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Users_CreatedById",
                table: "Topics");

            migrationBuilder.DropForeignKey(
                name: "FK_TopicUsers_Users_UserId",
                table: "TopicUsers");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_TopicUsers_UserId",
                table: "TopicUsers");

            migrationBuilder.DropIndex(
                name: "IX_Topics_CreatedById",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_TopicReviews_ReviewerId",
                table: "TopicReviews");

            migrationBuilder.DropIndex(
                name: "IX_TopicAttachments_UserId",
                table: "TopicAttachments");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_SubscriberId",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_UpdaterId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Documents_UpdaterId",
                table: "Documents");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "TopicUsers",
                type: "text",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "Topics",
                type: "text",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "ReviewerId",
                table: "TopicReviews",
                type: "text",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "TopicAttachments",
                type: "text",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "SubscriberId",
                table: "Subscriptions",
                type: "text",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "StudentDetails",
                type: "text",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Notifications",
                type: "text",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "UpdaterId",
                table: "Notifications",
                type: "text",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "UpdaterId",
                table: "Documents",
                type: "text",
                nullable: false,
                oldClrType: typeof(int));

            // Custom migration logic:
            //
            // - Columns holding user IDs are now of type text, but still contain the old integer user ID
            //   referring to the (no longer available) 'User'-table. We now "correct" this by assigning the
            //   pre-configured admin user ID.
            //
            // - In cases where the user ID is part of the primary key, we may now have created duplicates,
            //   which need to be eliminated (otherwise primary key constraint is violated and changes can't be saved)
            //
            AssignAdminUserIdAndRemoveDuplicates("TopicUsers", "UserId", new[] { "TopicId", "UserId", "Role" });
            AssignAdminUserId("Topics", "CreatedById");
            AssignAdminUserIdAndRemoveDuplicates("TopicReviews", "ReviewerId", new[] { "TopicId", "ReviewerId" });
            AssignAdminUserId("TopicAttachments", "UserId");
            AssignAdminUserId("Subscriptions", "SubscriberId");
            AssignAdminUserIdAndRemoveDuplicates("StudentDetails", "UserId", new[] { "UserId" });
            AssignAdminUserId("Notifications", "UserId");
            AssignAdminUserId("Notifications", "UpdaterId");
            AssignAdminUserId("Documents", "UpdaterId");

            void AssignAdminUserIdAndRemoveDuplicates(string table, string userIdColumn, string[] primaryKeyColumns)
            {
                // temporarily remove primary key uniquencess constraint
                migrationBuilder.DropUniqueConstraint(
                    name: "PK_" + table,
                    table: table);

                // replace invalid user IDs with admin user ID (possibly introducing duplicates)
                AssignAdminUserId(table, userIdColumn);

                // remove the duplicates
                RemoveDuplicates(table, primaryKeyColumns);

                // restore the uniqueness constraint
                migrationBuilder.AddUniqueConstraint(
                    name: "PK_" + table,
                    table: table,
                    columns: primaryKeyColumns);
            }

            void AssignAdminUserId(string table, string column)
            {
                migrationBuilder.Sql($"UPDATE \"{table}\" SET \"{column}\" = '{AdminUserId}'");
            }

            void RemoveDuplicates(string table, params string[] primaryKeyColumns)
            {
                var keyString = string.Join(", ", primaryKeyColumns.Select(c => $"\"{c}\""));
                migrationBuilder.Sql($"DELETE FROM \"{table}\" WHERE CTID NOT IN (SELECT MIN(CTID) FROM \"{table}\" GROUP BY {keyString})");
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "TopicUsers",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedById",
                table: "Topics",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "ReviewerId",
                table: "TopicReviews",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "TopicAttachments",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SubscriberId",
                table: "Subscriptions",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "StudentDetails",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Notifications",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "UpdaterId",
                table: "Notifications",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "UpdaterId",
                table: "Documents",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Email = table.Column<string>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    ProfilePicture = table.Column<string>(nullable: true),
                    Role = table.Column<string>(nullable: false),
                    UId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TopicUsers_UserId",
                table: "TopicUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_CreatedById",
                table: "Topics",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TopicReviews_ReviewerId",
                table: "TopicReviews",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_TopicAttachments_UserId",
                table: "TopicAttachments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_SubscriberId",
                table: "Subscriptions",
                column: "SubscriberId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UpdaterId",
                table: "Notifications",
                column: "UpdaterId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_UpdaterId",
                table: "Documents",
                column: "UpdaterId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Users_UpdaterId",
                table: "Documents",
                column: "UpdaterId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_UpdaterId",
                table: "Notifications",
                column: "UpdaterId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_UserId",
                table: "Notifications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentDetails_Users_UserId",
                table: "StudentDetails",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Users_SubscriberId",
                table: "Subscriptions",
                column: "SubscriberId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TopicAttachments_Users_UserId",
                table: "TopicAttachments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TopicReviews_Users_ReviewerId",
                table: "TopicReviews",
                column: "ReviewerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_Users_CreatedById",
                table: "Topics",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TopicUsers_Users_UserId",
                table: "TopicUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
