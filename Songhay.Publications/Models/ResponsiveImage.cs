using System;
using System.Collections.Generic;

namespace Songhay.Publications.Models
{
    /// <summary>
    /// Defines a responsive image
    /// </summary>
    public class ResponsiveImage
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public Uri Source { get; set; }

        /// <summary>
        /// Gets or sets the candidates.
        /// </summary>
        /// <value>
        /// The candidates.
        /// </value>
        public ICollection<ImageCandidate> Candidates { get; set; }

        /// <summary>
        /// Gets or sets the sizes.
        /// </summary>
        /// <value>
        /// The sizes.
        /// </value>
        public ICollection<ImageSize> Sizes { get; set; }
    }
}
