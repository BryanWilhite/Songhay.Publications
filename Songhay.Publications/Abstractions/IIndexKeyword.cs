namespace Songhay.Publications.Abstractions;

/// <summary>
/// Publication Index Keyword
/// </summary>
public interface IIndexKeyword : ITemporal
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
    /// Gets or sets the keyword value.
    /// </summary>
    string? KeywordValue { get; set; }
}
