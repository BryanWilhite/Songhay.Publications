using System.Diagnostics.CodeAnalysis;

namespace Songhay.Publications.Models;

/// <summary>
/// Publications keyword.
/// </summary>
/// <seealso cref="IIndexKeyword" />
public class IndexKeyword : IIndexKeyword
{
    /// <summary>
    /// Index Keyword Group ID.
    /// </summary>
    [DisallowNull]
    public int? Id { get => _id; set => _id = value.ToValueOrThrow(); }

    /// <summary>
    /// Index Keyword Group Client ID.
    /// </summary>
    [DisallowNull]
    public string? ClientId { get => _clientId; set => _clientId = value.ToReferenceTypeValueOrThrow(); }

    /// <summary>
    /// Gets or sets the keyword value.
    /// </summary>
    public string? KeywordValue { get; set; }

    /// <summary>
    /// Collection of Publication Index Keyword Group.
    /// </summary>
    public ICollection<IndexKeywordGroup> Groups { get; init; } = new List<IndexKeywordGroup>();

    /// <summary>
    /// Collection of Publication Index Keyword Group.
    /// </summary>
    /// <value></value>
    public ICollection<Document> Documents { get; init; } = new List<Document>();

    /// <summary>
    /// Gets or sets the incept date.
    /// </summary>
    /// <value>
    /// The incept date.
    /// </value>
    public DateTime? InceptDate { get; set; }

    /// <summary>
    /// Gets or sets the end date.
    /// </summary>
    /// <value>
    /// The end date.
    /// </value>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets or sets the modification date.
    /// </summary>
    /// <value>
    /// The modification date.
    /// </value>
    public DateTime? ModificationDate { get; set; }

    int? _id;
    string? _clientId;
}
