using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Darkengines.Expressions.Application.Migrations
{
    public partial class introducing_applications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApplicationId",
                table: "User",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Application",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_ApplicationId",
                table: "User",
                column: "ApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Application_ApplicationId",
                table: "User",
                column: "ApplicationId",
                principalTable: "Application",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Application_ApplicationId",
                table: "User");

            migrationBuilder.DropTable(
                name: "Application");

            migrationBuilder.DropIndex(
                name: "IX_User_ApplicationId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "User");
        }
    }
}
