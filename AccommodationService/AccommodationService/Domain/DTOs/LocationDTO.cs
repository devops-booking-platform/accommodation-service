namespace AccommodationService.Domain.DTOs
{
    public class LocationDTO
    {
        public string Country { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string Address { get; init; } = string.Empty;
        public string PostalCode { get; init; } = string.Empty;
    }
}
