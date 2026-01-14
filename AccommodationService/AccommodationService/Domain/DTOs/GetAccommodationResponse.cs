using AccommodationService.Domain.Enums;

namespace AccommodationService.Domain.DTOs;

public class GetAccommodationResponse
{
    public Guid Id { get; set; }
    public Guid HostId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsAutoConfirm { get; set; }

    public int MinimumNumberOfGuests { get; set; }

    public int MaximumNumberOfGuests { get; set; }

    public PriceType PriceType { get; set; }

    public LocationResponseDto Location { get; set; } = default!;

    public List<string> Photos { get; set; } = new();

    public List<AmenityResponseDto> Amenities { get; set; } = new();

    public List<AvailabilityResponseDto> Availabilities { get; set; } = new();
}

public class LocationResponseDto
{
    public Guid Id { get; set; }

    public string? Country { get; set; }

    public string? City { get; set; }

    public string? Address { get; set; }

    public string? PostalCode { get; set; }
}

public class AmenityResponseDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}

public class AvailabilityResponseDto
{
    public Guid Id { get; set; }

    public decimal Price { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }
}