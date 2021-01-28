using Songhay.Publications.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Songhay.Publications.Models
{
    /// <summary>
    /// GenericWeb Segment
    /// </summary>
    public partial class Segment : ISegment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Segment"/> class.
        /// </summary>
        public Segment()
        {
            this.Documents = new List<Document>();
            this.ChildSegments = new List<Segment>();
        }

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
        /// Gets or sets the modification date.
        /// </summary>
        /// <value>
        /// The modification date.
        /// </value>
        [Display(Name = "Modification Date", Order = 8)]
        [Required]
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
        public virtual ICollection<Document> Documents { get; set; }

        /// <summary>
        /// Gets or sets the segments of parent.
        /// </summary>
        /// <value>
        /// The segments of parent.
        /// </value>
        [Display(AutoGenerateField = false)]
        public virtual ICollection<Segment> ChildSegments { get; set; }

        /// <summary>
        /// Gets or sets the parent segment.
        /// </summary>
        /// <value>
        /// The parent segment.
        /// </value>
        [Display(AutoGenerateField = false)]
        public virtual Segment ParentSegment { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.ToDisplayText();
        }
    }
}
