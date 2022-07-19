namespace Songhay.Publications.Abstractions;

/// <summary>
/// Publications Document interface
/// </summary>
public interface IDocument: ITemporal
{
    /// <summary>
    /// Gets or sets the document identifier.
    /// </summary>
    int? DocumentId { get; set; }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    string Title { get; set; }

    /// <summary>
    /// Gets or sets the short name of the document.
    /// </summary>
    string DocumentShortName { get; set; }

    /// <summary>
    /// Gets or sets the name of the file.
    /// </summary>
    string FileName { get; set; }

    /// <summary>
    /// Gets or sets the path.
    /// </summary>
    string Path { get; set; }

    /// <summary>
    /// Gets or sets the template identifier.
    /// </summary>
    int? TemplateId { get; set; }

    /// <summary>
    /// Gets or sets the segment identifier.
    /// </summary>
    int? SegmentId { get; set; }

    /// <summary>
    /// Gets or sets the is root.
    /// </summary>
    bool? IsRoot { get; set; }

    /// <summary>
    /// Gets or sets the is active.
    /// </summary>
    bool? IsActive { get; set; }

    /// <summary>
    /// Gets or sets the sort ordinal.
    /// </summary>
    byte? SortOrdinal { get; set; }

    /// <summary>
    /// Gets or sets the client identifier.
    /// </summary>
    string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the tag.
    /// </summary>
    string Tag { get; set; }
}
