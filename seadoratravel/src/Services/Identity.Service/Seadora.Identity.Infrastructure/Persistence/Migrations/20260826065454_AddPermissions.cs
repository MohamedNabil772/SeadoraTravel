using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seadora.Identity.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // The User entity gained these profile/social columns but no migration ever added them
            // (the model snapshot was updated, so `migrations add` produced no diff). Add them here,
            // idempotently, so a fresh or existing AspNetUsers has every column the model expects.
            migrationBuilder.Sql(@"
                ALTER TABLE ""AspNetUsers"" ADD COLUMN IF NOT EXISTS ""FullName"" text;
                ALTER TABLE ""AspNetUsers"" ADD COLUMN IF NOT EXISTS ""GoogleId"" text;
                ALTER TABLE ""AspNetUsers"" ADD COLUMN IF NOT EXISTS ""FacebookId"" text;
                ALTER TABLE ""AspNetUsers"" ADD COLUMN IF NOT EXISTS ""AppleId"" text;
                ALTER TABLE ""AspNetUsers"" ADD COLUMN IF NOT EXISTS ""AvatarUrl"" text;
                ALTER TABLE ""AspNetUsers"" ADD COLUMN IF NOT EXISTS ""CreatedAt"" timestamp with time zone NOT NULL DEFAULT (now() at time zone 'utc');
                ALTER TABLE ""AspNetUsers"" ADD COLUMN IF NOT EXISTS ""LastLoginAt"" timestamp with time zone;
            ");

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Module = table.Column<string>(type: "text", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    PermissionId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "Permissions");
        }
    }
}
