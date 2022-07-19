using Newtonsoft.Json.Linq;

namespace Songhay.Publications.Models;

/// <summary>
/// Defines a conventional, Publication entry.
/// </summary>
public class MarkdownEntry
{
    /// <summary>
    /// Defines the new-line convention for entries.
    /// </summary>
    public static readonly string NewLine = Environment.NewLine;

    /// <summary>
    /// Markdown <see cref="FileInfo" />.
    /// </summary>
    public FileInfo? EntryFileInfo { get; set; }

    /// <summary>
    /// JSON front matter.
    /// </summary>
    public JObject? FrontMatter { get; set; }

    /// <summary>
    /// Text content.
    /// </summary>
    public string? Content { get; set; }
}
