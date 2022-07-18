using System.ComponentModel.DataAnnotations;

namespace Songhay.Publications.Models;

/// <summary>
/// Publications Segment
/// </summary>
public class Segment : ISegment
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
    /// Gets or sets the is active.
    /// </summary>
    /// <value>
    /// The is active.
    /// </value>
    [Display(Name = "Is Active?", Order = 4)]
    public bool? IsActive { get; set; }

    /// <summary>
    /// Gets or sets the parent segment identifier.
    /// </summary>
    /// <value>
    /// The parent segment identifier.
    /// </value>
    [Display(Name = "Parent Segment ID", Order = 0)]
    public int? ParentSegmentId { get; set; }

    /// <summary>
    /// Gets or sets the segment identifier.
    /// </summary>
    /// <value>
    /// The segment identifier.
    /// </value>
    [Display(Name = "Segment ID", Order = 1)]
    public int? SegmentId { get; set; }

    /// <summary>
    /// Gets or sets the name of the segment.
    /// </summary>
    /// <value>
    /// The name of the segment.
    /// </value>
    [Display(Name = "Segment Name", Order = 3)]
    public string SegmentName { get; set; }

    /// <summary>
    /// Gets or sets the sort ordinal.
    /// </summary>
    /// <value>
    /// The sort ordinal.
    /// </value>
    [Display(Name = "Sort Ordinal", Order = 6)]
    public byte? SortOrdinal { get; set; }

    /// <summary>
    /// Gets or sets the incept date.
    /// </summary>
    /// <value>
    /// The incept date.
    /// </value>
    [Display(Name = "Incept Date", Order = 5)]
    public DateTime? InceptDate { get; set; }

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
    /// Gets or sets the documents.
    /// </summary>
    /// <value>
    /// The documents.
    /// </value>
    [Display(AutoGenerateField = false)]
    public ICollection<Document> Documents { get; set; } = new List<Document>();

    /// <summary>
    /// Gets or sets child segments.
    /// </summary>
    /// <value>
    /// The child segments.
    /// </value>
    [Display(AutoGenerateField = false)]
    public ICollection<Segment> Segments { get; set; } = new List<Segment>();

    /// <summary>
    /// Gets or sets the parent segment.
    /// </summary>
    /// <value>
    /// The parent segment.
    /// </value>
    [Display(AutoGenerateField = false)]
    public Segment ParentSegment { get; set; }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="String" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return this.ToDisplayText();
    }
}