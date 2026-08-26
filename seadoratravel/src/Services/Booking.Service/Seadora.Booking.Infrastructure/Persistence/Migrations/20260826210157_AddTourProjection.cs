using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seadora.Booking.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTourProjection : Migration
    {
        // ponytail: hand-trimmed to be purely additive. The generated Up also re-created
        // ContactInquiries/Notifications and Bookings columns that already exist in the database
        // (pre-existing model-snapshot drift, unrelated to this change). Only the two new tables stay.
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProcessedMessages",
                columns: table => new
                {
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConsumerName = table.Column<string>(type: "text", nullable: false),
                    ProcessedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessedMessages", x => new { x.MessageId, x.ConsumerName });
                });

            migrationBuilder.CreateTable(
                name: "TourProjections",
                columns: table => new
                {
                    TourId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    TourTypeCode = table.Column<string>(type: "text", nullable: true),
                    AllocationModel = table.Column<string>(type: "text", nullable: false),
                    MinCapacity = table.Column<int>(type: "integer", nullable: false),
                    MaxCapacity = table.Column<int>(type: "integer", nullable: false),
                    RequiresGuestDetails = table.Column<bool>(type: "boolean", nullable: false),
                    RequiresPassport = table.Column<bool>(type: "boolean", nullable: false),
                    PayLaterAllowed = table.Column<bool>(type: "boolean", nullable: false),
                    PriceFrom = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourProjections", x => x.TourId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessedMessages");

            migrationBuilder.DropTable(
                name: "TourProjections");
        }
    }
}
