using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Darkengines.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Application",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Model",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Model", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    HashedPassword = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    LastIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DeactivatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeactivatedByUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedById = table.Column<int>(type: "int", nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_User_DeactivatedByUserId",
                        column: x => x.DeactivatedByUserId,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_User_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Entity",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ModelName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entity", x => new { x.ModelName, x.Name });
                    table.ForeignKey(
                        name: "FK_Entity_Model_ModelName",
                        column: x => x.ModelName,
                        principalTable: "Model",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ForeignKey",
                columns: table => new
                {
                    ModelName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForeignKey", x => new { x.ModelName, x.Name });
                    table.ForeignKey(
                        name: "FK_ForeignKey_Model_ModelName",
                        column: x => x.ModelName,
                        principalTable: "Model",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserApplication",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ApplicationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_UserApplication_Application_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Application",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserApplication_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserEmailAddress",
                columns: table => new
                {
                    HashedEmailAddress = table.Column<byte[]>(type: "varbinary(900)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GuidExpirationDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedById = table.Column<int>(type: "int", nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEmailAddress", x => x.HashedEmailAddress);
                    table.ForeignKey(
                        name: "FK_UserEmailAddress_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserEmailAddress_User_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserEmailAddress_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPasswordResetRequest",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedById = table.Column<int>(type: "int", nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPasswordResetRequest", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserPasswordResetRequest_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserPasswordResetRequest_User_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserPasswordResetRequest_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProfile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Firstname = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Lastname = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedById = table.Column<int>(type: "int", nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProfile_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserProfile_User_Id",
                        column: x => x.Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserProfile_User_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedById = table.Column<int>(type: "int", nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSettings_User_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserSettings_User_Id",
                        column: x => x.Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSettings_User_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserUserGroup",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UserGroupId = table.Column<int>(type: "int", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserUserGroup", x => new { x.UserId, x.UserGroupId });
                    table.ForeignKey(
                        name: "FK_UserUserGroup_UserGroup_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "UserGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserUserGroup_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Member",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ModelName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MemberType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityModelName = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    ForeignKeyName = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    ForeignKeyModelName = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    IsNullable = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    TypeName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Format = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsAutoGenerated = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    MaximumLength = table.Column<int>(type: "int", nullable: true),
                    MinimumLength = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => new { x.ModelName, x.EntityName, x.Name });
                    table.ForeignKey(
                        name: "FK_Member_Entity_EntityModelName_EntityName",
                        columns: x => new { x.EntityModelName, x.EntityName },
                        principalTable: "Entity",
                        principalColumns: new[] { "ModelName", "Name" });
                    table.ForeignKey(
                        name: "FK_Member_Entity_ModelName_EntityName",
                        columns: x => new { x.ModelName, x.EntityName },
                        principalTable: "Entity",
                        principalColumns: new[] { "ModelName", "Name" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Member_ForeignKey_ForeignKeyModelName_ForeignKeyName",
                        columns: x => new { x.ForeignKeyModelName, x.ForeignKeyName },
                        principalTable: "ForeignKey",
                        principalColumns: new[] { "ModelName", "Name" });
                    table.ForeignKey(
                        name: "FK_Member_ForeignKey_ModelName_ForeignKeyName",
                        columns: x => new { x.ModelName, x.ForeignKeyName },
                        principalTable: "ForeignKey",
                        principalColumns: new[] { "ModelName", "Name" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ForeignKeyProperty",
                columns: table => new
                {
                    ModelName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ForeignKeyName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    DependentEntityName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    DependentPropertyName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PrincipalEntityName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PrincipalPropertyName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    DependentPropertyModelName = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    DependentPropertyEntityName = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    PrincipalPropertyModelName = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    PrincipalPropertyEntityName = table.Column<string>(type: "nvarchar(256)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForeignKeyProperty", x => new { x.ModelName, x.ForeignKeyName, x.PrincipalEntityName, x.PrincipalPropertyName, x.DependentEntityName, x.DependentPropertyName });
                    table.ForeignKey(
                        name: "FK_ForeignKeyProperty_Entity_ModelName_DependentEntityName",
                        columns: x => new { x.ModelName, x.DependentEntityName },
                        principalTable: "Entity",
                        principalColumns: new[] { "ModelName", "Name" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForeignKeyProperty_Entity_ModelName_PrincipalEntityName",
                        columns: x => new { x.ModelName, x.PrincipalEntityName },
                        principalTable: "Entity",
                        principalColumns: new[] { "ModelName", "Name" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ForeignKeyProperty_ForeignKey_ModelName_ForeignKeyName",
                        columns: x => new { x.ModelName, x.ForeignKeyName },
                        principalTable: "ForeignKey",
                        principalColumns: new[] { "ModelName", "Name" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ForeignKeyProperty_Member_DependentPropertyModelName_DependentPropertyEntityName_DependentPropertyName",
                        columns: x => new { x.DependentPropertyModelName, x.DependentPropertyEntityName, x.DependentPropertyName },
                        principalTable: "Member",
                        principalColumns: new[] { "ModelName", "EntityName", "Name" });
                    table.ForeignKey(
                        name: "FK_ForeignKeyProperty_Member_PrincipalPropertyModelName_PrincipalPropertyEntityName_PrincipalPropertyName",
                        columns: x => new { x.PrincipalPropertyModelName, x.PrincipalPropertyEntityName, x.PrincipalPropertyName },
                        principalTable: "Member",
                        principalColumns: new[] { "ModelName", "EntityName", "Name" });
                });

            migrationBuilder.CreateIndex(
                name: "IX_ForeignKeyProperty_DependentPropertyModelName_DependentPropertyEntityName_DependentPropertyName",
                table: "ForeignKeyProperty",
                columns: new[] { "DependentPropertyModelName", "DependentPropertyEntityName", "DependentPropertyName" });

            migrationBuilder.CreateIndex(
                name: "IX_ForeignKeyProperty_ModelName_DependentEntityName",
                table: "ForeignKeyProperty",
                columns: new[] { "ModelName", "DependentEntityName" });

            migrationBuilder.CreateIndex(
                name: "IX_ForeignKeyProperty_ModelName_PrincipalEntityName",
                table: "ForeignKeyProperty",
                columns: new[] { "ModelName", "PrincipalEntityName" });

            migrationBuilder.CreateIndex(
                name: "IX_ForeignKeyProperty_PrincipalPropertyModelName_PrincipalPropertyEntityName_PrincipalPropertyName",
                table: "ForeignKeyProperty",
                columns: new[] { "PrincipalPropertyModelName", "PrincipalPropertyEntityName", "PrincipalPropertyName" });

            migrationBuilder.CreateIndex(
                name: "IX_Member_EntityModelName_EntityName",
                table: "Member",
                columns: new[] { "EntityModelName", "EntityName" });

            migrationBuilder.CreateIndex(
                name: "IX_Member_ForeignKeyModelName_ForeignKeyName",
                table: "Member",
                columns: new[] { "ForeignKeyModelName", "ForeignKeyName" });

            migrationBuilder.CreateIndex(
                name: "IX_Member_ModelName_ForeignKeyName",
                table: "Member",
                columns: new[] { "ModelName", "ForeignKeyName" });

            migrationBuilder.CreateIndex(
                name: "IX_User_CreatedById",
                table: "User",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_User_DeactivatedByUserId",
                table: "User",
                column: "DeactivatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Login",
                table: "User",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_ModifiedById",
                table: "User",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserApplication_ApplicationId",
                table: "UserApplication",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserApplication_UserId",
                table: "UserApplication",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserEmailAddress_CreatedById",
                table: "UserEmailAddress",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserEmailAddress_ModifiedById",
                table: "UserEmailAddress",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserEmailAddress_UserId",
                table: "UserEmailAddress",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPasswordResetRequest_CreatedById",
                table: "UserPasswordResetRequest",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserPasswordResetRequest_Guid",
                table: "UserPasswordResetRequest",
                column: "Guid");

            migrationBuilder.CreateIndex(
                name: "IX_UserPasswordResetRequest_ModifiedById",
                table: "UserPasswordResetRequest",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_CreatedById",
                table: "UserProfile",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_ModifiedById",
                table: "UserProfile",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_CreatedById",
                table: "UserSettings",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_ModifiedById",
                table: "UserSettings",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserUserGroup_UserGroupId",
                table: "UserUserGroup",
                column: "UserGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ForeignKeyProperty");

            migrationBuilder.DropTable(
                name: "UserApplication");

            migrationBuilder.DropTable(
                name: "UserEmailAddress");

            migrationBuilder.DropTable(
                name: "UserPasswordResetRequest");

            migrationBuilder.DropTable(
                name: "UserProfile");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "UserUserGroup");

            migrationBuilder.DropTable(
                name: "Member");

            migrationBuilder.DropTable(
                name: "Application");

            migrationBuilder.DropTable(
                name: "UserGroup");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Entity");

            migrationBuilder.DropTable(
                name: "ForeignKey");

            migrationBuilder.DropTable(
                name: "Model");
        }
    }
}
