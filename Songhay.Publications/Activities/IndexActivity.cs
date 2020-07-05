using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Songhay.Diagnostics;
using Songhay.Extensions;
using Songhay.Models;
using Songhay.Publications.Extensions;
using Songhay.Publications.Models;
using System;
using System.Diagnostics;
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
        static IndexActivity() => traceSource = TraceSources
           .Instance
           .GetTraceSourceFromConfiguredName()
           .WithSourceLevels();

        static readonly TraceSource traceSource;

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

        internal static FileInfo GenerateIndexFrom11tyEntries(DirectoryInfo entryRootInfo, DirectoryInfo indexRootInfo, string indexFileName)
        {
            if (entryRootInfo == null) throw new ArgumentNullException(nameof(entryRootInfo));
            if (indexRootInfo == null) throw new ArgumentNullException(nameof(indexRootInfo));
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
            var targetInfo = indexRootInfo.FindFile(indexFileName);

            File.WriteAllText(targetInfo.FullName, jA.ToString());

            return targetInfo;
        }

        internal void GenerateCompressed11tyIndex()
        {
            var (entryRootInfo, indexRootInfo, indexFileName) =
                this._jSettings.GetCompressed11tyIndexArgs(this._presentationInfo);

            var indexInfo = GenerateIndexFrom11tyEntries(
                entryRootInfo,
                indexRootInfo,
                indexFileName
            );

            var compressedIndexInfo = CompressIndex(indexInfo);

            traceSource.WriteLine($"index: {compressedIndexInfo.FullName}");
        }

        internal void SetContext(ProgramArgs args)
        {
            traceSource?.TraceVerbose($"setting conventional {MarkdownPresentationDirectories.DirectoryNamePresentationShell} directory...");
            var presentationShellInfo = new DirectoryInfo(args.GetArgValue(ProgramArgs.BasePath));
            presentationShellInfo.VerifyDirectory(MarkdownPresentationDirectories.DirectoryNamePresentationShell);

            traceSource?.TraceVerbose($"setting conventional {nameof(MarkdownPresentationDirectories)} parent directory...");
            this._presentationInfo = presentationShellInfo.Parent;
            this._presentationInfo.HasAllConventionalMarkdownPresentationDirectories();

            traceSource?.TraceVerbose($"getting settings file...");
            var settingsInfo = presentationShellInfo.FindFile(args.GetArgValue(ProgramArgs.SettingsFile));

            traceSource?.TraceVerbose($"applying settings...");
            this._jSettings = JObject.Parse(File.ReadAllText(settingsInfo.FullName));
        }

        DirectoryInfo _presentationInfo;
        JObject _jSettings;
    }
}
