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
        /// Finds the specified sub <see cref="DirectoryInfo"/>
        /// under the specified <see cref="DirectoryInfo"/>.
        /// </summary>
        /// <param name="directoryInfo">the specified <see cref="DirectoryInfo"/></param>
        /// <param name="expectedDirectoryName">the specified sub <see cref="DirectoryInfo.Name"/></param>
        /// <returns></returns>
        public static DirectoryInfo FindDirectory(this DirectoryInfo directoryInfo, string expectedDirectoryName)
        {//TODO: move to Core
            if (directoryInfo == null)
                throw new DirectoryNotFoundException("The expected directory is not here.");

            if (!directoryInfo.Exists)
                throw new DirectoryNotFoundException("The expected directory does not exist.");

            var subDirectoryInfo = directoryInfo.GetDirectories(expectedDirectoryName).FirstOrDefault();

            if (subDirectoryInfo == null)
                throw new DirectoryNotFoundException("The expected directory is not here.");

            return subDirectoryInfo;
        }

        /// <summary>
        /// Finds the specified <see cref="FileInfo"/>
        /// under the specified <see cref="DirectoryInfo"/>.
        /// </summary>
        /// <param name="directoryInfo">the specified <see cref="DirectoryInfo"/></param>
        /// <param name="expectedFileName">the specified <see cref="FileInfo.Name"/></param>
        /// <returns></returns>
        public static FileInfo FindFile(this DirectoryInfo directoryInfo, string expectedFileName)
        {//TODO: move to Core
            if (directoryInfo == null)
                throw new DirectoryNotFoundException("The expected directory is not here.");

            if (!directoryInfo.Exists)
                throw new DirectoryNotFoundException("The expected directory does not exist.");

            var fileInfo = directoryInfo.GetFiles(expectedFileName).FirstOrDefault();

            if (fileInfo == null)
                throw new FileNotFoundException("The expected file is not here.");

            return fileInfo;
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

        /// <summary>
        /// Verifies the specified <see cref="DirectoryInfo"/>
        /// with conventional error handling.
        /// </summary>
        /// <param name="directoryInfo">the specified <see cref="DirectoryInfo"/></param>
        /// <param name="expectedDirectoryName">the expected directory name</param>
        /// <returns></returns>
        public static void VerifyDirectory(this DirectoryInfo directoryInfo, string expectedDirectoryName)
        {//TODO: move to Core
            if (directoryInfo == null)
                throw new DirectoryNotFoundException("The expected directory is not here.");

            if (!directoryInfo.Exists)
                throw new DirectoryNotFoundException("The expected directory does not exist.");

            if (!directoryInfo.Name.EqualsInvariant(expectedDirectoryName))
                throw new DirectoryNotFoundException($"The expected directory is not here. [actual: { expectedDirectoryName ?? "[name]" }");
        }
    }
}
