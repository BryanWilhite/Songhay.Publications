using System.ComponentModel.DataAnnotations;

namespace Songhay.Publications.Models;

/// <summary>
/// Publications Document.
/// </summary>
public class Document : IDocument
{
    /// <summary>
    /// Gets or sets the client identifier.
    /// </summary>
    [Display(Name = "Client ID", Order = 2)]
    [DisallowNull]
    public string? ClientId { get => _clientId; set => _clientId = value.ToReferenceTypeValueOrThrow(); }

    /// <summary>
    /// Gets or sets the incept date.
    /// </summary>
    [Display(Name = "Incept Date", Order = 7)]
    public DateTime? InceptDate { get; set; }

    /// <summary>
    /// Gets or sets the document identifier.
    /// </summary>
    [Display(Name = "Document ID", Order = 1)]
    [DisallowNull]
    public int? DocumentId { get => _documentId; set => _documentId = value.ToValueOrThrow(); }

    /// <summary>
    /// Gets or sets the short name of the document.
    /// </summary>
    [Display(Name = "Short Name", Order = 4)]
    public string? DocumentShortName { get; set; }

    /// <summary>
    /// Gets or sets the name of the file.
    /// </summary>
    [Display(Name = "File Name", Order = 6)]
    public string? FileName { get; set; }

    /// <summary>
    /// Gets or sets the is active.
    /// </summary>
    [Display(Name = "Is Active?", Order = 10)]
    public bool? IsActive { get; set; }

    /// <summary>
    /// Gets or sets the is root.
    /// </summary>
    [Display(Name = "Is Root?", Order = 11)]
    public bool? IsRoot { get; set; }

    /// <summary>
    /// Gets or sets the modification date.
    /// </summary>
    [Display(Name = "Modification Date", Order = 8)]
    public DateTime? ModificationDate { get; set; }

    /// <summary>
    /// Gets or sets the end date.
    /// </summary>
    [Display(Name = "End Date", Order = 9)]
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets or sets the path.
    /// </summary>
    [Display(Name = "Path", Order = 5)]
    public string? Path { get; set; }

    /// <summary>
    /// Gets or sets the segment identifier.
    /// </summary>
    [Display(Name = "Segment ID", Order = 0)]
    public int? SegmentId { get; set; }

    /// <summary>
    /// Gets or sets the sort ordinal.
    /// </summary>
    [Display(Name = "Sort Ordinal", Order = 6)]
    public byte? SortOrdinal { get; set; }

    /// <summary>
    /// Gets or sets the tag.
    /// </summary>
    [Display(Name = "Document Tag", Order = 12)]
    public string? Tag { get; set; }

    /// <summary>
    /// Gets or sets the template identifier.
    /// </summary>
    [Display(Name = "XSL Template", Order = 13)]
    public int? TemplateId { get; set; }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    [Display(Name = "Document Title", Order = 3)]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the segment.
    /// </summary>
    [Display(AutoGenerateField = false)]
    public Segment? Segment { get; set; }

    /// <summary>
    /// Gets or sets the fragments.
    /// </summary>
    [Display(AutoGenerateField = false)]
    public ICollection<Fragment> Fragments { get; init; } = new List<Fragment>();

    /// <summary>
    /// Gets or sets the collection of <see cref="IndexKeyword" />.
    /// </summary>
    [Display(AutoGenerateField = false)]
    public ICollection<IndexKeyword> IndexKeywords { get; init; } = new List<IndexKeyword>();

    /// <summary>
    /// Gets or sets the collection of <see cref="ResponsiveImage" />.
    /// </summary>
    [Display(AutoGenerateField = false)]
    public ICollection<ResponsiveImage> ResponsiveImages { get; init; } = new List<ResponsiveImage>();

    /// <summary>
    /// Converts the <see cref="Document"/> into a string.
    /// </summary>
    public override string ToString()
    {
        return this.ToDisplayText();
    }

    private string? _clientId;
    private int? _documentId;
}
