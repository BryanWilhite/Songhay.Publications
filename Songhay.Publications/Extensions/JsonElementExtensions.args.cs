namespace Songhay.Publications.Extensions;

/// <summary>
/// Extensions of <see cref="JsonElement"/>.
/// </summary>
public static partial class JsonElementExtensions
{
    /// <summary>
    /// Gets the arguments for Activity method.
    /// </summary>
    /// <param name="element">The <see cref="JsonElement"/>.</param>
    /// <param name="presentationInfo">The presentation information.</param>
    public static string GetAddEntryExtractArg(this JsonElement element, DirectoryInfo? presentationInfo)
    {
        var entryPath = element.GetProperty("entryPath").GetString();
        entryPath = presentationInfo.ToCombinedPath(entryPath);

        return entryPath;
    }

    /// <summary>
    /// Gets the arguments for Activity method.
    /// </summary>
    /// <param name="element">The <see cref="JsonElement" />.</param>
    /// <param name="presentationInfo">The presentation information.</param>
    public static (
        DirectoryInfo entryRootInfo,
        DirectoryInfo indexRootInfo,
        string indexFileName
        ) GetCompressed11TyIndexArgs(this JsonElement element, DirectoryInfo? presentationInfo)
    {
        string? indexRoot = element.GetProperty("indexRoot").GetString();
        indexRoot = presentationInfo.ToCombinedPath(indexRoot);
        DirectoryInfo indexRootInfo = new DirectoryInfo(indexRoot);
        if (!indexRootInfo.Exists)
            throw new DirectoryNotFoundException($"The expected index root, {indexRoot}, is not here.");

        string indexFileName = element.GetProperty("indexFileName").GetString().ToReferenceTypeValueOrThrow();
        DirectoryInfo entryRootInfo = presentationInfo
            .FindDirectory("presentation")
            .FindDirectory("entry")
            .ToReferenceTypeValueOrThrow();

        return (entryRootInfo, indexRootInfo, indexFileName);
    }

    /// <summary>
    /// Gets the arguments for Activity method.
    /// </summary>
    /// <param name="element">The <see cref="JsonElement"/>.</param>
    /// <param name="presentationInfo">The presentation information.</param>
    public static (
        string entryPath,
        string collapsedHost
        ) GetExpandUrisArgs(this JsonElement element, DirectoryInfo? presentationInfo)
    {
        string collapsedHost = element.GetProperty("collapsedHost").GetString().ToReferenceTypeValueOrThrow();
        string? entryPath = element.GetProperty("entryPath").GetString();
        entryPath = presentationInfo.ToCombinedPath(entryPath).ToReferenceTypeValueOrThrow();

        return (entryPath, collapsedHost);
    }

    /// <summary>
    /// Gets the arguments for Activity method.
    /// </summary>
    /// <param name="element">The <see cref="JsonElement"/>.</param>
    /// <param name="presentationInfo">The presentation information.</param>
    public static (
        string input,
        string pattern,
        string replacement,
        bool useRegex,
        string outputPath)
        GetFindChangeArgs(this JsonElement element, DirectoryInfo? presentationInfo)
    {
        string? inputPath = element.GetProperty("inputPath").GetString();
        inputPath = presentationInfo.ToCombinedPath(inputPath);
        if (!File.Exists(inputPath))
            throw new FileNotFoundException($"The expected input file, `{inputPath}`, is not here.");

        string input = File.ReadAllText(inputPath).ToReferenceTypeValueOrThrow();

        string pattern = element.GetProperty("pattern").GetString().ToReferenceTypeValueOrThrow();
        string replacement = element.GetProperty("replacement").GetString().ToReferenceTypeValueOrThrow();
        bool useRegex = element.GetProperty("useRegex").GetBoolean();

        string? outputPath = element.GetProperty("outputPath").GetString();
        outputPath = presentationInfo.ToCombinedPath(outputPath);

        return (input, pattern, replacement, useRegex, outputPath);
    }

    /// <summary>
    /// Gets the arguments for Activity method.
    /// </summary>
    /// <param name="element">The <see cref="JsonElement"/>.</param>
    /// <param name="presentationInfo">The presentation information.</param>
    public static (
        DirectoryInfo entryDraftsRootInfo,
        string title
        ) GetGenerateEntryArgs(this JsonElement element, DirectoryInfo? presentationInfo)
    {
        DirectoryInfo entryDraftsRootInfo = presentationInfo
            .FindDirectory(MarkdownPresentationDirectories.DirectoryNamePresentationDrafts)
            .ToReferenceTypeValueOrThrow();
        string title = element.GetProperty("title").GetString().ToReferenceTypeValueOrThrow();

        return (entryDraftsRootInfo, title);
    }

    /// <summary>
    /// Gets the arguments for Activity method.
    /// </summary>
    /// <param name="element">The <see cref="JsonElement"/>.</param>
    /// <param name="presentationInfo">The presentation information.</param>
    public static (
        DirectoryInfo entryDraftsRootInfo,
        DirectoryInfo entryRootInfo,
        string entryFileName
        ) GetPublishEntryArgs(this JsonElement element, DirectoryInfo? presentationInfo)
    {
        DirectoryInfo entryDraftsRootInfo = presentationInfo
            .FindDirectory(MarkdownPresentationDirectories.DirectoryNamePresentationDrafts)
            .ToReferenceTypeValueOrThrow();
        DirectoryInfo entryRootInfo = presentationInfo
            .FindDirectory(MarkdownPresentationDirectories.DirectoryNamePresentation)
            .FindDirectory("entry")
            .FindDirectory(DateTime.Now.Year.ToString())
            .ToReferenceTypeValueOrThrow();
        string entryFileName = element.GetProperty("entryFileName").GetString().ToReferenceTypeValueOrThrow();

        return (entryDraftsRootInfo, entryRootInfo, entryFileName);
    }
}
