using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Songhay.Publications.Models;

namespace Songhay.Publications.DataAccess.EntityTypeConfigurations;

public class SegmentEntityTypeConfiguration : IEntityTypeConfiguration<Segment>
{
    public void Configure(EntityTypeBuilder<Segment> builder)
    {
        builder.HasKey(e => e.SegmentId);
        builder.HasIndex(e => e.ClientId);

        builder
            .Property(e => e.SegmentName)
            .IsRequired()
            .HasMaxLength(512);

        builder
            .HasMany(segment => segment.Documents)
            .WithOne(document => document.Segment)
            .HasForeignKey(document => document.SegmentId);

        builder.HasOne(segment => segment.ParentSegment);
    }
}
