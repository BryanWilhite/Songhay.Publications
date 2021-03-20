using System;
using System.Collections.Generic;
using System.Linq;

namespace Songhay.Publications.Models
{
    /// <summary>
    /// Defines a responsive image
    /// </summary>
    public class ResponsiveImage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponsiveImage"/> class.
        /// </summary>
        public ResponsiveImage()
        {
            this.Candidates = Enumerable.Empty<ImageCandidate>().ToList();
            this.Sizes = Enumerable.Empty<ImageSize>().ToList();
        }

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
