using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Songhay.Publications.DataAccess.EntityTypeConfigurations;
using Songhay.Publications.Models;

namespace Songhay.Publications.DataAccess;

/// <summary>
/// The <see cref="DbContext"/> sub-class for Entity Framework Core.
/// </summary>
public class PublicationsDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of <see cref="PublicationsDbContext"/>
    /// </summary>
    /// <param name="options">the <see cref="DbContextOptions"/></param>
    /// <remarks>
    /// This constructor is optimized
    /// for <see cref="IServiceCollection"/> dependency injection
    /// in ASP.NET or the Generic .NET Host.
    /// 
    /// For more detail, see “DbContext Lifetime, Configuration, and Initialization”
    /// [https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/]
    /// 
    /// Example:
    /// <code>
    /// public void ConfigureServices(IServiceCollection services)
    /// {
    ///     services.AddDbContextFactory&lt;ApplicationDbContext&gt;(options =>
    ///         options.UseSqlServer("&lt;your connection string&gt;"));
    /// }
    /// </code>
    /// </remarks>
    public PublicationsDbContext(DbContextOptions<PublicationsDbContext> options) : base(options){}

    /// <summary>
    /// The <see cref="DbSet{T}"/> of <see cref="Segment"/>.
    /// </summary>
    public DbSet<Segment>? Segments { get; set; }

    /// <summary>
    /// The <see cref="DbSet{T}"/> of <see cref="Document"/>.
    /// </summary>
    public DbSet<Document>? Documents { get; set; }

    /// <summary>
    /// The <see cref="DbSet{T}"/> of <see cref="IndexKeyword"/>.
    /// </summary>
    public DbSet<IndexKeyword>? IndexKeywords { get; set; }

    /// <summary>
    /// The <see cref="DbSet{T}"/> of <see cref="ResponsiveImage"/>.
    /// </summary>
    public DbSet<ResponsiveImage>? ResponsiveImages { get; set; }

    /// <summary>
    /// The <see cref="DbSet{T}"/> of <see cref="ImageCandidate"/>.
    /// </summary>
    public DbSet<ImageCandidate>? ImageCandidates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new SegmentEntityTypeConfiguration().Configure(modelBuilder.Entity<Segment>());
        new DocumentEntityTypeConfiguration().Configure(modelBuilder.Entity<Document>());
        new IndexKeywordEntityTypeConfiguration().Configure(modelBuilder.Entity<IndexKeyword>());
        new IndexKeywordGroupEntityTypeConfiguration().Configure(modelBuilder.Entity<IndexKeywordGroup>());
        new ResponsiveImageEntityTypeConfiguration().Configure(modelBuilder.Entity<ResponsiveImage>());
        new ImageCandidateEntityTypeConfiguration().Configure(modelBuilder.Entity<ImageCandidate>());
    }
}
