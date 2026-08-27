using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seadora.Customer.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingHistoryAndInbox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookingHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    TourId = table.Column<Guid>(type: "uuid", nullable: false),
                    TourDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    PlacedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingHistory", x => x.Id);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_BookingHistory_BookingId",
                table: "BookingHistory",
                column: "BookingId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingHistory");

            migrationBuilder.DropTable(
                name: "ProcessedMessages");
        }
    }
}
