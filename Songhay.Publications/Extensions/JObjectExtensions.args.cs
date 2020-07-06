using Newtonsoft.Json.Linq;
using Songhay.Extensions;
using Songhay.Publications.Models;
using System;
using System.IO;

namespace Songhay.Publications.Extensions
{
    /// <summary>
    /// Extensions of <see cref="JObject"/>
    /// </summary>
    public static partial class JObjectExtensions
    {
        /// <summary>
        /// Gets the arguments for Activity method.
        /// </summary>
        /// <param name="jObject">The <see cref="JObject"/>.</param>
        /// <param name="presentationInfo">The presentation information.</param>
        /// <returns></returns>
        public static string GetAddEntryExtractArg(this JObject jObject, DirectoryInfo presentationInfo)
        {
            var entryPath = jObject.GetValue<string>("entryPath");
            entryPath = presentationInfo.ToCombinedPath(entryPath);

            return entryPath;
        }

        /// <summary>
        /// Gets the arguments for Activity method.
        /// </summary>
        /// <param name="jObject">The <see cref="JObject" />.</param>
        /// <param name="presentationInfo">The presentation information.</param>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException">The expected index root, {indexRoot}, is not here.</exception>
        public static (
            DirectoryInfo entryRootInfo,
            DirectoryInfo indexRootInfo,
            string indexFileName
            ) GetCompressed11tyIndexArgs(this JObject jObject, DirectoryInfo presentationInfo)
        {
            var indexRoot = jObject.GetValue<string>("indexRoot");
            var indexRootInfo = new DirectoryInfo(indexRoot);
            if (!indexRootInfo.Exists) throw new DirectoryNotFoundException($"The expected index root, {indexRoot}, is not here.");

            var indexFileName = jObject.GetValue<string>("indexFileName");
            var entryRootInfo = presentationInfo.FindDirectory("presentation").FindDirectory("entry");

            return (entryRootInfo, indexRootInfo, indexFileName);
        }

        /// <summary>
        /// Gets the arguments for Activity method.
        /// </summary>
        /// <param name="jObject">The <see cref="JObject"/>.</param>
        /// <param name="presentationInfo">The presentation information.</param>
        /// <returns></returns>
        public static (
            string entryPath,
            string collapsedHost
            ) GetExpandUrisArgs(this JObject jObject, DirectoryInfo presentationInfo)
        {
            var collapsedHost = jObject.GetValue<string>("collapsedHost");
            var entryPath = jObject.GetValue<string>("entryPath");
            entryPath = presentationInfo.ToCombinedPath(entryPath);

            return (entryPath, collapsedHost);
        }

        /// <summary>
        /// Gets the arguments for Activity method.
        /// </summary>
        /// <param name="jObject">The <see cref="JObject"/>.</param>
        /// <param name="presentationInfo">The presentation information.</param>
        /// <returns></returns>
        public static (
            DirectoryInfo entryDraftsRootInfo,
            string title
            ) GetGenerateEntryArgs(this JObject jObject, DirectoryInfo presentationInfo)
        {
            var entryDraftsRootInfo = presentationInfo.FindDirectory(MarkdownPresentationDirectories.DirectoryNamePresentationDrafts);
            var title = jObject.GetValue<string>("title");

            return (entryDraftsRootInfo, title);
        }

        /// <summary>
        /// Gets the arguments for Activity method.
        /// </summary>
        /// <param name="jObject">The <see cref="JObject"/>.</param>
        /// <param name="presentationInfo">The presentation information.</param>
        public static (
            DirectoryInfo entryDraftsRootInfo,
            DirectoryInfo entryRootInfo,
            string entryFileName
            ) GetPublishEntryArgs(this JObject jObject, DirectoryInfo presentationInfo)
        {
            var entryDraftsRootInfo = presentationInfo
                .FindDirectory(MarkdownPresentationDirectories.DirectoryNamePresentationDrafts);
            var entryRootInfo = presentationInfo
                .FindDirectory(MarkdownPresentationDirectories.DirectoryNamePresentation)
                .FindDirectory("entry")
                .FindDirectory(DateTime.Now.Year.ToString());
            var entryFileName = jObject.GetValue<string>("entryFileName");

            return (entryDraftsRootInfo, entryRootInfo, entryFileName);
        }
    }
}
