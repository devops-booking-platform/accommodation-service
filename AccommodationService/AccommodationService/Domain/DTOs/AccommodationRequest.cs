using System.ComponentModel.DataAnnotations;
using AccommodationService.Domain.Entities;
using AccommodationService.Domain.Enums;

namespace AccommodationService.Domain.DTOs;

public class AccommodationRequest
{
    [Required]
    [MaxLength(Accommodation.NameMaxLength)]
    public string Name { get; set; } = string.Empty;
    public LocationRequest Location { get; set; } = null!;
    public int MinimumNumberOfGuests { get; set; }
    public int MaximumNumberOfGuests { get; set; }
    
    [Required]
    [MaxLength(Accommodation.DescriptionMaxLength)]
    public string Description { get; set; } = string.Empty;
    public bool IsAutoConfirm { get; set; }
    [Required]
    public PriceType PriceType { get; set; }
    public ICollection<string> Photos { get; set; } = new HashSet<string>();
    public ICollection<Guid> Amenities { get; set; } = new HashSet<Guid>();
}