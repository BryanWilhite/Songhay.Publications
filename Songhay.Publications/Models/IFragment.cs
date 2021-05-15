using Songhay.Models;

namespace Songhay.Publications.Models
{
    /// <summary>
    /// Publications Fragment interface
    /// </summary>
    public interface IFragment : ITemporal
    {
        /// <summary>
        /// Gets or sets the fragment identifier.
        /// </summary>
        /// <value>
        /// The fragment identifier.
        /// </value>
        int? FragmentId { get; set; }

        /// <summary>
        /// Gets or sets the name of the fragment.
        /// </summary>
        /// <value>
        /// The name of the fragment.
        /// </value>
        string FragmentName { get; set; }

        /// <summary>
        /// Gets or sets the display name of the fragment.
        /// </summary>
        /// <value>
        /// The display name of the fragment.
        /// </value>
        string FragmentDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the sort ordinal.
        /// </summary>
        /// <value>
        /// The sort ordinal.
        /// </value>
        byte? SortOrdinal { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        string Content { get; set; }

        /// <summary>
        /// Gets or sets the document identifier.
        /// </summary>
        /// <value>
        /// The document identifier.
        /// </value>
        int? DocumentId { get; set; }

        /// <summary>
        /// Gets or sets the previous fragment identifier.
        /// </summary>
        /// <value>
        /// The previous fragment identifier.
        /// </value>
        int? PrevFragmentId { get; set; }

        /// <summary>
        /// Gets or sets the next fragment identifier.
        /// </summary>
        /// <value>
        /// The next fragment identifier.
        /// </value>
        int? NextFragmentId { get; set; }

        /// <summary>
        /// Gets or sets the is previous.
        /// </summary>
        /// <value>
        /// The is previous.
        /// </value>
        bool? IsPrevious { get; set; }

        /// <summary>
        /// Gets or sets the is next.
        /// </summary>
        /// <value>
        /// The is next.
        /// </value>
        bool? IsNext { get; set; }

        /// <summary>
        /// Gets or sets the is wrapper.
        /// </summary>
        /// <value>
        /// The is wrapper.
        /// </value>
        bool? IsWrapper { get; set; }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the is active.
        /// </summary>
        /// <value>
        /// The is active.
        /// </value>
        bool? IsActive { get; set; }
    }
}
