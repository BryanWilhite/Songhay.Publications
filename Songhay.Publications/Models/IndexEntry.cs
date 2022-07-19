using System.Diagnostics.CodeAnalysis;

namespace Songhay.Publications.Models;

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
    public IndexEntry() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PublicationContext"/> class.
    /// </summary>
    /// <param name="data">The <see cref="Segment"/> data.</param>
    public IndexEntry(Segment data)
    {
        if(data == null) throw new ArgumentNullException(nameof(data));

        Documents = data.Documents.OfType<IDocument>().ToArray();

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
    public IIndexEntry[] Segments { get; set; } = Enumerable.Empty<IIndexEntry>().ToArray();

    /// <summary>
    /// Gets or sets the documents.
    /// </summary>
    public IDocument[] Documents { get; set; } = Enumerable.Empty<IDocument>().ToArray();

    /// <summary>
    /// Gets or sets the segment identifier.
    /// </summary>
    public int? SegmentId { get; set; }

    /// <summary>
    /// Gets or sets the name of the segment.
    /// </summary>
    [DisallowNull]
    public string? SegmentName
    {
        get => _segmentName;
        set => _segmentName = value.ToReferenceTypeValueOrThrow();
    }

    /// <summary>
    /// Gets or sets the sort ordinal.
    /// </summary>
    public byte? SortOrdinal { get; set; }

    /// <summary>
    /// Gets or sets the parent segment identifier.
    /// </summary>
    public int? ParentSegmentId { get; set; }

    /// <summary>
    /// Gets or sets the client identifier.
    /// </summary>
    [DisallowNull]
    public string? ClientId
    {
        get => _clientId;
        set => _clientId = value.ToReferenceTypeValueOrThrow();
    }

    /// <summary>
    /// Gets or sets the is active.
    /// </summary>
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

    string? _segmentName;
    string? _clientId;
}
