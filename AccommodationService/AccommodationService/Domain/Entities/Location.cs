using AccommodationService.Domain.DTOs;

namespace AccommodationService.Domain.Entities;

public class Location : EntityWithGuidId
{
    public const int StringFieldsMaxLength = 128;
    public string Country { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string? PostalCode { get; set; }
    public Guid AccommodationId { get; set; }
    public Accommodation? Accommodation { get; set; }

    public static Location Create(LocationRequest request)
        => new()
        {
            Country = request.Country,
            City = request.City,
            Address = request.Address,
            PostalCode = request.PostalCode
        };

    public void Update(LocationRequest request)
    {
        Country = request.Country;
        City = request.City;
        Address = request.Address;
        PostalCode = request.PostalCode;
    }
}