namespace AccommodationService.Domain.DTOs;

public sealed class HostAccommodationListItemDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public int MinGuests { get; init; }
    public int MaxGuests { get; init; }
}