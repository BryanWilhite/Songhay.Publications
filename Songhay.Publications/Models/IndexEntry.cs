using System;
using System.Linq;

namespace Songhay.Publications.Models
{
    /// <summary>
    /// Implements <see cref="IIndexEntry"/>.
    /// </summary>
    /// <remarks>
    /// The opinion here is that this class demonstrates
    /// the advantages of Typescript over a “classic” OOP language.
    /// 
    /// The ability to cast an anonymous object into an interface
    /// would eliminate the need for this class.
    /// 
    /// For more detail, see https://github.com/dotnet/roslyn/issues/13#issuecomment-70338359
    /// </remarks>
    public class IndexEntry : IIndexEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublicationContext"/> class.
        /// </summary>
        /// <param name="data"></param>
        public IndexEntry(Segment data)
        {
            if(data == null) throw new ArgumentNullException(nameof(data));

            Documents = data.Documents.ToArray();

            ClientId = data.ClientId;
            EndDate = data.EndDate;
            InceptDate = data.InceptDate;
            IsActive = data.IsActive;
            ModificationDate = data.ModificationDate;
            ParentSegmentId = data.ParentSegmentId;
            SegmentId = data.SegmentId;
            SegmentName = data.SegmentName;
            SortOrdinal = data.SortOrdinal;
        }

        /// <summary>
        /// Gets or sets child segments.
        /// </summary>
        /// <value>
        /// The child segments.
        /// </value>
        public IIndexEntry[] Segments { get; set; }

        /// <summary>
        /// Gets or sets the documents.
        /// </summary>
        /// <value>
        /// The documents.
        /// </value>
        public IDocument[] Documents { get; set; }

        /// <summary>
        /// Gets or sets the segment identifier.
        /// </summary>
        /// <value>
        /// The segment identifier.
        /// </value>
        public int? SegmentId { get; set; }

        /// <summary>
        /// Gets or sets the name of the segment.
        /// </summary>
        /// <value>
        /// The name of the segment.
        /// </value>
        public string SegmentName { get; set; }

        /// <summary>
        /// Gets or sets the sort ordinal.
        /// </summary>
        /// <value>
        /// The sort ordinal.
        /// </value>
        public byte? SortOrdinal { get; set; }

        /// <summary>
        /// Gets or sets the parent segment identifier.
        /// </summary>
        /// <value>
        /// The parent segment identifier.
        /// </value>
        public int? ParentSegmentId { get; set; }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the is active.
        /// </summary>
        /// <value>
        /// The is active.
        /// </value>
        public bool? IsActive { get; set; }

        /// <summary>
        /// End/expiration <see cref="DateTime"/> of the item.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Origin <see cref="DateTime"/> of the item.
        /// </summary>
        public DateTime? InceptDate { get; set; }

        /// <summary>
        /// Modification/editorial <see cref="DateTime"/> of the item.
        /// </summary>
        public DateTime? ModificationDate { get; set; }
    }
}