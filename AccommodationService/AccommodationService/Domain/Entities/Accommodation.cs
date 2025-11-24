using AccommodationService.Domain.DTOs;

namespace AccommodationService.Domain.Entities;

public class Accommodation : EntityWithGuidId
{
    public const int NameMaxLength = 64;
    public const int DescriptionMaxLength = 512;
    public string Name { get; set; } = string.Empty;
    public Guid LocationId { get; set; }
    public Guid HostId { get; set; }
    public string? Description { get; set; }
    public bool IsAutoConfirm { get; set; }
    public int MinimumNumberOfGuests { get; set; }
    public int MaximumNumberOfGuests { get; set; }

    public Location? Location { get; set; }
    public ICollection<Photo> Photos { get; set; } = new HashSet<Photo>();
    public ICollection<Amenity> Amenities { get; set; } = new HashSet<Amenity>();
    public ICollection<Availability> Availabilities { get; set; } = new HashSet<Availability>();

    public static Accommodation Create(AccommodationRequest request, Guid hostId)
    {
        var location = Location.Create(request.Location);

        var accommodation = new Accommodation
        {
            Name = request.Name,
            Description = request.Description,
            IsAutoConfirm = request.IsAutoConfirm,
            MinimumNumberOfGuests = request.MinimumNumberOfGuests,
            MaximumNumberOfGuests = request.MaximumNumberOfGuests,
            HostId = hostId,
            LocationId = location.Id,
            Location = location
        };
        
        location.AccommodationId = accommodation.Id;

        return accommodation;
    }
}