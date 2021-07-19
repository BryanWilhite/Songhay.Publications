using System;

namespace Songhay.Publications.Models
{
    /// <summary>
    /// Defines the Search Index Entry
    /// </summary>
    public class SearchIndexEntry : Document
    {
        /// <summary>
        /// The <see cref="Document"/> Extract
        /// </summary>
        /// <value></value>
        public string Extract { get; set; }
    }
}