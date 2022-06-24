using System;
using System.Linq;
using Songhay.Publications.Models;

namespace Songhay.Publications.Extensions
{
    /// <summary>
    /// Extensions of <see cref="ResponsiveImage" />
    /// </summary>
    public static class ResponsiveImageExtensions
    {
        /// <summary>
        /// Returns CSS <c>@media</c> At-rule blocks.
        /// </summary>
        /// <param name="responsiveImage"><see cref="ResponsiveImage" /></param>
        /// <returns></returns>
        public static string ToCssMediaAtRules(this ResponsiveImage responsiveImage)
        {
            responsiveImage.EnsureResponsiveImage();

            var candidatesCollection = responsiveImage
                .Candidates
                .Select(i => $"background-image: url({i.ImageUri?.OriginalString});")
                .ToArray();

            if (!candidatesCollection.Any()) return string.Empty;

            var sizesCollection = responsiveImage
                .Sizes
                .Select(i => $"@media only screen and {i.MediaCondition}")
                .ToArray();

            if (!sizesCollection.Any()) return string.Empty;

            var stringCollection = sizesCollection
                .Zip(candidatesCollection, (media, background) => $@"
{media} {{{
    spacer}{background}
}}");
            return string.Join(string.Empty,
                new[] {
                    $"/* {responsiveImage.Description ?? string.Empty} */"
                }.Concat(stringCollection)
            );
        }

        /// <summary>
        /// Returns <c>img</c> markup with <c>srcset</c> and <c>sizes</c>.
        /// </summary>
        /// <param name="responsiveImage"><see cref="ResponsiveImage" /></param>
        /// <returns></returns>
        public static string ToImgMarkup(this ResponsiveImage responsiveImage)
        {
            responsiveImage.EnsureResponsiveImage();

            var srcset = responsiveImage.ToSrcSetAttribute();
            var sizes = responsiveImage.ToSizesSetAttribute();

            return $@"
<img{
    spacer}alt=""{responsiveImage.Description ?? string.Empty}""{
    spacer}src=""{responsiveImage.Source?.OriginalString ?? string.Empty}""{srcset}{sizes}>
";
        }

        /// <summary>
        /// Reduces <see cref="ResponsiveImage.Sizes" /> to the <c>sizes</c> attribute.
        /// </summary>
        /// <param name="responsiveImage"><see cref="ResponsiveImage" /></param>
        /// <returns></returns>
        public static string ToSizesSetAttribute(this ResponsiveImage responsiveImage)
        {
            responsiveImage.EnsureResponsiveImage();

            var collection = responsiveImage
                .Sizes
                .Select(i => $"({i.MediaCondition}) {i.LayoutWidth}")
                .ToArray();

            if (!collection.Any()) return string.Empty;

            var attributeValue = collection.Aggregate((a, i) => $"{a},{spacer}{i}");

            return $@"{spacer}sizes=""{attributeValue}""";
        }

        /// <summary>
        /// Reduces <see cref="ResponsiveImage.Candidates" /> to the <c>srcset</c> attribute.
        /// </summary>
        /// <param name="responsiveImage"><see cref="ResponsiveImage" /></param>
        /// <returns></returns>
        public static string ToSrcSetAttribute(this ResponsiveImage responsiveImage)
        {
            responsiveImage.EnsureResponsiveImage();

            var collection = responsiveImage
                .Candidates
                .Select(i => $"{i.ImageUri} {i.Width ?? i.PixelDensity}")
                .ToArray();

            if (!collection.Any()) return string.Empty;

            var attributeValue = collection.Aggregate((a, i) => $"{a},{spacer}{i}");

            return $@"{spacer}srcset=""{attributeValue}""";
        }

        static void EnsureResponsiveImage(this ResponsiveImage responsiveImage)
        {
            if (responsiveImage == null) throw new ArgumentNullException(nameof(responsiveImage));
        }

        static readonly string spacer = $"{Environment.NewLine}    ";
    }
}
