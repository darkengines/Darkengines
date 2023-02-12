using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Darkengines.Migrations
{
    /// <inheritdoc />
    public partial class UserEmailAddressHashedEmailAddressUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserEmailAddress_HashedEmailAddress",
                table: "UserEmailAddress");

            migrationBuilder.CreateIndex(
                name: "IX_UserEmailAddress_HashedEmailAddress",
                table: "UserEmailAddress",
                column: "HashedEmailAddress",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserEmailAddress_HashedEmailAddress",
                table: "UserEmailAddress");

            migrationBuilder.CreateIndex(
                name: "IX_UserEmailAddress_HashedEmailAddress",
                table: "UserEmailAddress",
                column: "HashedEmailAddress");
        }
    }
}
