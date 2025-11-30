using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccommodationService.Migrations
{
    /// <inheritdoc />
    public partial class AddedPriceType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PricePerNight",
                table: "Availability",
                newName: "Price");

            migrationBuilder.AddColumn<string>(
                name: "PriceType",
                table: "Accommodation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceType",
                table: "Accommodation");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Availability",
                newName: "PricePerNight");
        }
    }
}
