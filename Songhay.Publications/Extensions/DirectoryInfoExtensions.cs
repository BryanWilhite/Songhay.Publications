using Songhay.Extensions;
using Songhay.Publications.Models;
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

            void check(string directoryName, DirectoryInfo info)
            {
                if (info == null) throw new DirectoryNotFoundException($"The expected directory, {directoryName}, is not here.");
            }

            var presentation = directoryInfo.GetDirectories(MarkdownPresentationDirectories.DirectoryNamePresentation).FirstOrDefault();
            check(MarkdownPresentationDirectories.DirectoryNamePresentation, presentation);

            var drafts = directoryInfo.GetDirectories(MarkdownPresentationDirectories.DirectoryNamePresentationDrafts).FirstOrDefault();
            check(MarkdownPresentationDirectories.DirectoryNamePresentationDrafts, drafts);

            var shell = directoryInfo.GetDirectories(MarkdownPresentationDirectories.DirectoryNamePresentationShell).FirstOrDefault();
            check(MarkdownPresentationDirectories.DirectoryNamePresentationShell, shell);

            return true;
        }

        /// <summary>
        /// Returns true when all of the specified conventional markdown presentation
        /// directory is present.
        /// </summary>
        /// <param name="directoryInfo">the expected top-level presentation directory</param>
        /// <param name="directoryName"><see cref="MarkdownPresentationDirectories.All"/></param>
        /// <returns></returns>
        public static bool IsConventionalMarkdownPresentationDirectory(this DirectoryInfo directoryInfo, string directoryName)
        {
            if (!directoryInfo.Name.EqualsInvariant(directoryName))
                throw new DirectoryNotFoundException($"The expected Presentation Shell directory is not here. [actual: { directoryInfo?.Name ?? "[name]" }");

            return true;
        }
    }
}
