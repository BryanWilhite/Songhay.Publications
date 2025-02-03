namespace Songhay.Publications.Abstractions;

/// <summary>
/// Publication Index Keyword Group
/// </summary>
public interface IIndexKeywordGroup : ITemporal
{
    /// <summary>
    /// Index Keyword Group ID
    /// </summary>
    [DisallowNull]
    int? Id { get; set; }

    /// <summary>
    /// Index Keyword Group Client ID
    /// </summary>
    [DisallowNull]
    string? ClientId { get; set; }

    /// <summary>
    /// Index Keyword Group Name
    /// </summary>
    string? Name { get; set; }
}
