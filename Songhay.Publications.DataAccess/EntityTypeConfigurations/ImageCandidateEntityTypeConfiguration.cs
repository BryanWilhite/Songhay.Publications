using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Songhay.Publications.Models;

namespace Songhay.Publications.DataAccess.EntityTypeConfigurations;

public class ImageCandidateEntityTypeConfiguration : IEntityTypeConfiguration<ImageCandidate>
{
    public void Configure(EntityTypeBuilder<ImageCandidate> builder)
    {
        builder.HasKey(e => e.ImageUri);
    }
}
