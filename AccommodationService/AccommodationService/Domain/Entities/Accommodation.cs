namespace AccommodationService.Domain.Entities;

public class Accommodation : EntityWithGuidId
{
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
}