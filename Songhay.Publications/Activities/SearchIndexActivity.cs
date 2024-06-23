using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Songhay.Publications.Activities;

/// <summary>
/// <see cref="IActivity"/> implementation for Publication Search Indices
/// </summary>
/// <seealso cref="IActivity" />
/// <seealso cref="IActivityConfigurationSupport" />
public class SearchIndexActivity : IActivityWithTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MarkdownEntryActivity"/> class.
    /// </summary>
    /// <param name="configuration">the <see cref="IConfiguration"/></param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    public SearchIndexActivity(IConfiguration configuration, ILogger<SearchIndexActivity> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Displays the help.
    /// </summary>
    /// <param name="args">The arguments.</param>
    public string DisplayHelp(ProgramArgs? args) => throw new NotImplementedException();

    /// <summary>Starts with the specified arguments.</summary>
    /// <param name="args">The arguments.</param>
    public void Start(ProgramArgs? args) => throw new NotImplementedException();

    /// <summary>
    /// Starts the <see cref="IActivity"/>.
    /// </summary>
    public async Task StartAsync()
    {
        await Task.Run(() =>
        {
            _logger.LogInformation($"Starting {nameof(SearchIndexActivity)}...");

            (_presentationInfo, _jSettings) = GetContext();

            var command = _jSettings.GetPublicationCommand();
            _logger.LogInformation($"{nameof(MarkdownEntryActivity)}: {nameof(command)}: {command}");

            if (command.EqualsInvariant(IndexCommands.CommandNameGenerateCompressed11TySearchIndex)) GenerateCompressed11TySearchIndex();
            else
            {
                _logger.LogWarning($"{nameof(MarkdownEntryActivity)}: The expected command is not here. Actual: `{command ?? "[null]"}`");
            }
        });
    }

    internal static FileInfo CompressSearchIndex(FileInfo indexInfo)
    {
        if (indexInfo == null) throw new ArgumentNullException(nameof(indexInfo));

        var compressedIndexInfo = new FileInfo(indexInfo.FullName.Replace(".json", ".c.json"));

        using FileStream fileStream = indexInfo.OpenRead();
        using FileStream compressedFileStream = File.Create(compressedIndexInfo.FullName);
        using GZipStream gZipStream = new GZipStream(compressedFileStream, CompressionMode.Compress);
        fileStream.CopyTo(gZipStream);

        return compressedIndexInfo;
    }

    internal static FileInfo[] GenerateSearchIndexFrom11TyEntries(DirectoryInfo entryRootInfo,
        DirectoryInfo indexRootInfo, string indexFileName) =>
        GenerateSearchIndexFrom11TyEntries(entryRootInfo, indexRootInfo, indexFileName, partitionSize: 1000);

    internal static FileInfo[] GenerateSearchIndexFrom11TyEntries(DirectoryInfo entryRootInfo, DirectoryInfo indexRootInfo, string indexFileName, int partitionSize)
    {
        if (entryRootInfo == null) throw new ArgumentNullException(nameof(entryRootInfo));
        if (indexRootInfo == null) throw new ArgumentNullException(nameof(indexRootInfo));
        if (string.IsNullOrEmpty(indexFileName)) throw new ArgumentNullException(nameof(indexFileName));

        var frontMatterDocumentCollections = entryRootInfo
            .GetFiles("*.md", SearchOption.AllDirectories)
            .Select(fileInfo => fileInfo.ToMarkdownEntry().FrontMatter)
            .Select(jO => new
            {
                extract = JsonNode.Parse(jO["tag"]?.GetValue<string>() ?? @"{ ""extract"": ""[empty]"" }")?.AsObject()["extract"]?.GetValue<string>(),
                clientId = jO["clientId"]?.GetValue<string>() ?? "[empty]",
                inceptDate = jO["date"]?.GetValue<string>() ?? string.Empty,
                modificationDate = jO["modificationDate"]?.GetValue<string>() ?? string.Empty,
                title = jO["title"]?.GetValue<string>() ?? string.Empty
            }.ToJsonNode().ToReferenceTypeValueOrThrow())
            .OrderByDescending(o => o["clientId"]?.GetValue<string>())
            .Partition(partitionSize);

        var indices = new List<FileInfo>();
        var count = 0;
        foreach (var frontMatterDocuments in frontMatterDocumentCollections)
        {
            var jA = new JsonArray(new JsonNodeOptions { PropertyNameCaseInsensitive = false },frontMatterDocuments.ToArray());
            var path = indexRootInfo.ToCombinedPath(indexFileName.Replace(".json", $"-{count:00}.json"));
            indices.Add(new FileInfo(path));

            File.WriteAllText(path, jA.ToString());

            count++;
        }

        return indices.ToArray();
    }

    internal void GenerateCompressed11TySearchIndex()
    {
        var (entryRootInfo, indexRootInfo, indexFileName) =
            _jSettings.GetCompressed11TyIndexArgs(_presentationInfo);

        var indices = GenerateSearchIndexFrom11TyEntries(
            entryRootInfo,
            indexRootInfo,
            indexFileName
        );

        foreach (var indexInfo in indices)
        {
            var compressedIndexInfo = CompressSearchIndex(indexInfo);

            _logger.LogInformation($"index: {compressedIndexInfo.FullName}");
        }

    }

    internal (DirectoryInfo presentationInfo, JsonElement jSettings) GetContext()
    {
        var (presentationInfo, settingsInfo) = _configuration.ToPresentationAndSettingsInfo(_logger);

        _logger.LogInformation($"applying settings...");
        var jSettings = JsonDocument.Parse(File.ReadAllText(settingsInfo.FullName)).ToReferenceTypeValueOrThrow().RootElement;

        return (presentationInfo, jSettings);
    }

    DirectoryInfo? _presentationInfo;
    JsonElement _jSettings;

    readonly IConfiguration _configuration;
    readonly ILogger<SearchIndexActivity> _logger;
}
