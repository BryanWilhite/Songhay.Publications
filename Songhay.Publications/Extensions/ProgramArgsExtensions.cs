using Songhay.Diagnostics;
using Songhay.Extensions;
using Songhay.Models;
using Songhay.Publications.Models;
using System.Diagnostics;
using System.IO;

namespace Songhay.Publications.Extensions
{
    /// <summary>
    /// Extensions of <see cref="ProgramArgs" />.
    /// </summary>
    public static class ProgramArgsExtensions
    {
        static ProgramArgsExtensions() => traceSource = TraceSources
           .Instance
           .GetTraceSourceFromConfiguredName()
           .WithSourceLevels();

        static readonly TraceSource traceSource;

        /// <summary>
        /// Converts <see cref="ProgramArgs"/> to Presentation <see cref="DirectoryInfo"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static (DirectoryInfo presentationInfo, FileInfo settingsInfo)
            ToPresentationAndSettingsInfo(this ProgramArgs args)
        {
            traceSource?.TraceVerbose($"setting conventional {MarkdownPresentationDirectories.DirectoryNamePresentationShell} directory...");
            var presentationShellInfo = new DirectoryInfo(args.GetArgValue(ProgramArgs.BasePath));
            presentationShellInfo.VerifyDirectory(MarkdownPresentationDirectories.DirectoryNamePresentationShell);

            traceSource?.TraceVerbose($"setting conventional {nameof(MarkdownPresentationDirectories)} parent directory...");
            var presentationInfo = presentationShellInfo.Parent;
            presentationInfo.HasAllConventionalMarkdownPresentationDirectories();

            traceSource?.TraceVerbose($"getting settings file...");
            var settingsInfo = presentationShellInfo.FindFile(args.GetArgValue(ProgramArgs.SettingsFile));

            return (presentationInfo, settingsInfo);
        }

    }
}
