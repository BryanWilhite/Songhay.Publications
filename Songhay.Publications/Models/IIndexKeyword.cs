namespace Songhay.Publications.Models
{
    /// <summary>
    /// Publication Index Keyword
    /// </summary>
    public interface IIndexKeyword : ITemporal
    {
        /// <summary>
        /// Gets or sets the keyword value.
        /// </summary>
        /// <value>
        /// The keyword value.
        /// </value>
        string KeywordValue { get; set; }
    }
}
