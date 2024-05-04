using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Darkengines.Migrations
{
    /// <inheritdoc />
    public partial class passwordResetRequestprimarykey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPasswordResetRequest",
                table: "UserPasswordResetRequest");

            migrationBuilder.AlterColumn<string>(
                name: "MemberType",
                table: "Member",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPasswordResetRequest",
                table: "UserPasswordResetRequest",
                columns: new[] { "UserId", "Guid" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPasswordResetRequest",
                table: "UserPasswordResetRequest");

            migrationBuilder.AlterColumn<string>(
                name: "MemberType",
                table: "Member",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(13)",
                oldMaxLength: 13);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPasswordResetRequest",
                table: "UserPasswordResetRequest",
                column: "UserId");
        }
    }
}
