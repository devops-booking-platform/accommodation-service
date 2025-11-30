using AccommodationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccommodationService.Data.Configurations;

public class AccommodationConfiguration : IEntityTypeConfiguration<Accommodation>
{
    public void Configure(EntityTypeBuilder<Accommodation> builder)
    {
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(Accommodation.NameMaxLength);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(Accommodation.DescriptionMaxLength);

        builder.Property(p => p.HostId)
            .IsRequired();

        builder.HasOne(x => x.Location)
            .WithOne(x => x.Accommodation)
            .HasForeignKey<Accommodation>(x => x.LocationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Amenities)
            .WithMany();

        builder.HasMany(x => x.Photos)
            .WithOne(x => x.Accommodation)
            .HasForeignKey(x => x.AccommodationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Availabilities)
            .WithOne(x => x.Accommodation)
            .HasForeignKey(x => x.AccommodationId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(x => x.PriceType)
            .IsRequired()
            .HasConversion<string>();
    }
}