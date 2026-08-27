using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seadora.Booking.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartures : Migration
    {
        // ponytail: hand-trimmed - the generated Up also emitted an `xmin` column. xmin is a PostgreSQL
        // system column present on every table; EF only reads it as the concurrency token, so creating it
        // would both fail and be redundant. Table + unique index only, purely additive.
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    TourId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TimeSlot = table.Column<string>(type: "text", nullable: false),
                    Capacity = table.Column<int>(type: "integer", nullable: false),
                    AllocationModel = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departures", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Departures_TourId_StartUtc_TimeSlot",
                table: "Departures",
                columns: new[] { "TourId", "StartUtc", "TimeSlot" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Departures");
        }
    }
}
