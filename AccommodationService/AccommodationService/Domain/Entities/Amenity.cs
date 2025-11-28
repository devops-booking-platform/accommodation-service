namespace AccommodationService.Domain.Entities;

public class Amenity : EntityWithGuidId
{
    public const int NameMaxLength = 64;
    public const int DescriptionMaxLength = 256;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Amenity(string name, string description)
    {
        Name = name;
        Description = description;
    }
}