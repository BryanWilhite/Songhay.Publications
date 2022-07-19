namespace Songhay.Publications.Abstractions;

/// <summary>
/// Publication Index Keyword
/// </summary>
public interface IIndexKeyword : ITemporal
{
    /// <summary>
    /// Index Keyword Group ID
    /// </summary>
    /// <value></value>
    int? Id { get; set; }

    /// <summary>
    /// Index Keyword Group Client ID
    /// </summary>
    /// <value></value>
    string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the keyword value.
    /// </summary>
    /// <value>
    /// The keyword value.
    /// </value>
    string KeywordValue { get; set; }
}