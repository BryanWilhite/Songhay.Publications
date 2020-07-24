using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Songhay.Diagnostics;
using Songhay.Extensions;
using Songhay.Models;
using Songhay.Publications.Extensions;
using Songhay.Publications.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Songhay.Publications.Activities
{
    /// <summary>
    /// <see cref="IActivity"/> implementation for Publication Indices
    /// </summary>
    /// <seealso cref="IActivity" />
    /// <seealso cref="IActivityConfigurationSupport" />
    public class IndexActivity : IActivity
    {
        static IndexActivity() => traceSource = TraceSources
           .Instance
           .GetTraceSourceFromConfiguredName()
           .WithSourceLevels();

        static readonly TraceSource traceSource;

        /// <summary>
        /// Compresses the index.
        /// </summary>
        /// <param name="indexInfo">The index information.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">indexInfo</exception>
        public static FileInfo CompressIndex(FileInfo indexInfo)
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

        /// <summary>
        /// Generates the index from 11ty entries.
        /// </summary>
        /// <param name="entryRootInfo">The entry root information.</param>
        /// <param name="indexRootInfo">The index root information.</param>
        /// <param name="indexFileName">Name of the index file.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">entryRootInfo
        /// or
        /// indexRootInfo
        /// or
        /// indexFileName</exception>
        public static FileInfo[] GenerateIndexFrom11tyEntries(DirectoryInfo entryRootInfo, DirectoryInfo indexRootInfo, string indexFileName) => GenerateIndexFrom11tyEntries(entryRootInfo, indexRootInfo, indexFileName, partitionSize: 1000);

        /// <summary>
        /// Generates the index from 11ty entries.
        /// </summary>
        /// <param name="entryRootInfo">The entry root information.</param>
        /// <param name="indexRootInfo">The index root information.</param>
        /// <param name="indexFileName">Name of the index file.</param>
        /// <param name="partitionSize">Size of the partition.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">entryRootInfo
        /// or
        /// indexRootInfo
        /// or
        /// indexFileName</exception>
        public static FileInfo[] GenerateIndexFrom11tyEntries(DirectoryInfo entryRootInfo, DirectoryInfo indexRootInfo, string indexFileName, int partitionSize)
        {
            if (entryRootInfo == null) throw new ArgumentNullException(nameof(entryRootInfo));
            if (indexRootInfo == null) throw new ArgumentNullException(nameof(indexRootInfo));
            if (string.IsNullOrEmpty(indexFileName)) throw new ArgumentNullException(nameof(indexFileName));

            var frontMatterDocumentCollections = entryRootInfo
                .GetFiles("*.md", SearchOption.AllDirectories)
                .Select(fileInfo => fileInfo.ToMarkdownEntry().FrontMatter)
                .Select(jO => JObject.FromObject(new
                {
                    extract = JObject.Parse(jO.GetValue<string>("tag", throwException: false) ?? @"{ ""extract"": ""[empty]"" }").GetValue<string>("extract"),
                    clientId = jO.GetValue<string>("clientId", throwException: false) ?? "[empty]",
                    inceptDate = jO.GetValue<string>("date", throwException: false) ?? string.Empty,
                    modificationDate = jO.GetValue<string>("modificationDate", throwException: false) ?? string.Empty,
                    title = jO.GetValue<string>("title", throwException: false) ?? string.Empty
                }))
                .OrderByDescending(o => o.GetValue<string>("clientId"))
                .Partition(partitionSize);

            var indices = new List<FileInfo>();
            var count = 0;
            foreach (var frontMatterDocuments in frontMatterDocumentCollections)
            {
                var jA = new JArray(frontMatterDocuments);
                var path = indexRootInfo.ToCombinedPath(indexFileName.Replace(".json", $"-{count:00}.json"));
                indices.Add(new FileInfo(path));

                File.WriteAllText(path, jA.ToString());

                count++;
            }

            return indices.ToArray();
        }

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
            traceSource?.WriteLine($"starting {nameof(IndexActivity)} with {nameof(ProgramArgs)}: {args} ");
            this.SetContext(args);

            var command = this._jSettings.GetPublicationCommand();
            traceSource?.TraceVerbose($"{nameof(MarkdownEntryActivity)}: {nameof(command)}: {command}");

            if (command.EqualsInvariant(IndexCommands.CommandNameGenerateCompressed11tyIndex)) GenerateCompressed11tyIndex();
            else
            {
                traceSource?.TraceWarning($"{nameof(MarkdownEntryActivity)}: The expected command is not here. Actual: `{command ?? "[null]"}`");
            }
        }

        internal void GenerateCompressed11tyIndex()
        {
            var (entryRootInfo, indexRootInfo, indexFileName) =
                this._jSettings.GetCompressed11tyIndexArgs(this._presentationInfo);

            var indices = GenerateIndexFrom11tyEntries(
                entryRootInfo,
                indexRootInfo,
                indexFileName
            );

            foreach (var indexInfo in indices)
            {
                var compressedIndexInfo = CompressIndex(indexInfo);

                traceSource?.WriteLine($"index: {compressedIndexInfo.FullName}");
            }

        }

        internal void SetContext(ProgramArgs args)
        {
            var (presentationInfo, settingsInfo) = args.ToPresentationAndSettingsInfo();

            this._presentationInfo = presentationInfo;

            traceSource?.TraceVerbose($"applying settings...");
            this._jSettings = JObject.Parse(File.ReadAllText(settingsInfo.FullName));
        }

        DirectoryInfo _presentationInfo;
        JObject _jSettings;
    }
}
