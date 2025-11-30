namespace AccommodationService.Domain.DTOs;

public class AvailabilityRequest
{
    public Guid? Id { get; set; }
    public decimal Price { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public Guid AccommodationId { get; set; }
}