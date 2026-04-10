namespace Songhay.Publications.Models;

/// <summary>
/// Publication Index Keyword.
/// </summary>
public class IndexKeywordGroup : IIndexKeywordGroup
{
    /// <summary>
    /// Index Keyword Group ID.
    /// </summary>
    [DisallowNull]
    public int? Id
    {
        get;
        set => field = value.ToValueOrThrow();
    }

    /// <summary>
    /// Index Keyword Group Client ID.
    /// </summary>
    [DisallowNull]
    public string? ClientId
    {
        get;
        set => field = value.ToReferenceTypeValueOrThrow();
    }

    /// <summary>
    /// Index Keyword Group Name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the incept date.
    /// </summary>
    public DateTime? InceptDate { get; set; }

    /// <summary>
    /// Gets or sets the end date.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets or sets the modification date.
    /// </summary>
    public DateTime? ModificationDate { get; set; }
}
