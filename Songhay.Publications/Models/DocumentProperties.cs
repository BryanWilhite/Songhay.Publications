// ReSharper disable once CheckNamespace
namespace Songhay.Publications.Models;

/// <summary>
/// Defines the properties of an <see cref="IDocument"/>.
/// </summary>
/// <seealso cref="DocumentTags"/>
/// <remarks>
/// The intent here is to make the general-purpose assertion
/// that all Publication documents are adorned with tags and properties.
/// </remarks>
public record DocumentProperties
{
    /// <summary>
    /// The <see cref="IDocument.DocumentId"/>
    /// </summary>
    public required int DocumentId { get; set; }

    /// <summary>
    /// The set of <see cref="IDocument"/> properties.
    /// </summary>
    public Dictionary<string, string> Properties { get; set; } = [];
}
