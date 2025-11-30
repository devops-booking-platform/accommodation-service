using AccommodationService.Data.Seed;
using AccommodationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccommodationService.Data.Configurations;

public class AmenityConfiguration : IEntityTypeConfiguration<Amenity>
{
    public void Configure(EntityTypeBuilder<Amenity> builder)
    {
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(Amenity.NameMaxLength);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(Amenity.DescriptionMaxLength);

        builder.HasData(AmenitySeeding.Data);
    }
}