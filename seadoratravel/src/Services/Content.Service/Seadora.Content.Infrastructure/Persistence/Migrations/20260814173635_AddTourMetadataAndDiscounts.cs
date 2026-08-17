using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seadora.Content.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTourMetadataAndDiscounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercentage",
                table: "Tours",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FreeCancellation",
                table: "Tours",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HotelPickup",
                table: "Tours",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBestseller",
                table: "Tours",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsInHighDemand",
                table: "Tours",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrivateOption",
                table: "Tours",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTopRated",
                table: "Tours",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "OriginalPrice",
                table: "Tours",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Rating",
                table: "Tours",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "ReserveAndPayLater",
                table: "Tours",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ReviewCount",
                table: "Tours",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StartTime",
                table: "Tours",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountPercentage",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "FreeCancellation",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "HotelPickup",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "IsBestseller",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "IsInHighDemand",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "IsPrivateOption",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "IsTopRated",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "OriginalPrice",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "ReserveAndPayLater",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "ReviewCount",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Tours");
        }
    }
}
