using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Songhay.Publications.Models;

namespace Songhay.Publications.DataAccess.EntityTypeConfigurations;

public class ResponsiveImageEntityTypeConfiguration : IEntityTypeConfiguration<ResponsiveImage>
{
    public void Configure(EntityTypeBuilder<ResponsiveImage> builder)
    {
        builder.HasKey(e => e.Key);
        builder.HasIndex(e => e.Source);

        builder
            .Property(e => e.Description)
            .HasMaxLength(512);

        builder.Ignore(e => e.Sizes);
    }
}
