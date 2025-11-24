namespace AccommodationService.Domain.Entities;

public abstract class EntityWithGuidId
{
    public Guid Id { get; set; } = Guid.NewGuid();
}