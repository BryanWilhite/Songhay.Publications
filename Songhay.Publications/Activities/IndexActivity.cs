using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Songhay.Extensions;
using Songhay.Models;
using Songhay.Publications.Extensions;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Songhay.Publications.Activities
{
    /// <summary>
    /// <see cref="IActivity"/> implementation for  Publication Indexes
    /// </summary>
    /// <seealso cref="IActivity" />
    /// <seealso cref="IActivityConfigurationSupport" />
    public class IndexActivity : IActivity
    {
        /// <summary>
        /// Displays the help.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public string DisplayHelp(ProgramArgs args)
        {
            throw new NotImplementedException();
        }

        /// <summary>Starts with the specified arguments.</summary>
        /// <param name="args">The arguments.</param>
        public void Start(ProgramArgs args)
        {
            throw new NotImplementedException();
        }

        internal static FileInfo CompressIndex(FileInfo indexInfo)
        {
            if (indexInfo == null) throw new ArgumentNullException(nameof(indexInfo));

            var compressedIndexInfo = new FileInfo(indexInfo.FullName.Replace(".json", ".c.json"));

            using (FileStream fileStream = indexInfo.OpenRead())
            {
                using (FileStream compressedFileStream = File.Create(compressedIndexInfo.FullName))
                {
                    using (GZipStream gZipStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
                    {
                        fileStream.CopyTo(gZipStream);
                    }
                }
            }

            return compressedIndexInfo;
        }

        internal static FileInfo GenerateIndexFrom11tyEntries(DirectoryInfo entryRootInfo, DirectoryInfo jsonRootInfo, string indexFileName)
        {
            if (entryRootInfo == null) throw new ArgumentNullException(nameof(entryRootInfo));
            if (jsonRootInfo == null) throw new ArgumentNullException(nameof(jsonRootInfo));
            if (string.IsNullOrEmpty(indexFileName)) throw new ArgumentNullException(nameof(indexFileName));

            var frontMatterDocuments = entryRootInfo
                .GetFiles("*.md", SearchOption.AllDirectories)
                .Select(fileInfo => fileInfo.ToMarkdownEntry().FrontMatter)
                .Select(jO => JObject.FromObject(new
                {
                    extract = JObject.Parse(jO.GetValue<string>("tag")).GetValue<string>("extract"),
                    clientId = jO.GetValue<string>("clientId"),
                    inceptDate = jO.GetValue<string>("date"),
                    modificationDate = jO.GetValue<string>("modificationDate"),
                    title = jO.GetValue<string>("title")
                }))
                .ToArray();

            var jA = new JArray(frontMatterDocuments);
            var targetInfo = jsonRootInfo.FindFile(indexFileName);

            File.WriteAllText(targetInfo.FullName, jA.ToString());

            return targetInfo;
        }
    }
}
