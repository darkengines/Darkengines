using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Darkengines.Expressions.Application.Migrations
{
    /// <inheritdoc />
    public partial class HashedEmailAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HashedEmailAddress",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "HashedPassword",
                table: "User",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HashedEmailAddress",
                table: "User");

            migrationBuilder.DropColumn(
                name: "HashedPassword",
                table: "User");
        }
    }
}
