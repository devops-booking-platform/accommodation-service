namespace AccommodationService.Domain.DTOs;

public class AccommodationRequest
{
    public string Name { get; set; } = string.Empty;
    public LocationRequest Location { get; set; } = null!;
    public int MinimumNumberOfGuests { get; set; }
    public int MaximumNumberOfGuests { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsAutoConfirm { get; set; }
    public ICollection<string> Photos { get; set; } = new HashSet<string>();
    public ICollection<Guid> Amenities { get; set; } = new HashSet<Guid>();
}