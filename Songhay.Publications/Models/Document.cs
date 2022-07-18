using System.ComponentModel.DataAnnotations;

namespace Songhay.Publications.Models;

/// <summary>
/// Publications Document
/// </summary>
public class Document : IDocument
{
    /// <summary>
    /// Gets or sets the client identifier.
    /// </summary>
    /// <value>
    /// The client identifier.
    /// </value>
    [Display(Name = "Client ID", Order = 2)]
    public string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the incept date.
    /// </summary>
    /// <value>
    /// The incept date.
    /// </value>
    [Display(Name = "Incept Date", Order = 7)]
    public DateTime? InceptDate { get; set; }

    /// <summary>
    /// Gets or sets the document identifier.
    /// </summary>
    /// <value>
    /// The document identifier.
    /// </value>
    [Display(Name = "Document ID", Order = 1)]
    public int? DocumentId { get; set; }

    /// <summary>
    /// Gets or sets the short name of the document.
    /// </summary>
    /// <value>
    /// The short name of the document.
    /// </value>
    [Display(Name = "Short Name", Order = 4)]
    public string DocumentShortName { get; set; }

    /// <summary>
    /// Gets or sets the name of the file.
    /// </summary>
    /// <value>
    /// The name of the file.
    /// </value>
    [Display(Name = "File Name", Order = 6)]
    public string FileName { get; set; }

    /// <summary>
    /// Gets or sets the is active.
    /// </summary>
    /// <value>
    /// The is active.
    /// </value>
    [Display(Name = "Is Active?", Order = 10)]
    public bool? IsActive { get; set; }

    /// <summary>
    /// Gets or sets the is root.
    /// </summary>
    /// <value>
    /// The is root.
    /// </value>
    [Display(Name = "Is Root?", Order = 11)]
    public bool? IsRoot { get; set; }

    /// <summary>
    /// Gets or sets the modification date.
    /// </summary>
    /// <value>
    /// The modification date.
    /// </value>
    [Display(Name = "Modification Date", Order = 8)]
    public DateTime? ModificationDate { get; set; }

    /// <summary>
    /// Gets or sets the end date.
    /// </summary>
    /// <value>
    /// The end date.
    /// </value>
    [Display(Name = "End Date", Order = 9)]
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets or sets the path.
    /// </summary>
    /// <value>
    /// The path.
    /// </value>
    [Display(Name = "Path", Order = 5)]
    public string Path { get; set; }

    /// <summary>
    /// Gets or sets the segment identifier.
    /// </summary>
    /// <value>
    /// The segment identifier.
    /// </value>
    [Display(Name = "Segment ID", Order = 0)]
    public int? SegmentId { get; set; }

    /// <summary>
    /// Gets or sets the sort ordinal.
    /// </summary>
    /// <value>
    /// The sort ordinal.
    /// </value>
    [Display(Name = "Sort Ordinal", Order = 6)]
    public byte? SortOrdinal { get; set; }

    /// <summary>
    /// Gets or sets the tag.
    /// </summary>
    /// <value>
    /// The tag.
    /// </value>
    [Display(Name = "Document Tag", Order = 12)]
    public string Tag { get; set; }

    /// <summary>
    /// Gets or sets the template identifier.
    /// </summary>
    /// <value>
    /// The template identifier.
    /// </value>
    [Display(Name = "XSL Template", Order = 13)]
    public int? TemplateId { get; set; }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    [Display(Name = "Document Title", Order = 3)]
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the segment.
    /// </summary>
    /// <value>
    /// The segment.
    /// </value>
    [Display(AutoGenerateField = false)]
    public virtual Segment Segment { get; set; }

    /// <summary>
    /// Gets or sets the fragments.
    /// </summary>
    /// <value>
    /// The fragments.
    /// </value>
    [Display(AutoGenerateField = false)]
    public ICollection<Fragment> Fragments { get; set; } = new List<Fragment>();

    /// <summary>
    /// Gets or sets the collection of <see cref="IndexKeyword" />.
    /// </summary>
    /// <value>
    /// The collection of <see cref="IndexKeyword" />.
    /// </value>
    [Display(AutoGenerateField = false)]
    public ICollection<IndexKeyword> IndexKeywords { get; set; } = new List<IndexKeyword>();

    /// <summary>
    /// Gets or sets the collection of <see cref="ResponsiveImage" />.
    /// </summary>
    /// <value>
    /// The collection of <see cref="ResponsiveImage" />.
    /// </value>
    [Display(AutoGenerateField = false)]
    public ICollection<ResponsiveImage> ResponsiveImages { get; set; } = new List<ResponsiveImage>();

    /// <summary>
    /// Converts the <see cref="Document"/> into a string.
    /// </summary>
    public override string ToString()
    {
        return this.ToDisplayText();
    }
}