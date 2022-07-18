namespace Songhay.Publications.Models;

/// <summary>
/// Publication Index Keyword
/// </summary>
public class IndexKeywordGroup : IIndexKeywordGroup
{
    /// <summary>
    /// Index Keyword Group ID
    /// </summary>
    /// <value></value>
    public int? Id { get; set; }

    /// <summary>
    /// Index Keyword Group Client ID
    /// </summary>
    /// <value></value>
    public string ClientId { get; set; }

    /// <summary>
    /// Index Keyword Group Name
    /// </summary>
    /// <value></value>
    public string Name { get; set; }

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
}