namespace AccommodationService.Domain.DTOs
{
	public sealed class HostAccommodationListItemDTO
	{
		public Guid Id { get; init; }
		public string Name { get; init; } = default!;
		public string Address { get; init; } = default!;
		public int MinGuests { get; init; }
		public int MaxGuests { get; init; }
	}
}
