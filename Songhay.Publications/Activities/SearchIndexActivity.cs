using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Songhay.Publications.Activities;

/// <summary>
/// <see cref="IActivity"/> implementation for Publication Search Indices
/// </summary>
/// <seealso cref="IActivity" />
/// <seealso cref="IActivityConfigurationSupport" />
public class SearchIndexActivity : IActivity
{
    static SearchIndexActivity() => TraceSource = TraceSources
        .Instance
        .GetTraceSourceFromConfiguredName()
        .WithSourceLevels();

    static readonly TraceSource? TraceSource;

    /// <summary>
    /// Compresses the Publications Search Index.
    /// </summary>
    /// <param name="indexInfo">The index information.</param>
    public static FileInfo CompressSearchIndex(FileInfo indexInfo)
    {
        if (indexInfo == null) throw new ArgumentNullException(nameof(indexInfo));

        var compressedIndexInfo = new FileInfo(indexInfo.FullName.Replace(".json", ".c.json"));

        using FileStream fileStream = indexInfo.OpenRead();
        using FileStream compressedFileStream = File.Create(compressedIndexInfo.FullName);
        using GZipStream gZipStream = new GZipStream(compressedFileStream, CompressionMode.Compress);
        fileStream.CopyTo(gZipStream);

        return compressedIndexInfo;
    }

    /// <summary>
    /// Generates the Publications Search Index from 11ty entries.
    /// </summary>
    /// <param name="entryRootInfo">The entry root information.</param>
    /// <param name="indexRootInfo">The index root information.</param>
    /// <param name="indexFileName">Name of the index file.</param>
    public static FileInfo[] GenerateSearchIndexFrom11TyEntries(DirectoryInfo entryRootInfo,
        DirectoryInfo indexRootInfo, string indexFileName) =>
        GenerateSearchIndexFrom11TyEntries(entryRootInfo, indexRootInfo, indexFileName, partitionSize: 1000);

    /// <summary>
    /// Generates the Publications Search Index from 11ty entries.
    /// </summary>
    /// <param name="entryRootInfo">The entry root information.</param>
    /// <param name="indexRootInfo">The index root information.</param>
    /// <param name="indexFileName">Name of the index file.</param>
    /// <param name="partitionSize">Size of the partition.</param>
    public static FileInfo[] GenerateSearchIndexFrom11TyEntries(DirectoryInfo entryRootInfo, DirectoryInfo indexRootInfo, string indexFileName, int partitionSize)
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

    /// <summary>
    /// Displays the help.
    /// </summary>
    /// <param name="args">The arguments.</param>
    public string DisplayHelp(ProgramArgs? args)
    {
        throw new NotImplementedException();
    }

    /// <summary>Starts with the specified arguments.</summary>
    /// <param name="args">The arguments.</param>
    public void Start(ProgramArgs? args)
    {
        TraceSource?.WriteLine($"starting {nameof(SearchIndexActivity)} with {nameof(ProgramArgs)}: {args} ");

        (_presentationInfo, _jSettings) = GetContext(args);

        var command = _jSettings.GetPublicationCommand();
        TraceSource?.TraceVerbose($"{nameof(MarkdownEntryActivity)}: {nameof(command)}: {command}");

        if (command.EqualsInvariant(IndexCommands.CommandNameGenerateCompressed11TySearchIndex)) GenerateCompressed11TySearchIndex();
        else
        {
            TraceSource?.TraceWarning($"{nameof(MarkdownEntryActivity)}: The expected command is not here. Actual: `{command ?? "[null]"}`");
        }
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

            TraceSource?.WriteLine($"index: {compressedIndexInfo.FullName}");
        }

    }

    internal (DirectoryInfo presentationInfo, JsonElement jSettings) GetContext(ProgramArgs? args)
    {
        var (presentationInfo, settingsInfo) = args.ToPresentationAndSettingsInfo();

        TraceSource?.TraceVerbose($"applying settings...");
        var jSettings = JsonDocument.Parse(File.ReadAllText(settingsInfo.FullName)).ToReferenceTypeValueOrThrow().RootElement;

        return (presentationInfo, jSettings);
    }

    DirectoryInfo? _presentationInfo;
    JsonElement _jSettings;
}
