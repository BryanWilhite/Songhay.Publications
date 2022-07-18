﻿namespace Songhay.Publications.Models;

/// <summary>
/// Defines responsive-image sizes.
/// </summary>
/// <remarks>
/// “`sizes` allows you to specify the layout width
/// of the image for each of a list of media conditions.”
/// MDN [ 📖 https://developer.mozilla.org/en-US/docs/Web/API/HTMLImageElement/sizes ]
/// </remarks>
public class ImageSize
{
    /// <summary>
    /// Gets or sets the media condition.
    /// </summary>
    /// <value>
    /// The media condition.
    /// </value>
    public string MediaCondition { get; set; }

    /// <summary>
    /// Gets or sets the width of the layout.
    /// </summary>
    /// <value>
    /// The width of the layout.
    /// </value>
    public string LayoutWidth { get; set; }
}