using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Darkengines.Migrations
{
    /// <inheritdoc />
    public partial class UserGroupExtenalId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "UserGroup",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroup_ExternalId",
                table: "UserGroup",
                column: "ExternalId",
                filter: "[ExternalId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserGroup_ExternalId",
                table: "UserGroup");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "UserGroup");
        }
    }
}
