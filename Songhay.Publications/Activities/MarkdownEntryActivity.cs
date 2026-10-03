using System.Text.RegularExpressions;

namespace Songhay.Publications.Activities;

/// <summary>
/// <see cref="IActivity"/> implementation for <see cref="MarkdownEntry"/>.
/// </summary>
/// <param name="configuration">the <see cref="IConfiguration"/></param>
/// <param name="httpClientFactory">the <see cref="IHttpClientFactory"/></param>
/// <param name="logger">the <see cref="ILogger"/></param>
/// <seealso cref="IActivity" />
public class MarkdownEntryActivity(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<MarkdownEntryActivity> logger) : IActivityTask
{
    /// <summary>
    /// Starts the <see cref="IActivity"/>.
    /// </summary>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.Run(() =>
        {
            logger.LogInformation("Starting {ActivityName}...", nameof(MarkdownEntryActivity));

            (_presentationInfo, _jSettings) = GetContext();

            string? command = _jSettings.GetPublicationCommand();

            logger.LogInformation("{ActivityName}: {Label}: `{Command}`", nameof(MarkdownEntryActivity), nameof(command), command);

            if (command.EqualsInvariant(MarkdownPresentationCommands.CommandNameAddEntryExtract)) AddEntryExtract();
            else if (command.EqualsInvariant(MarkdownPresentationCommands.CommandNameExpandUris)) ExpandUris();
            else if (command.EqualsInvariant(MarkdownPresentationCommands.CommandNameGenerateEntry)) GenerateEntry();
            else if (command.EqualsInvariant(MarkdownPresentationCommands.CommandNamePublishEntry)) PublishEntry();
            else
            {
                logger.LogWarning("{ActivityName}: The expected command is not here. Actual: `{Command}`",
                    nameof(MarkdownEntryActivity), command ?? "[null]");
            }
        }, cancellationToken);
    }

    internal static string FindChange(string? input, string? pattern, string? replacement, bool useRegex)
    {
        input.ThrowWhenNullOrWhiteSpace();
        pattern.ThrowWhenNullOrWhiteSpace();
        
        if (string.IsNullOrWhiteSpace(replacement)) replacement = string.Empty;

        return useRegex ?
            Regex.Replace(input, pattern, replacement, RegexOptions.IgnoreCase | RegexOptions.Multiline)
            :
            input.Replace(pattern, replacement);
    }

    internal void AddEntryExtract()
    {
        var entryPath = _jSettings.GetAddEntryExtractArg(_presentationInfo);

        AddEntryExtract(entryPath);
    }

    internal void AddEntryExtract(string? entryPath)
    {
        if (!File.Exists(entryPath))
            throw new FileNotFoundException($"The expected file, `{entryPath},` is not here.");

        var entryInfo = new FileInfo(entryPath);
        var entry = entryInfo.ToMarkdownEntry();
        var finalEdit = entry
            .With11TyExtract(255)
            .ToFinalEdit();

        File.WriteAllText(entryInfo.FullName, $"{finalEdit}");

        var clientId = entry.FrontMatter["clientId"].ToReferenceTypeValueOrThrow().GetValue<string>();
        logger.LogInformation("{ActivityName}: Added entry extract: `{Id}`", nameof(MarkdownEntryActivity), clientId);
    }

    internal void ExpandUris()
    {
        var (entryPath, collapsedHost) = _jSettings.GetExpandUrisArgs(_presentationInfo);

        ExpandUrisAsync(entryPath, collapsedHost).GetAwaiter().GetResult();
    }

    internal async Task ExpandUrisAsync(string? entryPath, string? collapsedHost)
    {
        if (!File.Exists(entryPath))
            throw new FileNotFoundException($"The expected file, `{entryPath},` is not here.");

        var entryInfo = new FileInfo(entryPath);

        logger.LogInformation("{ActivityName}: expanding `{Host}` URIs in `{Name}`...", nameof(MarkdownEntryActivity),
            collapsedHost, entryInfo.Name);

        var entry = entryInfo.ToMarkdownEntry();
        var matches =
            Regex.Matches(entry.Content.ToReferenceTypeValueOrThrow(), $@"https*://{collapsedHost}[^ \]\)]+");
        var uris = matches.OfType<Match>()
            .ToReferenceTypeValueOrThrow()
            .Select(i => new Uri(i.Value))
            .Distinct().ToArray();

        async Task<KeyValuePair<Uri, Uri>?> ExpandUriPairAsync(Uri expandableUri)
        {
            KeyValuePair<Uri, Uri>? nullable = null;
            try
            {
                logger.LogInformation("{ActivityName}: expanding `{Uri}`...", nameof(MarkdownEntryActivity),
                    expandableUri.OriginalString);

                var pair = await expandableUri.ToExpandedUriPairAsync(httpClientFactory.CreateClient);
                if (pair.Key is not null && pair.Value is not null)
                {
                    nullable = new KeyValuePair<Uri, Uri>(pair.Key, pair.Value);

                    logger.LogInformation("{ActivityName}: expanded `{Uri}` to `{UriExpanded}`.", nameof(MarkdownEntryActivity),
                        nullable.Value.Key.OriginalString, nullable.Value.Value.OriginalString);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("{Message}{NewLine}{StackTrace}", ex.Message, Environment.NewLine, ex.StackTrace);
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
                pair.Key.OriginalString,
                pair.Value.OriginalString
            );

        logger.LogInformation("{ActivityName}: saving `{Name}`...", nameof(MarkdownEntryActivity), entryInfo.Name);

        await File.WriteAllTextAsync(entryInfo.FullName, entry.ToFinalEdit());
    }

    internal void GenerateEntry()
    {
        var (entryDraftsRootInfo, title) = _jSettings.GetGenerateEntryArgs(_presentationInfo);

        GenerateEntry(entryDraftsRootInfo, title);
    }

    internal void GenerateEntry(DirectoryInfo entryDraftsRootInfo, string title)
    {
        var entry = MarkdownEntryUtility.GenerateEntryFor11Ty(entryDraftsRootInfo.FullName, title);

        if (entry == null)
        {
            throw new NullReferenceException($"The expected {nameof(entry)} is not here.");
        }

        var clientId = entry.FrontMatter["clientId"].ToReferenceTypeValueOrThrow().GetValue<string>();
        logger.LogInformation("{ActivityName}: Generated entry: `{Id}`", nameof(MarkdownEntryActivity), clientId);
    }

    internal (DirectoryInfo presentationInfo, JsonElement jSettings) GetContext()
    {
        (DirectoryInfo presentationInfo, FileInfo settingsInfo) = configuration.ToPresentationAndSettingsInfo(logger);

        logger.LogInformation("applying settings...");

        using var jDoc = JsonDocument.Parse(File.ReadAllText(settingsInfo.FullName)).ToReferenceTypeValueOrThrow();
        JsonElement jSettings = jDoc.RootElement;

        return (presentationInfo, jSettings);
    }

    internal void PublishEntry()
    {
        var (entryDraftsRootInfo, entryRootInfo, entryFileName) = _jSettings.GetPublishEntryArgs(_presentationInfo);

        PublishEntry(entryDraftsRootInfo, entryRootInfo, entryFileName);
    }

    internal void PublishEntry(DirectoryInfo entryDraftsRootInfo, DirectoryInfo entryRootInfo, string entryFileName)
    {
        var path = MarkdownEntryUtility.PublishEntryFor11Ty(entryDraftsRootInfo.FullName, entryRootInfo.FullName, entryFileName);

        logger.LogInformation("{Name}: Published entry: `{Path}`", nameof(MarkdownEntryActivity), path);
    }

    private DirectoryInfo? _presentationInfo;
    private JsonElement _jSettings;
}
