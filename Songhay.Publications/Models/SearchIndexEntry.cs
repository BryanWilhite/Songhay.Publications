namespace Songhay.Publications.Models;

/// <summary>
/// Defines the Search Index Entry.
/// </summary>
public class SearchIndexEntry : Document
{
    /// <summary>
    /// The <see cref="Document"/> Extract.
    /// </summary>
    public string? Extract { get; set; }
}
