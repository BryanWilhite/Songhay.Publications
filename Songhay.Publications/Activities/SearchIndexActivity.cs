using System.IO.Compression;

namespace Songhay.Publications.Activities;

/// <summary>
/// <see cref="IActivity"/> implementation for Publication Search Indices
/// </summary>
/// <seealso cref="IActivity" />
/// <seealso cref="IActivityTask" />
public class SearchIndexActivity : IActivityTask
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
    /// Starts the <see cref="IActivity"/>.
    /// </summary>
    public async Task StartAsync()
    {
        await Task.Run(() =>
        {
            _logger.LogInformation($"Starting {nameof(SearchIndexActivity)}...");

            (_presentationInfo, _jSettings) = GetContext();

            var command = _jSettings.GetPublicationCommand();
            _logger.LogInformation("{ActivityName}: {Label}: `{Command}`", nameof(MarkdownEntryActivity), nameof(command),
                command);

            if (command.EqualsInvariant(IndexCommands.CommandNameGenerateCompressed11TySearchIndex)) GenerateCompressed11TySearchIndex();
            else
            {
                _logger.LogWarning("{ActivityName}: The expected command is not here. Actual: `{Command}`",
                    nameof(MarkdownEntryActivity), command ?? "[null]");
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
        ArgumentNullException.ThrowIfNull(entryRootInfo);
        ArgumentNullException.ThrowIfNull(indexRootInfo);
        indexFileName.ThrowWhenNullOrWhiteSpace();

        var frontMatterDocumentCollections = entryRootInfo
            .GetFiles("*.md", SearchOption.AllDirectories)
            .Select(fileInfo => fileInfo.ToMarkdownEntry().FrontMatter)
            .Select(jO =>
            {
                JsonNode? node = JsonNodeUtility.ConvertToJsonNode(new {
                    extract = JsonNode.Parse(jO["tag"]?.GetValue<string>() ?? @"{ ""extract"": ""[empty]"" }")?.AsObject()["extract"]?.GetValue<string>(),
                    clientId = jO["clientId"]?.GetValue<string>() ?? "[empty]",
                    inceptDate = jO["date"]?.GetValue<string>() ?? string.Empty,
                    modificationDate = jO["modificationDate"]?.GetValue<string>() ?? string.Empty,
                    title = jO["title"]?.GetValue<string>() ?? string.Empty
                });

                return node.ToReferenceTypeValueOrThrow();
            })
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

            _logger.LogInformation("index: `{Name}`", compressedIndexInfo.FullName);
        }

    }

    internal (DirectoryInfo presentationInfo, JsonElement jSettings) GetContext()
    {
        var (presentationInfo, settingsInfo) = _configuration.ToPresentationAndSettingsInfo(_logger);

        _logger.LogInformation("applying settings...");
        var jSettings = JsonDocument.Parse(File.ReadAllText(settingsInfo.FullName)).ToReferenceTypeValueOrThrow().RootElement;

        return (presentationInfo, jSettings);
    }

    private DirectoryInfo? _presentationInfo;
    private JsonElement _jSettings;

    private readonly IConfiguration _configuration;
    private readonly ILogger<SearchIndexActivity> _logger;
}
