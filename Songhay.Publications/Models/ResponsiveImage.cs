namespace Songhay.Publications.Models;

/// <summary>
/// Defines a responsive image.
/// </summary>
public class ResponsiveImage
{
    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the key.
    /// </summary>
    [DisallowNull]
    public string? Key
    {
        get;
        set => field = value.ToReferenceTypeValueOrThrow();
    }

    /// <summary>
    /// Gets or sets the source.
    /// </summary>
    [DisallowNull]
    public Uri? Source
    {
        get;
        set => field = value.ToReferenceTypeValueOrThrow();
    }

    /// <summary>
    /// Gets or sets the candidates.
    /// </summary>
    public ICollection<ImageCandidate> Candidates { get; init; } = [];

    /// <summary>
    /// Gets or sets the sizes.
    /// </summary>
    public ICollection<ImageSize> Sizes { get; init; } = [];
}
