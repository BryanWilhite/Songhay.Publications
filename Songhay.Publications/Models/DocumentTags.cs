// ReSharper disable once CheckNamespace
namespace Songhay.Publications.Models;

/// <summary>
/// Defines the tags or keywords of an <see cref="IDocument"/>.
/// </summary>
/// <seealso cref="DocumentProperties"/>
/// <remarks>
/// The intent here is to make the general-purpose assertion
/// that all Publication documents are adorned with tags and properties.
/// </remarks>
public record DocumentTags
{
    /// <summary>
    /// The <see cref="IDocument.DocumentId"/>
    /// </summary>
    public required int DocumentId { get; set; }

    /// <summary>
    /// The collection of <see cref="IDocument"/> tags.
    /// </summary>
    public ICollection<string> Tags { get; set; } = new HashSet<string>();
}
