namespace AccommodationService.Domain.Entities;

public class Photo : EntityWithGuidId
{
    public string Url { get; set; } =  string.Empty;
    public Guid AccommodationId { get; set; }
    public Accommodation? Accommodation { get; set; }
}