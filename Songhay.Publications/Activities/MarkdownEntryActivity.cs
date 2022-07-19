using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Songhay.Publications.Activities;

/// <summary>
/// <see cref="IActivity"/> implementation for <see cref="MarkdownEntry"/>.
/// </summary>
/// <seealso cref="IActivity" />
public class MarkdownEntryActivity : IActivity
{
    static MarkdownEntryActivity() => TraceSource = TraceSources
        .Instance
        .GetTraceSourceFromConfiguredName()
        .WithSourceLevels();

    static readonly TraceSource? TraceSource;

    /// <summary>
    /// Wrapper for <see cref="MarkdownEntryExtensions.With11TyExtract"/>.
    /// </summary>
    /// <param name="entryPath">The entry path.</param>
    public static void AddEntryExtract(string? entryPath)
    {
        if (!File.Exists(entryPath))
            throw new FileNotFoundException($"The expected file, `{entryPath},` is not here.");

        var entryInfo = new FileInfo(entryPath);
        var entry = entryInfo.ToMarkdownEntry();
        var finalEdit = entry
            .With11TyExtract(255)
            .ToFinalEdit();

        File.WriteAllText(entryInfo.FullName, $"{finalEdit}");

        var clientId = entry.FrontMatter.GetValue<string>("clientId");
        TraceSource?.WriteLine($"{nameof(MarkdownEntryActivity)}: Added entry extract: {clientId}");
    }

    /// <summary>
    /// Expands the URIs of the specified host
    /// with <see cref="UriExtensions.ToExpandedUriPairAsync(Uri)"/>.
    /// </summary>
    /// <param name="entryPath">The entry path.</param>
    /// <param name="collapsedHost">The collapsed host.</param>
    public static async Task ExpandUrisAsync(string? entryPath, string? collapsedHost)
    {
        if (!File.Exists(entryPath))
            throw new FileNotFoundException($"The expected file, `{entryPath},` is not here.");

        var entryInfo = new FileInfo(entryPath);

        TraceSource?.WriteLine($"{nameof(MarkdownEntryActivity)}: expanding `{collapsedHost}` URIs in `{entryInfo.Name}`...");

        var entry = entryInfo.ToMarkdownEntry();
        var matches =
            Regex.Matches(entry.Content.ToReferenceTypeValueOrThrow(), $@"https*://{collapsedHost}[^ \]\)]+");
        var uris = matches.OfType<Match>()
            .ToReferenceTypeValueOrThrow()
            .Select(i => new Uri(i.Value))
            .Distinct().ToArray();
        async Task<KeyValuePair<Uri?, Uri?>?> ExpandUriPairAsync(Uri expandableUri)
        {
            KeyValuePair<Uri?, Uri?>? nullable = null;
            try
            {
                TraceSource?.TraceVerbose($"{nameof(MarkdownEntryActivity)}: expanding `{expandableUri.OriginalString}`...");
                nullable = await expandableUri.ToExpandedUriPairAsync();
                TraceSource?.TraceVerbose($"{nameof(MarkdownEntryActivity)}: expanded `{nullable.Value.Key.OriginalString}` to `{nullable.Value.Value.OriginalString}`.");
            }
            catch (Exception ex)
            {
                TraceSource?.TraceError(ex);
            }

            return nullable;
        }

        var tasks = uris
            .Select(ExpandUriPairAsync)
            .Where(i => i.Result.HasValue)
            .ToArray();

        await Task.WhenAll(tasks);

        var findChangeSet =
            tasks.Select(i =>
                i.Result.GetValueOrDefault())
                .ToDictionary(
                    k => k.Key,
                    v => v.Value
                );

        foreach (var pair in findChangeSet)
            entry.Content = entry.Content.Replace(
                pair.Key?.OriginalString ?? string.Empty,
                pair.Value?.OriginalString
            );

        TraceSource?.WriteLine($"{nameof(MarkdownEntryActivity)}: saving `{entryInfo.Name}`...");
        File.WriteAllText(entryInfo.FullName, entry.ToFinalEdit());
    }

    /// <summary>
    /// Finds the specified pattern in the input.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="pattern">The pattern.</param>
    /// <param name="replacement">The replacement.</param>
    /// <param name="useRegex">if set to <c>true</c> [use regex].</param>
    public static string FindChange(string input, string pattern, string replacement, bool useRegex)
    {
        if (string.IsNullOrWhiteSpace(input)) throw new ArgumentNullException(nameof(input));
        if (string.IsNullOrWhiteSpace(pattern)) throw new ArgumentNullException(nameof(pattern));
        if (string.IsNullOrWhiteSpace(replacement)) replacement = string.Empty;

        return useRegex ?
            Regex.Replace(input, pattern, replacement, RegexOptions.IgnoreCase | RegexOptions.Multiline)
            :
            input.Replace(pattern, replacement);
    }

    /// <summary>
    /// Generates the <see cref="MarkdownEntry"/>
    /// at the conventional drafts root.
    /// </summary>
    /// <param name="entryDraftsRootInfo">The entry drafts root information.</param>
    /// <param name="title">The title.</param>
    public static void GenerateEntry(DirectoryInfo entryDraftsRootInfo, string title)
    {
        var entry = MarkdownEntryUtility.GenerateEntryFor11Ty(entryDraftsRootInfo.FullName, title);

        if (entry == null)
        {
            throw new NullReferenceException($"The expected {nameof(entry)} is not here.");
        }

        var clientId = entry.FrontMatter.GetValue<string>("clientId");
        TraceSource?.WriteLine($"{nameof(MarkdownEntryActivity)}: Generated entry: {clientId}");
    }

    /// <summary>
    /// Wrapper for <see cref="MarkdownEntryUtility.PublishEntryFor11Ty(string, string, string)"/>.
    /// </summary>
    /// <param name="entryDraftsRootInfo">The entry drafts root information.</param>
    /// <param name="entryRootInfo">The entry root information.</param>
    /// <param name="entryFileName">Name of the entry file.</param>
    public static void PublishEntry(DirectoryInfo entryDraftsRootInfo, DirectoryInfo entryRootInfo, string entryFileName)
    {
        var path = MarkdownEntryUtility.PublishEntryFor11Ty(entryDraftsRootInfo.FullName, entryRootInfo.FullName, entryFileName);

        TraceSource?.WriteLine($"{nameof(MarkdownEntryActivity)}: Published entry: {path}");
    }

    /// <summary>
    /// Displays the help.
    /// </summary>
    /// <param name="args">The arguments.</param>
    public string DisplayHelp(ProgramArgs? args)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Starts the <see cref="IActivity"/>.
    /// </summary>
    /// <param name="args">The arguments.</param>
    public void Start(ProgramArgs? args)
    {
        TraceSource?.WriteLine($"starting {nameof(MarkdownEntryActivity)} with {nameof(ProgramArgs)}: {args} ");

        SetContext(args);

        var command = _jSettings.GetPublicationCommand();
        TraceSource?.TraceVerbose($"{nameof(MarkdownEntryActivity)}: {nameof(command)}: {command}");

        if (command.EqualsInvariant(MarkdownPresentationCommands.CommandNameAddEntryExtract)) AddEntryExtract();
        else if (command.EqualsInvariant(MarkdownPresentationCommands.CommandNameExpandUris)) ExpandUris();
        else if (command.EqualsInvariant(MarkdownPresentationCommands.CommandNameGenerateEntry)) GenerateEntry();
        else if (command.EqualsInvariant(MarkdownPresentationCommands.CommandNamePublishEntry)) PublishEntry();
        else
        {
            TraceSource?.TraceWarning($"{nameof(MarkdownEntryActivity)}: The expected command is not here. Actual: `{command ?? "[null]"}`");
        }
    }

    internal void AddEntryExtract()
    {
        var entryPath = _jSettings.GetAddEntryExtractArg(_presentationInfo);
        AddEntryExtract(entryPath);
    }

    internal void ExpandUris()
    {
        var (entryPath, collapsedHost) = _jSettings.GetExpandUrisArgs(_presentationInfo);

        ExpandUrisAsync(entryPath, collapsedHost).GetAwaiter().GetResult();
    }

    internal void FindChange()
    {
        var (input, pattern, replacement, useRegex, outputPath) = _jSettings.GetFindChangeArgs(_presentationInfo);
        var output = FindChange(input, pattern, replacement, useRegex);
        File.Create(outputPath);
        File.WriteAllText(outputPath, output);
    }

    internal void GenerateEntry()
    {
        var (entryDraftsRootInfo, title) = _jSettings.GetGenerateEntryArgs(_presentationInfo);

        GenerateEntry(entryDraftsRootInfo, title);
    }

    internal void PublishEntry()
    {
        var (entryDraftsRootInfo, entryRootInfo, entryFileName) =
            _jSettings.GetPublishEntryArgs(_presentationInfo);

        PublishEntry(entryDraftsRootInfo, entryRootInfo, entryFileName);
    }

    internal void SetContext(ProgramArgs? args)
    {
        var (presentationInfo, settingsInfo) = args.ToPresentationAndSettingsInfo();

        _presentationInfo = presentationInfo;

        TraceSource?.TraceVerbose($"applying settings...");
        _jSettings = JObject.Parse(File.ReadAllText(settingsInfo.FullName));
    }

    DirectoryInfo? _presentationInfo;
    JObject? _jSettings;
}
