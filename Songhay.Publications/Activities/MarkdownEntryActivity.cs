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
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Songhay.Publications.Activities
{
    public class MarkdownEntryActivity : IActivity
    {
        static MarkdownEntryActivity() => traceSource = TraceSources
            .Instance
            .GetTraceSourceFromConfiguredName()
            .WithSourceLevels();

        static readonly TraceSource traceSource;

        public static void AddEntryExtract(string entryPath)
        {
            if (!File.Exists(entryPath))
                throw new FileNotFoundException($"The expected file, `{entryPath},` is not here.");

            var entryInfo = new FileInfo(entryPath);
            var entry = entryInfo.ToMarkdownEntry();
            var finalEdit = entry
                .With11tyExtract(255)
                .ToFinalEdit();

            File.WriteAllText(entryInfo.FullName, $"{finalEdit}");

            var clientId = entry.FrontMatter.GetValue<string>("clientId");
            traceSource?.WriteLine($"{nameof(MarkdownEntryActivity)}: Added entry extract: {clientId}");
        }

        public static void ExpandUris(string entryPath, string collapsedHost)
        {
            if (!File.Exists(entryPath))
                throw new FileNotFoundException($"The expected file, `{entryPath},` is not here.");

            var entryInfo = new FileInfo(entryPath);

            traceSource?.WriteLine($"{nameof(MarkdownEntryActivity)}: expanding `{collapsedHost}` URIs in `{entryInfo.Name}`...");

            var entry = entryInfo.ToMarkdownEntry();
            var matches = Regex.Matches(entry.Content, $@"https*://{collapsedHost}[^ \]\)]+");
            var uris = matches.OfType<Match>().Select(i => new Uri(i.Value)).Distinct().ToArray();
            async Task<KeyValuePair<Uri, Uri>?> ExpandUriPairAsync(Uri expandableUri)
            {
                KeyValuePair<Uri, Uri>? nullable = null;
                try
                {
                    traceSource?.TraceVerbose($"{nameof(MarkdownEntryActivity)}: expanding `{expandableUri.OriginalString}`...");
                    nullable = await expandableUri.ToExpandedUriPairAsync();
                    traceSource?.TraceVerbose($"{nameof(MarkdownEntryActivity)}: expanded `{nullable.Value.Key.OriginalString}` to `{nullable.Value.Value.OriginalString}`.");
                }
                catch (Exception ex)
                {
                    traceSource?.TraceError(ex);
                }

                return nullable;
            }

            var tasks = uris.Select(ExpandUriPairAsync).Where(i => i.Result.HasValue).ToArray();

            Task.WaitAll(tasks);

            var findChangeSet = tasks.Select(i => i.Result.Value).ToDictionary(k => k.Key, v => v.Value);

            foreach (var pair in findChangeSet)
                entry.Content = entry.Content.Replace(pair.Key.OriginalString, pair.Value.OriginalString);

            traceSource?.WriteLine($"{nameof(MarkdownEntryActivity)}: saving `{entryInfo.Name}`...");
            File.WriteAllText(entryInfo.FullName, entry.ToFinalEdit());
        }

        public static void GenerateEntry(DirectoryInfo entryDraftsRootInfo, string title)
        {
            var entry = MarkdownEntryUtility.GenerateEntryFor11ty(entryDraftsRootInfo.FullName, title);

            if (entry == null)
            {
                throw new NullReferenceException($"The expected {nameof(entry)} is not here.");
            }

            var clientId = entry.FrontMatter.GetValue<string>("clientId");
            traceSource?.WriteLine($"{nameof(MarkdownEntryActivity)}: Generated entry: {clientId}");
        }

        public static void PublishEntry(DirectoryInfo entryDraftsRootInfo, DirectoryInfo entryRootInfo, string entryFileName)
        {
            var path = MarkdownEntryUtility.PublishEntryFor11ty(entryDraftsRootInfo.FullName, entryRootInfo.FullName, entryFileName);

            traceSource?.WriteLine($"{nameof(MarkdownEntryActivity)}: Published entry: {path}");
        }

        public string DisplayHelp(ProgramArgs args)
        {
            throw new NotImplementedException();
        }

        public void Start(ProgramArgs args)
        {
            traceSource?.WriteLine($"starting {nameof(MarkdownEntryActivity)} with {nameof(ProgramArgs)}: {args} ");

            this.SetContext(args);

            var command = this._jSettings.GetValue<string>("command");
            traceSource?.TraceVerbose($"{nameof(MarkdownEntryActivity)}: {nameof(command)}: {command}");

            if (command.EqualsInvariant(MarkdownPresentationCommands.CommandNameAddEntryExtract)) AddEntryExtract();
            else if (command.EqualsInvariant(MarkdownPresentationCommands.CommandNameExpandUris)) ExpandUris();
            else if (command.EqualsInvariant(MarkdownPresentationCommands.CommandNameGenerateEntry)) GenerateEntry();
            else if (command.EqualsInvariant(MarkdownPresentationCommands.CommandNamePublishEntry)) PublishEntry();
            else
            {
                traceSource?.TraceWarning($"{nameof(MarkdownEntryActivity)}: The expected command is not here. Actual: `{command ?? "[null]"}`");
            }
        }

        internal void AddEntryExtract()
        {
            var entryPath = this._jSettings.GetValue<string>("entryPath");
            entryPath = this._presentationInfo.ToCombinedPath(entryPath);

            AddEntryExtract(entryPath);
        }

        internal void ExpandUris()
        {
            var collapsedHost = this._jSettings.GetValue<string>("collapsedHost");
            var entryPath = this._jSettings.GetValue<string>("entryPath");
            entryPath = this._presentationInfo.ToCombinedPath(entryPath);

            ExpandUris(entryPath, collapsedHost);
        }

        internal void GenerateEntry()
        {
            var entryDraftsRootInfo = this._presentationInfo.FindDirectory(MarkdownPresentationDirectories.DirectoryNamePresentationDrafts);
            var title = this._jSettings.GetValue<string>("title");

            GenerateEntry(entryDraftsRootInfo, title);
        }

        internal void PublishEntry()
        {
            var entryDraftsRootInfo = this._presentationInfo
                .FindDirectory(MarkdownPresentationDirectories.DirectoryNamePresentationDrafts);
            var entryRootInfo = this._presentationInfo
                .FindDirectory(MarkdownPresentationDirectories.DirectoryNamePresentation)
                .FindDirectory("entry")
                .FindDirectory(DateTime.Now.Year.ToString());
            var entryFileName = this._jSettings.GetValue<string>("entryFileName");

            PublishEntry(entryDraftsRootInfo, entryRootInfo, entryFileName);
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
