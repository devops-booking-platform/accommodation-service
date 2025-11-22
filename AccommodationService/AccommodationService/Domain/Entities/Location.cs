namespace AccommodationService.Domain.Entities;

public class Location : EntityWithGuidId
{
    public string Country { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string? PostalCode { get; set; }
    public Guid AccommodationId { get; set; }
    public Accommodation? Accommodation { get; set; }
}