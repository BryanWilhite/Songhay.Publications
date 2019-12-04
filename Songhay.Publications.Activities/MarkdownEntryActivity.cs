using Newtonsoft.Json.Linq;
using Songhay.Diagnostics;
using Songhay.Extensions;
using Songhay.Models;
using Songhay.Publications.Extensions;
using Songhay.Publications.Models;
using System;
using System.Diagnostics;
using System.IO;

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

            if (command.EqualsInvariant(MarkdownPresentationCommands.CommandNameGenerateEntry)) GenerateEntry();
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
