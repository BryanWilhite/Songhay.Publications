using System;
using System.ComponentModel.DataAnnotations;

namespace Songhay.Publications.Models
{
    /// <summary>
    /// Defines <see cref="Segment"/>
    /// attributes for validation and display.
    /// </summary>
    /// <seealso cref="Songhay.Publications.Models.ISegment" />
    public class SegmentMetadata : ISegment
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
        [Display(Name = "Incept Date", Order = 5)]
        [Required]
        public DateTime? InceptDate { get; set; }

        /// <summary>
        /// Gets or sets the is active.
        /// </summary>
        /// <value>
        /// The is active.
        /// </value>
        [Display(Name = "Is Active?", Order = 4)]
        [Required]
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
        public int SegmentId { get; set; }

        /// <summary>
        /// Gets or sets the name of the segment.
        /// </summary>
        /// <value>
        /// The name of the segment.
        /// </value>
        [Display(Name = "Segment Name", Order = 3)]
        [Required]
        public string SegmentName { get; set; }

        /// <summary>
        /// Gets or sets the sort ordinal.
        /// </summary>
        /// <value>
        /// The sort ordinal.
        /// </value>
        [Display(Name = "Sort Ordinal", Order = 6)]
        public byte? SortOrdinal { get; set; }
    }
}
