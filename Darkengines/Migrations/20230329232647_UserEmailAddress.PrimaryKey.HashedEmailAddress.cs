using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Darkengines.Migrations
{
    /// <inheritdoc />
    public partial class UserEmailAddressPrimaryKeyHashedEmailAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserEmailAddress",
                table: "UserEmailAddress");

            migrationBuilder.DropIndex(
                name: "IX_UserEmailAddress_HashedEmailAddress",
                table: "UserEmailAddress");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserEmailAddress",
                table: "UserEmailAddress",
                column: "HashedEmailAddress");

            migrationBuilder.CreateIndex(
                name: "IX_UserEmailAddress_UserId",
                table: "UserEmailAddress",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserEmailAddress",
                table: "UserEmailAddress");

            migrationBuilder.DropIndex(
                name: "IX_UserEmailAddress_UserId",
                table: "UserEmailAddress");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserEmailAddress",
                table: "UserEmailAddress",
                columns: new[] { "UserId", "EmailAddress" });

            migrationBuilder.CreateIndex(
                name: "IX_UserEmailAddress_HashedEmailAddress",
                table: "UserEmailAddress",
                column: "HashedEmailAddress",
                unique: true);
        }
    }
}
