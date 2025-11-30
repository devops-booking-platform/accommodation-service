using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AccommodationService.Migrations
{
    /// <inheritdoc />
    public partial class AddedAmenitySeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Amenity",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("01684193-7e90-4f04-8ea1-a5dcf533b150"), "Wardrobe or closet available", "Wardrobe" },
                    { new Guid("04771b32-22e8-4877-8d98-f05ba616dc1b"), "Oven for baking or roasting", "Oven" },
                    { new Guid("16407b8b-eaa2-4045-8171-8de0701bed3a"), "Basic first aid supplies", "First Aid Kit" },
                    { new Guid("1a7388ce-eb51-4301-9231-6be9c64f9303"), "Television available", "TV" },
                    { new Guid("20a167a4-d94e-4131-a415-9eec6e6fbcd3"), "Kitchen suitable for cooking", "Fully Equipped Kitchen" },
                    { new Guid("22b89c5e-d41f-4705-8f4f-ecae301d7f1a"), "Fridge for food storage", "Refrigerator" },
                    { new Guid("24da175b-6e4e-4b9d-a104-5038855a86f4"), "Stovetop for cooking", "Stove" },
                    { new Guid("255ef502-43c2-42f4-8b0b-8e05f60eb537"), "Books available for reading", "Books" },
                    { new Guid("27556265-bcc0-415d-82a9-491360b25149"), "CO detection device", "Carbon Monoxide Detector" },
                    { new Guid("33cbff6a-0b16-40b0-ad5c-4496213cefbc"), "Microwave oven available", "Microwave" },
                    { new Guid("4d0fb2f2-b63f-458e-bc63-a613265ddab7"), "Clothes dryer available", "Dryer" },
                    { new Guid("5399ea27-ca14-4717-a6f0-530525e2c406"), "High-speed wireless internet", "Wi-Fi" },
                    { new Guid("56803dea-baf6-4293-9a0a-56d79afcaf1e"), "Dedicated private parking spot", "Private Parking" },
                    { new Guid("57dc2603-846f-46f2-a571-718c409fb2b7"), "Additional bedding for comfort", "Extra Pillows & Blankets" },
                    { new Guid("5e6c4ff6-718a-4fe1-a752-2316eff39d66"), "Outdoor chairs, sofa, or loungers", "Patio Furniture" },
                    { new Guid("62165eac-7085-4b64-a180-dc9629a218a1"), "Fire safety extinguisher", "Fire Extinguisher" },
                    { new Guid("677841ba-c64e-43b1-8c99-fce2125f2ad8"), "Air conditioning available", "Air Conditioning" },
                    { new Guid("68324f8f-555e-444b-ae00-9e91c2baccf6"), "Laundry washing machine", "Washing Machine" },
                    { new Guid("6b17c64d-9484-48ec-8742-d746c0d4fcd3"), "Outdoor terrace space", "Terrace" },
                    { new Guid("7c3125aa-d9ab-467d-9a9d-d7fa22ee1e10"), "Assorted board games", "Board Games" },
                    { new Guid("7ddbb4ba-2d1f-4b5c-ae24-ba98c096cd5a"), "Machine for making coffee", "Coffee Maker" },
                    { new Guid("883ae7f1-ac6f-4ff5-ad04-e75ee22f34d2"), "Desk or dedicated work area", "Workspace" },
                    { new Guid("a1ac66d6-e164-4ff3-8b6e-2832d5f1f06e"), "Dishwashing machine available", "Dishwasher" },
                    { new Guid("a751df3b-16bd-40a3-a810-09e994b9b792"), "Hot water available in the property", "Hot Water" },
                    { new Guid("a8e24c14-76e2-424a-81db-cd4ffa341fe7"), "Garden area exclusive to guests", "Private Garden" },
                    { new Guid("acf2b9dc-afc8-4367-9f6a-d486e9e80644"), "Outdoor grill for cooking", "BBQ Grill" },
                    { new Guid("b6b783c5-2c9a-4fd8-82e9-db04401e0d87"), "Curtains that block outside light", "Blackout Curtains" },
                    { new Guid("bc52047e-a560-4592-88e0-e2ba807113e2"), "Pots, pans, oil, salt, and spices", "Cooking Basics" },
                    { new Guid("bea91001-d5c5-4868-8295-9a36caab68cc"), "Private balcony area", "Balcony" },
                    { new Guid("c6914809-f9f1-4a4f-89f5-fa4238ad0d3b"), "Parking available on the street", "Street Parking" },
                    { new Guid("d2aadf15-132c-4d47-b407-68f10057425f"), "Iron for clothing", "Iron" },
                    { new Guid("d64e5b47-33fa-490b-9331-184a77bbfb03"), "Table and seating outdoors", "Outdoor Dining Area" },
                    { new Guid("d7d2c920-8d62-4900-9926-8d8f02f0861e"), "Hair dryer available", "Hair Dryer" },
                    { new Guid("d99d1b20-3b22-473e-91cc-2327537bb40c"), "Toaster for bread or pastries", "Toaster" },
                    { new Guid("e96afaa8-2440-4fed-b95e-6d05f2a561b8"), "Kettle for boiling water", "Electric Kettle" },
                    { new Guid("effb512e-847e-4a2d-8c6f-1449b0ebb5ce"), "Central or electric heating", "Heating" },
                    { new Guid("f7bca978-0dba-4aa2-a4fc-450c023bffb1"), "Access to Netflix, Prime, etc.", "Streaming Services" },
                    { new Guid("fa1dec29-cc4e-4bb3-b526-e83adafce25f"), "Drying rack for clothes", "Drying Rack" },
                    { new Guid("fd8707f0-6750-4a44-90fa-861b5410c265"), "Electric vehicle charging station", "EV Charger" },
                    { new Guid("fec9e4be-4635-42cb-b294-a71beb4a3487"), "Smoke detection system", "Smoke Detector" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("01684193-7e90-4f04-8ea1-a5dcf533b150"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("04771b32-22e8-4877-8d98-f05ba616dc1b"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("16407b8b-eaa2-4045-8171-8de0701bed3a"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("1a7388ce-eb51-4301-9231-6be9c64f9303"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("20a167a4-d94e-4131-a415-9eec6e6fbcd3"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("22b89c5e-d41f-4705-8f4f-ecae301d7f1a"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("24da175b-6e4e-4b9d-a104-5038855a86f4"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("255ef502-43c2-42f4-8b0b-8e05f60eb537"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("27556265-bcc0-415d-82a9-491360b25149"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("33cbff6a-0b16-40b0-ad5c-4496213cefbc"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("4d0fb2f2-b63f-458e-bc63-a613265ddab7"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("5399ea27-ca14-4717-a6f0-530525e2c406"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("56803dea-baf6-4293-9a0a-56d79afcaf1e"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("57dc2603-846f-46f2-a571-718c409fb2b7"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("5e6c4ff6-718a-4fe1-a752-2316eff39d66"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("62165eac-7085-4b64-a180-dc9629a218a1"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("677841ba-c64e-43b1-8c99-fce2125f2ad8"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("68324f8f-555e-444b-ae00-9e91c2baccf6"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("6b17c64d-9484-48ec-8742-d746c0d4fcd3"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("7c3125aa-d9ab-467d-9a9d-d7fa22ee1e10"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("7ddbb4ba-2d1f-4b5c-ae24-ba98c096cd5a"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("883ae7f1-ac6f-4ff5-ad04-e75ee22f34d2"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("a1ac66d6-e164-4ff3-8b6e-2832d5f1f06e"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("a751df3b-16bd-40a3-a810-09e994b9b792"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("a8e24c14-76e2-424a-81db-cd4ffa341fe7"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("acf2b9dc-afc8-4367-9f6a-d486e9e80644"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("b6b783c5-2c9a-4fd8-82e9-db04401e0d87"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("bc52047e-a560-4592-88e0-e2ba807113e2"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("bea91001-d5c5-4868-8295-9a36caab68cc"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("c6914809-f9f1-4a4f-89f5-fa4238ad0d3b"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("d2aadf15-132c-4d47-b407-68f10057425f"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("d64e5b47-33fa-490b-9331-184a77bbfb03"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("d7d2c920-8d62-4900-9926-8d8f02f0861e"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("d99d1b20-3b22-473e-91cc-2327537bb40c"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("e96afaa8-2440-4fed-b95e-6d05f2a561b8"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("effb512e-847e-4a2d-8c6f-1449b0ebb5ce"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("f7bca978-0dba-4aa2-a4fc-450c023bffb1"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("fa1dec29-cc4e-4bb3-b526-e83adafce25f"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("fd8707f0-6750-4a44-90fa-861b5410c265"));

            migrationBuilder.DeleteData(
                table: "Amenity",
                keyColumn: "Id",
                keyValue: new Guid("fec9e4be-4635-42cb-b294-a71beb4a3487"));
        }
    }
}
