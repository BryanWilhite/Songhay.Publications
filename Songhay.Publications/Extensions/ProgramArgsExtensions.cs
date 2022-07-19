namespace Songhay.Publications.Extensions;

/// <summary>
/// Extensions of <see cref="ProgramArgs" />.
/// </summary>
public static class ProgramArgsExtensions
{
    static ProgramArgsExtensions() => TraceSource = TraceSources
        .Instance
        .GetTraceSourceFromConfiguredName()
        .WithSourceLevels();

    static readonly TraceSource? TraceSource;

    /// <summary>
    /// Converts <see cref="ProgramArgs"/> to Presentation <see cref="DirectoryInfo"/>.
    /// </summary>
    /// <param name="args">The arguments.</param>
    public static (DirectoryInfo presentationInfo, FileInfo settingsInfo)
        ToPresentationAndSettingsInfo(this ProgramArgs? args)
    {
        TraceSource?.TraceVerbose($"setting conventional {MarkdownPresentationDirectories.DirectoryNamePresentationShell} directory...");
        var presentationShellInfo = new DirectoryInfo(args.GetArgValue(ProgramArgs.BasePath).ToReferenceTypeValueOrThrow());
        presentationShellInfo.VerifyDirectory(MarkdownPresentationDirectories.DirectoryNamePresentationShell);

        TraceSource?.TraceVerbose($"setting conventional {nameof(MarkdownPresentationDirectories)} parent directory...");
        var presentationInfo = presentationShellInfo.Parent.ToReferenceTypeValueOrThrow();
        presentationInfo.HasAllConventionalMarkdownPresentationDirectories();

        TraceSource?.TraceVerbose($"getting settings file...");
        var settingsInfo = presentationShellInfo
            .FindFile(args.GetArgValue(ProgramArgs.SettingsFile))
            .ToReferenceTypeValueOrThrow();

        return (presentationInfo, settingsInfo);
    }
}
