namespace Songhay.Publications.Models
{
    /// <summary>
    /// GenericWeb keyword
    /// </summary>
    /// <seealso cref="Songhay.Publications.Models.IIndexKeyword" />
    public partial class IndexKeyword : IIndexKeyword
    {
        /// <summary>
        /// Gets or sets the document identifier.
        /// </summary>
        /// <value>
        /// The document identifier.
        /// </value>
        public int? DocumentId { get; set; }

        /// <summary>
        /// Gets or sets the document identifier.
        /// </summary>
        /// <value>
        /// The document identifier.
        /// </value>
        public string DocumentClientId { get; set; }

        /// <summary>
        /// Gets or sets the keyword value.
        /// </summary>
        /// <value>
        /// The keyword value.
        /// </value>
        public string KeywordValue { get; set; }

        /// <summary>
        /// Gets or sets the document.
        /// </summary>
        /// <value>
        /// The document.
        /// </value>
        public virtual Document Document { get; set; }
    }
}
