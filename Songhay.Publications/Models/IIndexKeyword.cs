namespace Songhay.Publications.Models
{
    /// <summary>
    /// Generic Web Index Keyword interface
    /// </summary>
    public interface IIndexKeyword
    {
        /// <summary>
        /// Gets or sets the document identifier.
        /// </summary>
        /// <value>
        /// The document identifier.
        /// </value>
        int? DocumentId { get; set; }

        /// <summary>
        /// Gets or sets the document identifier.
        /// </summary>
        /// <value>
        /// The document identifier.
        /// </value>
        string DocumentClientId { get; set; }

        /// <summary>
        /// Gets or sets the keyword value.
        /// </summary>
        /// <value>
        /// The keyword value.
        /// </value>
        string KeywordValue { get; set; }
    }
}
