using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Songhay.Publications.Models;

namespace Songhay.Publications.DataAccess.EntityTypeConfigurations;

public class IndexKeywordGroupEntityTypeConfiguration : IEntityTypeConfiguration<IndexKeywordGroup>
{
    public void Configure(EntityTypeBuilder<IndexKeywordGroup> builder)
    {
        builder
            .ToTable($"{nameof(IndexKeywordGroup)}s")
            .HasKey(e => e.Id);
        builder.HasIndex(e => e.ClientId);

        builder
            .Property(e => e.Name)
            .HasMaxLength(255);
    }
}
