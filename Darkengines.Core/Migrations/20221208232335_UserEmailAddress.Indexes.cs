using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Darkengines.Core.Migrations
{
    /// <inheritdoc />
    public partial class UserEmailAddressIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserEmailAddress",
                table: "UserEmailAddress");

            migrationBuilder.DropIndex(
                name: "IX_UserEmailAddress_UserId",
                table: "UserEmailAddress");

            migrationBuilder.AlterColumn<byte[]>(
                name: "HashedEmailAddress",
                table: "UserEmailAddress",
                type: "varbinary(900)",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "UserEmailAddress",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserEmailAddress",
                table: "UserEmailAddress",
                columns: new[] { "UserId", "EmailAddress" });

            migrationBuilder.CreateIndex(
                name: "IX_UserEmailAddress_HashedEmailAddress",
                table: "UserEmailAddress",
                column: "HashedEmailAddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserEmailAddress",
                table: "UserEmailAddress");

            migrationBuilder.DropIndex(
                name: "IX_UserEmailAddress_HashedEmailAddress",
                table: "UserEmailAddress");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "UserEmailAddress");

            migrationBuilder.AlterColumn<byte[]>(
                name: "HashedEmailAddress",
                table: "UserEmailAddress",
                type: "varbinary(max)",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(900)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserEmailAddress",
                table: "UserEmailAddress",
                column: "EmailAddress");

            migrationBuilder.CreateIndex(
                name: "IX_UserEmailAddress_UserId",
                table: "UserEmailAddress",
                column: "UserId");
        }
    }
}
