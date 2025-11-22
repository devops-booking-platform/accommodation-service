using AccommodationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccommodationService.Data.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.Property(p => p.Country)
            .IsRequired()
            .HasMaxLength(Location.StringFieldsMaxLength);

        builder.Property(p => p.City)
            .IsRequired()
            .HasMaxLength(Location.StringFieldsMaxLength);

        builder.Property(p => p.Address)
            .IsRequired()
            .HasMaxLength(Location.StringFieldsMaxLength);

        builder.Property(p => p.PostalCode)
            .HasMaxLength(Location.StringFieldsMaxLength);

        builder.Property(p => p.AccommodationId)
            .IsRequired();
    }
}