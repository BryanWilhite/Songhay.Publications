namespace Songhay.Publications.Models;

/// <summary>
/// Defines a responsive-image candidate.
/// </summary>
/// <remarks>
/// “`srcset` is a string which identifies one
/// or more image candidate strings, separated using commas (,)
/// each specifying image resources to use under given circumstances.”
/// MDN [ 📖 https://developer.mozilla.org/en-US/docs/Web/API/HTMLImageElement/srcset ]
/// </remarks>
public class ImageCandidate
{
    /// <summary>
    /// Gets or sets the image URI.
    /// </summary>
    [DisallowNull]
    public Uri? ImageUri { get => _imageUri; set => _imageUri = value.ToReferenceTypeValueOrThrow(); }

    /// <summary>
    /// Gets or sets the pixel density.
    /// </summary>
    public string? PixelDensity { get; set; }

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    public string? Width { get; set; }

    Uri? _imageUri;
}
