using System.Diagnostics.CodeAnalysis;

namespace Songhay.Publications.Abstractions;

/// <summary>
/// Publications Segment interface
/// </summary>
public interface ISegment: ITemporal
{
    /// <summary>
    /// Gets or sets the segment identifier.
    /// </summary>
    [DisallowNull]
    int? SegmentId { get; set; }

    /// <summary>
    /// Gets or sets the name of the segment.
    /// </summary>
    [DisallowNull]
    string? SegmentName { get; set; }

    /// <summary>
    /// Gets or sets the sort ordinal.
    /// </summary>
    byte? SortOrdinal { get; set; }

    /// <summary>
    /// Gets or sets the parent segment identifier.
    /// </summary>
    int? ParentSegmentId { get; set; }

    /// <summary>
    /// Gets or sets the client identifier.
    /// </summary>
    [DisallowNull]
    string? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the is active.
    /// </summary>
    bool? IsActive { get; set; }
}
