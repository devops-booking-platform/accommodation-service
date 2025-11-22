using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccommodationService.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Amenity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amenity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    City = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    AccommodationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Accommodation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    IsAutoConfirm = table.Column<bool>(type: "bit", nullable: false),
                    MinimumNumberOfGuests = table.Column<int>(type: "int", nullable: false),
                    MaximumNumberOfGuests = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accommodation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accommodation_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccommodationAmenity",
                columns: table => new
                {
                    AccommodationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AmenitiesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccommodationAmenity", x => new { x.AccommodationId, x.AmenitiesId });
                    table.ForeignKey(
                        name: "FK_AccommodationAmenity_Accommodation_AccommodationId",
                        column: x => x.AccommodationId,
                        principalTable: "Accommodation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccommodationAmenity_Amenity_AmenitiesId",
                        column: x => x.AmenitiesId,
                        principalTable: "Amenity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Availability",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PricePerNight = table.Column<decimal>(type: "decimal(12,4)", precision: 12, scale: 4, nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    AccommodationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Availability", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Availability_Accommodation_AccommodationId",
                        column: x => x.AccommodationId,
                        principalTable: "Accommodation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Photo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    AccommodationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photo_Accommodation_AccommodationId",
                        column: x => x.AccommodationId,
                        principalTable: "Accommodation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accommodation_LocationId",
                table: "Accommodation",
                column: "LocationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccommodationAmenity_AmenitiesId",
                table: "AccommodationAmenity",
                column: "AmenitiesId");

            migrationBuilder.CreateIndex(
                name: "IX_Availability_AccommodationId",
                table: "Availability",
                column: "AccommodationId");

            migrationBuilder.CreateIndex(
                name: "IX_Photo_AccommodationId",
                table: "Photo",
                column: "AccommodationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccommodationAmenity");

            migrationBuilder.DropTable(
                name: "Availability");

            migrationBuilder.DropTable(
                name: "Photo");

            migrationBuilder.DropTable(
                name: "Amenity");

            migrationBuilder.DropTable(
                name: "Accommodation");

            migrationBuilder.DropTable(
                name: "Location");
        }
    }
}
