using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seadora.Content.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTourTypePolicy : Migration
    {
        // ponytail: hand-trimmed to the 6 new TourTypes columns. The scaffolded body also carried
        // years of pre-existing snapshot drift (renames, drops, table creates) that ContentSeeder
        // already owns via idempotent raw SQL; replaying it here would be destructive.

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AllocationModel",
                table: "TourTypes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DefaultMinCapacity",
                table: "TourTypes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefaultMaxCapacity",
                table: "TourTypes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresGuestDetails",
                table: "TourTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresPassport",
                table: "TourTypes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PayLaterAllowed",
                table: "TourTypes",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "AllocationModel", table: "TourTypes");
            migrationBuilder.DropColumn(name: "DefaultMinCapacity", table: "TourTypes");
            migrationBuilder.DropColumn(name: "DefaultMaxCapacity", table: "TourTypes");
            migrationBuilder.DropColumn(name: "RequiresGuestDetails", table: "TourTypes");
            migrationBuilder.DropColumn(name: "RequiresPassport", table: "TourTypes");
            migrationBuilder.DropColumn(name: "PayLaterAllowed", table: "TourTypes");
        }
    }
}
