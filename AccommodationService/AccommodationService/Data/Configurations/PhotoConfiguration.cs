using AccommodationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccommodationService.Data.Configurations;

public class PhotoConfiguration : IEntityTypeConfiguration<Photo>
{
    public void Configure(EntityTypeBuilder<Photo> builder)
    {
        builder.Property(p => p.Url)
            .IsRequired()
            .HasMaxLength(Photo.UrlMaxLength);

        builder.Property(p => p.AccommodationId)
            .IsRequired();
    }
}