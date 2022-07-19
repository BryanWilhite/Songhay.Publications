using System.Diagnostics.CodeAnalysis;

namespace Songhay.Publications.Abstractions;

/// <summary>
/// Publications Fragment interface
/// </summary>
public interface IFragment : ITemporal
{
    /// <summary>
    /// Gets or sets the fragment identifier.
    /// </summary>
    [DisallowNull]
    int? FragmentId { get; set; }

    /// <summary>
    /// Gets or sets the name of the fragment.
    /// </summary>
    string? FragmentName { get; set; }

    /// <summary>
    /// Gets or sets the display name of the fragment.
    /// </summary>
    string? FragmentDisplayName { get; set; }

    /// <summary>
    /// Gets or sets the sort ordinal.
    /// </summary>
    byte? SortOrdinal { get; set; }

    /// <summary>
    /// Gets or sets the content.
    /// </summary>
    string? Content { get; set; }

    /// <summary>
    /// Gets or sets the document identifier.
    /// </summary>
    [DisallowNull]
    int? DocumentId { get; set; }

    /// <summary>
    /// Gets or sets the previous fragment identifier.
    /// </summary>
    int? PrevFragmentId { get; set; }

    /// <summary>
    /// Gets or sets the next fragment identifier.
    /// </summary>
    int? NextFragmentId { get; set; }

    /// <summary>
    /// Gets or sets the is previous.
    /// </summary>
    bool? IsPrevious { get; set; }

    /// <summary>
    /// Gets or sets the is next.
    /// </summary>
    bool? IsNext { get; set; }

    /// <summary>
    /// Gets or sets the is wrapper.
    /// </summary>
    bool? IsWrapper { get; set; }

    /// <summary>
    /// Gets or sets the client identifier.
    /// </summary>
    [DisallowNull]
    string? ClientId { get; set; }

    /// <summary>
    /// Gets or sets the is active.
    /// </summary>
    bool? IsActive { get; set; }
}