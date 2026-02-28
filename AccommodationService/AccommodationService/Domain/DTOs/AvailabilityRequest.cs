namespace AccommodationService.Domain.DTOs;

public class AvailabilityRequest
{
    public Guid? Id { get; set; }
    public decimal Price { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public Guid AccommodationId { get; set; }
}