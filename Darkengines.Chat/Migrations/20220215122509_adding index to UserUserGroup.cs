using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Darkengines.Expressions.Application.Migrations
{
    public partial class addingindextoUserUserGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Index",
                table: "UserUserGroup",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Index",
                table: "UserUserGroup");
        }
    }
}
