using Newtonsoft.Json.Linq;
using Songhay.Diagnostics;
using Songhay.Extensions;
using Songhay.Models;
using Songhay.Publications.Extensions;
using Songhay.Publications.Models;
using System;
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

        public string DisplayHelp(ProgramArgs args)
        {
            throw new NotImplementedException();
        }

        public void Start(ProgramArgs args)
        {
            traceSource?.TraceInformation($"starting {nameof(MarkdownEntryActivity)} with {nameof(ProgramArgs)}: {args} ");

            this.SetContext(args);

            var command = this._jSettings.GetValue<string>("command");
            traceSource?.TraceVerbose($"{nameof(command)}: {command}");

            if (command.EqualsInvariant(MarkdownPresentationCommands.CommandNameExpandUris)) ExpandUris();
            if (command.EqualsInvariant(MarkdownPresentationCommands.CommandNameGenerateEntry)) GenerateEntry();
            if (command.EqualsInvariant(MarkdownPresentationCommands.CommandNamePublishEntry)) PublishEntry();
        }

        internal void ExpandUris()
        {
            var collapsedHost = this._jSettings.GetValue<string>("collapsedHost");
            var entryPath = this._jSettings.GetValue<string>("entryPath");
            entryPath = this._presentationInfo.ToCombinedPath(entryPath);

            if (!File.Exists(entryPath))
                throw new FileNotFoundException($"The expected file, `{entryPath},` is not here.");

            var entryInfo = new FileInfo(entryPath);
            var entry = entryInfo.ToMarkdownEntry();
            var matches = Regex.Matches(entry.Content, $@"https*://{collapsedHost}[^ \]\)]+");
            var uris = matches.OfType<Match>().Select(i => new Uri(i.Value)).Distinct().ToArray();
            var tasks = uris.Select(i => i.ToExpandedUriPairAsync()).ToArray();

            Task.WaitAll(tasks);

            var findChangeSet = tasks.Select(i => i.Result).ToDictionary(k => k.Key, v => v.Value);

            foreach (var pair in findChangeSet)
                entry.Content = entry.Content.Replace(pair.Key.OriginalString, pair.Value.OriginalString);

            File.WriteAllText(entryInfo.FullName, entry.ToFinalEdit());
        }

        internal void GenerateEntry()
        {
            var entryDraftsRootInfo = this._presentationInfo.FindDirectory(MarkdownPresentationDirectories.DirectoryNamePresentationDrafts);
            var title = this._jSettings.GetValue<string>("title");

            var entry = MarkdownEntryUtility.GenerateEntryFor11ty(entryDraftsRootInfo.FullName, title);

            if(entry == null)
            {
                throw new NullReferenceException($"The expected {nameof(entry)} is not here.");
            }

            var clientId = entry.FrontMatter.GetValue<string>("clientId");

            traceSource?.TraceInformation($"Generated entry: {clientId}");
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

            var path = MarkdownEntryUtility.PublishEntryFor11ty(entryDraftsRootInfo.FullName, entryRootInfo.FullName, entryFileName);

            traceSource?.TraceInformation($"Published entry: {path}");
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
