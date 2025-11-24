namespace AccommodationService.Domain.Entities;

public class Availability : EntityWithGuidId
{
    public decimal PricePerNight { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public Guid AccommodationId { get; set; }
    public Accommodation? Accommodation { get; set; }
}