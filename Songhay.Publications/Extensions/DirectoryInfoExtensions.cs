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
