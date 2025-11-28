using AccommodationService.Domain.Entities;

namespace AccommodationService.Data.Seed;

public static class AmenitySeeding
{
    public static IReadOnlyList<Amenity> Data { get; }

    static AmenitySeeding()
    {
        Data = new List<Amenity>
        {
            // Basic & Comfort
            new("Wi-Fi", "High-speed wireless internet"),
            new("Air Conditioning", "Air conditioning available"),
            new("Heating", "Central or electric heating"),
            new("Hot Water", "Hot water available in the property"),
            new("Extra Pillows & Blankets", "Additional bedding for comfort"),
            new("Blackout Curtains", "Curtains that block outside light"),
            new("Workspace", "Desk or dedicated work area"),
            new("Wardrobe", "Wardrobe or closet available"),
            new("Iron", "Iron for clothing"),
            new("Hair Dryer", "Hair dryer available"),

            // Kitchen & Dining
            new("Fully Equipped Kitchen", "Kitchen suitable for cooking"),
            new("Refrigerator", "Fridge for food storage"),
            new("Oven", "Oven for baking or roasting"),
            new("Stove", "Stovetop for cooking"),
            new("Microwave", "Microwave oven available"),
            new("Dishwasher", "Dishwashing machine available"),
            new("Coffee Maker", "Machine for making coffee"),
            new("Electric Kettle", "Kettle for boiling water"),
            new("Toaster", "Toaster for bread or pastries"),
            new("Cooking Basics", "Pots, pans, oil, salt, and spices"),

            // Entertainment
            new("TV", "Television available"),
            new("Streaming Services", "Access to Netflix, Prime, etc."),
            new("Board Games", "Assorted board games"),
            new("Books", "Books available for reading"),

            // Laundry
            new("Washing Machine", "Laundry washing machine"),
            new("Dryer", "Clothes dryer available"),
            new("Drying Rack", "Drying rack for clothes"),

            // Outdoors
            new("Balcony", "Private balcony area"),
            new("Terrace", "Outdoor terrace space"),
            new("Private Garden", "Garden area exclusive to guests"),
            new("BBQ Grill", "Outdoor grill for cooking"),
            new("Outdoor Dining Area", "Table and seating outdoors"),
            new("Patio Furniture", "Outdoor chairs, sofa, or loungers"),

            // Parking & Transport
            new("Private Parking", "Dedicated private parking spot"),
            new("Street Parking", "Parking available on the street"),
            new("EV Charger", "Electric vehicle charging station"),

            // Safety
            new("Smoke Detector", "Smoke detection system"),
            new("Carbon Monoxide Detector", "CO detection device"),
            new("Fire Extinguisher", "Fire safety extinguisher"),
            new("First Aid Kit", "Basic first aid supplies")
        };
    }
}