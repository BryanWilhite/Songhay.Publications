using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Songhay.Publications.Models;

namespace Songhay.Publications.DataAccess.EntityTypeConfigurations;

public class IndexKeywordEntityTypeConfiguration : IEntityTypeConfiguration<IndexKeyword>
{
    public void Configure(EntityTypeBuilder<IndexKeyword> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.ClientId);

        builder
            .Property(e => e.KeywordValue)
            .HasMaxLength(255);

        builder.HasMany(e => e.Groups);
    }
}
