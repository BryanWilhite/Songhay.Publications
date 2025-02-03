using System.ComponentModel.DataAnnotations;

namespace Songhay.Publications.Models;

/// <summary>
/// Publications Segment.
/// </summary>
public class Segment : ISegment
{
    /// <summary>
    /// Gets or sets the client identifier.
    /// </summary>
    [Display(Name = "Client ID", Order = 2)]
    [DisallowNull]
    public string? ClientId { get => _clientId; set => _clientId = value.ToReferenceTypeValueOrThrow(); }

    /// <summary>
    /// Gets or sets the is active.
    /// </summary>
    [Display(Name = "Is Active?", Order = 4)]
    public bool? IsActive { get; set; }

    /// <summary>
    /// Gets or sets the parent segment identifier.
    /// </summary>
    [Display(Name = "Parent Segment ID", Order = 0)]
    public int? ParentSegmentId { get; set; }

    /// <summary>
    /// Gets or sets the segment identifier.
    /// </summary>
    [Display(Name = "Segment ID", Order = 1)]
    [DisallowNull]
    public int? SegmentId { get => _segmentId; set => _segmentId = value.ToValueOrThrow(); }

    /// <summary>
    /// Gets or sets the name of the segment.
    /// </summary>
    [Display(Name = "Segment Name", Order = 3)]
    [DisallowNull]
    public string? SegmentName { get => _segmentName; set => _segmentName = value.ToReferenceTypeValueOrThrow(); }

    /// <summary>
    /// Gets or sets the sort ordinal.
    /// </summary>
    [Display(Name = "Sort Ordinal", Order = 6)]
    public byte? SortOrdinal { get; set; }

    /// <summary>
    /// Gets or sets the incept date.
    /// </summary>
    [Display(Name = "Incept Date", Order = 5)]
    public DateTime? InceptDate { get; set; }

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
    /// Gets or sets the documents.
    /// </summary>
    [Display(AutoGenerateField = false)]
    public ICollection<Document> Documents { get; init; } = new List<Document>();

    /// <summary>
    /// Gets or sets child segments.
    /// </summary>
    [Display(AutoGenerateField = false)]
    public ICollection<Segment> Segments { get; init; } = new List<Segment>();

    /// <summary>
    /// Gets or sets the parent segment.
    /// </summary>
    [Display(AutoGenerateField = false)]
    public Segment? ParentSegment { get; set; }

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

    string? _clientId;
    int? _segmentId;
    string? _segmentName;
}
