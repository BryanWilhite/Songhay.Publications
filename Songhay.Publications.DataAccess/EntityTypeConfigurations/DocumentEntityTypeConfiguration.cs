using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Songhay.Publications.Models;

namespace Songhay.Publications.DataAccess.EntityTypeConfigurations;

public class DocumentEntityTypeConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.HasKey(e => e.DocumentId);
        builder.HasIndex(e => e.ClientId);

        builder
            .Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(1024);

        builder
            .Property(e => e.DocumentShortName)
            .HasMaxLength(255);

        builder
            .Property(e => e.Path)
            .HasMaxLength(2048);

        builder
            .Property(e => e.FileName)
            .HasMaxLength(512);

        builder
            .HasMany(document => document.IndexKeywords)
            .WithMany(keyword => keyword.Documents);

        builder.HasMany(document => document.ResponsiveImages);

        builder.Ignore(e => e.Fragments);
    }
}
