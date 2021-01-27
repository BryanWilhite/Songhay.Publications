using System;

namespace Songhay.Publications.Models
{
    /// <summary>
    /// GenericWeb Segment interface
    /// </summary>
    public interface ISegment: ITemporal
    {
        /// <summary>
        /// Gets or sets the segment identifier.
        /// </summary>
        /// <value>
        /// The segment identifier.
        /// </value>
        int SegmentId { get; set; }

        /// <summary>
        /// Gets or sets the name of the segment.
        /// </summary>
        /// <value>
        /// The name of the segment.
        /// </value>
        string SegmentName { get; set; }

        /// <summary>
        /// Gets or sets the sort ordinal.
        /// </summary>
        /// <value>
        /// The sort ordinal.
        /// </value>
        byte? SortOrdinal { get; set; }

        /// <summary>
        /// Gets or sets the parent segment identifier.
        /// </summary>
        /// <value>
        /// The parent segment identifier.
        /// </value>
        int? ParentSegmentId { get; set; }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the is active.
        /// </summary>
        /// <value>
        /// The is active.
        /// </value>
        bool? IsActive { get; set; }
    }
}
