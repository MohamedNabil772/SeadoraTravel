using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seadora.Booking.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingMoneyAndType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                table: "Bookings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "Bookings",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Money_AddonsTotal",
                table: "Bookings",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Money_AmountPaid",
                table: "Bookings",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Money_BalanceDue",
                table: "Bookings",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Money_Currency",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Money_Discount",
                table: "Bookings",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Money_Subtotal",
                table: "Bookings",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Money_TaxTotal",
                table: "Bookings",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Money_Total",
                table: "Bookings",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TourTypeCode",
                table: "Bookings",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Money_AddonsTotal",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Money_AmountPaid",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Money_BalanceDue",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Money_Currency",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Money_Discount",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Money_Subtotal",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Money_TaxTotal",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Money_Total",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "TourTypeCode",
                table: "Bookings");
        }
    }
}
