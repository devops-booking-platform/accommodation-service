using AccommodationService.Domain.DTOs;

namespace AccommodationService.Domain.Entities;

public class Availability : EntityWithGuidId
{
    public decimal Price { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public Guid AccommodationId { get; set; }
    public Accommodation? Accommodation { get; set; }

    public static Availability Create(AvailabilityRequest request)
        => new()
        {
            Price = request.Price,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            AccommodationId = request.AccommodationId,
        };

    public void Update(AvailabilityRequest request)
    {
        Price = request.Price;
        StartDate = request.StartDate;
        EndDate = request.EndDate;
    }
}