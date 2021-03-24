using System;
using System.Collections.Generic;

namespace Songhay.Publications.Models
{
    /// <summary>
    /// Publications keyword
    /// </summary>
    /// <seealso cref="IIndexKeyword" />
    public partial class IndexKeyword : IIndexKeyword
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexKeyword"/> class.
        /// </summary>
        public IndexKeyword()
        {
            this.Groups = new List<IndexKeywordGroup>();
            this.Documents = new List<Document>();
        }

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
        /// Gets or sets the keyword value.
        /// </summary>
        /// <value>
        /// The keyword value.
        /// </value>
        public string KeywordValue { get; set; }

        /// <summary>
        /// collection of Publication Index Keyword Group
        /// </summary>
        /// <value></value>
        public ICollection<IndexKeywordGroup> Groups { get; set; }

        /// <summary>
        /// collection of Publication Index Keyword Group
        /// </summary>
        /// <value></value>
        public ICollection<Document> Documents { get; set; }

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
}
