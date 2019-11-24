using System;

namespace Songhay.Publications.Models
{
    /// <summary>
    /// Generic Web Web Keyword interface
    /// </summary>
    public interface IWebKeyword
    {
        /// <summary>
        /// Gets or sets the document identifier.
        /// </summary>
        /// <value>
        /// The document identifier.
        /// </value>
        int DocumentId { get; set; }

        /// <summary>
        /// Gets or sets the keyword value.
        /// </summary>
        /// <value>
        /// The keyword value.
        /// </value>
        string KeywordValue { get; set; }
    }
}
