using Songhay.Extensions;
using Songhay.Publications.Models;
using System;
using System.IO;
using System.Linq;

namespace Songhay.Publications.Extensions
{
    /// <summary>
    /// Extensions of <see cref="DirectoryInfo"/>
    /// </summary>
    public static class DirectoryInfoExtensions
    {

        /// <summary>
        /// Gets the absolute or relative path with the specified file segment.
        /// </summary>
        /// <param name="directoryInfo">The base information.</param>
        /// <param name="fileSegment">The file segment.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// baseInfo
        /// or
        /// fileSegment
        /// </exception>
        public static string GetAbsoluteOrRelativePath(this DirectoryInfo directoryInfo, string fileSegment)
        { // TODO: move to Core
            if (directoryInfo == null) throw new ArgumentNullException(nameof(directoryInfo));
            if (string.IsNullOrEmpty(fileSegment)) throw new ArgumentNullException(nameof(fileSegment));

            fileSegment = FrameworkFileUtility.TrimLeadingDirectorySeparatorChars(fileSegment);
            fileSegment = FrameworkFileUtility.NormalizePath(fileSegment);

            return Path.IsPathRooted(fileSegment) ? fileSegment : directoryInfo.ToCombinedPath(fileSegment);
        }

        /// <summary>
        /// Returns true when all of the conventional markdown presentation
        /// directories are present.
        /// </summary>
        /// <param name="directoryInfo">the expected top-level presentation directory</param>
        /// <returns></returns>
        public static bool HasAllConventionalMarkdownPresentationDirectories(this DirectoryInfo directoryInfo)
        {
            if (directoryInfo == null) return false;

            directoryInfo
                .GetDirectories(MarkdownPresentationDirectories.DirectoryNamePresentation)
                .FirstOrDefault()
                .VerifyDirectory(MarkdownPresentationDirectories.DirectoryNamePresentation);

            directoryInfo
                .GetDirectories(MarkdownPresentationDirectories.DirectoryNamePresentationDrafts)
                .FirstOrDefault()
                .VerifyDirectory(MarkdownPresentationDirectories.DirectoryNamePresentationDrafts);

            directoryInfo
                .GetDirectories(MarkdownPresentationDirectories.DirectoryNamePresentationShell)
                .FirstOrDefault()
                .VerifyDirectory(MarkdownPresentationDirectories.DirectoryNamePresentationShell);

            return true;
        }
    }
}
