namespace Songhay.Publications.Extensions;

/// <summary>
/// Extensions of <see cref="FileInfo"/>
/// </summary>
public static class FileInfoExtensions
{
    /// <summary>
    /// Return <c>true</c> when the specified collection of lines look like JSON
    /// </summary>
    /// <param name="lines">collection of lines</param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    public static bool LookLikeJsonFrontMatter(this IReadOnlyCollection<string>? lines, ILogger logger)
    {
        if (lines == null || lines.Count == 0)
        {
            logger.LogWarning("Warning: the expected lines are not here.");

            return false;
        }

        logger.LogInformation("Scanning front matter lines for JSON-like format...");

        if (!lines.First().Trim().StartsWith("{"))
        {
            logger.LogWarning("Warning: Cannot find leading `{{`. Does not look like well-formed JSON front matter. Returning...");

            return false;
        }

        if (!lines.Skip(1).First().Trim().StartsWith("\""))
        {
            logger.LogWarning("Warning: Cannot find leading `\"`. Does not look like well-formed JSON front matter. Returning...");

            return false;
        }

        if (!lines.Skip(1).First().Trim().Contains(':'))
        {
            logger.LogWarning("Warning: Cannot find `:`. Does not look like well-formed JSON front matter. Returning...");

            return false;
        }

        if (!lines.Last().Trim().EndsWith("}"))
        {
            logger.LogWarning("Warning: Cannot find trailing `}}`. Does not look like well-formed JSON front matter. Returning...");

            return false;
        }

        logger.LogInformation("Looks like JSON front matter. Returning...");

        return true;
    }

    /// <summary>
    /// Return <c>true</c> when the specified collection of lines look like <see cref="MarkdownEntry.Content"/>
    /// </summary>
    /// <param name="lines">collection of lines</param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    public static bool LookLikeMarkdownEntryWithFrontMatter(this IReadOnlyCollection<string>? lines, ILogger logger)
    {
        if (lines == null || lines.Count == 0)
        {
            logger.LogWarning("Warning: the expected lines are not here.");

            return false;
        }

        if (!lines.First().Trim().StartsWith(PublicationAppScalars.FrontMatterFence))
        {
            logger.LogWarning("Warning: The expected entry format is not here [front matter top].");

            return false;
        }

        if (!lines.Skip(1).Contains(PublicationAppScalars.FrontMatterFence))
        {
            logger.LogWarning("Warning: The expected entry format is not here [front matter bottom].");

            return false;
        }

        logger.LogInformation("Looks like a Publications entry. Returning...");

        return true;
    }

    /// <summary>
    /// Return <c>true</c> when the specified collection of lines look like YAML
    /// </summary>
    /// <param name="lines">collection of lines</param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    public static bool LookLikeYamlFrontMatter(this IReadOnlyCollection<string>? lines, ILogger logger)
    {
        if (lines == null || lines.Count == 0)
        {
            logger.LogWarning("Warning: the expected lines are not here.");

            return false;
        }

        logger.LogInformation("Scanning front matter lines for YAML-like format...");

        if (lines.LookLikeJsonFrontMatter(logger))
        {
            logger.LogWarning("Looks like JSON front matter! Returning...");

            return false;
        }

        if (!lines.Skip(1).First().Trim().Contains(':'))
        {
            logger.LogInformation("Cannot find `:`. Does not look like well-formed YAML front matter. Returning...");

            return false;
        }

        logger.LogInformation("Looks like YAML front matter. Returning...");

        return true;
    }

    /// <summary>
    /// Converts the specified <see cref="FileInfo"/> publications entry
    /// into two collections of lines: one for front matter and one for content.
    /// </summary>
    /// <param name="entry">the <see cref="FileInfo"/> publications entry</param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    /// <remarks>
    /// This member expects the first line of the entry to be a front matter fence (<c>---</c>)
    /// or it will return empty collections.
    /// </remarks>
    public static (IReadOnlyCollection<string> FrontMatterLines, IReadOnlyCollection<string> ContentLines)
        ToFrontMatterLinesAndContentLines(this FileInfo? entry, ILogger logger)
    {
        if (entry == null)
        {
            logger.LogError("Error: the expected entry is not here.");

            return (Array.Empty<string>(), Array.Empty<string>());
        }

        string[] lines = File.ReadAllLines(entry.FullName);

        if (!lines.LookLikeMarkdownEntryWithFrontMatter(logger))
        {
            logger.LogWarning("Warning: the expected markdown entry lines are not here. Returning {Count} lines as content...", lines.Length);

            return (Array.Empty<string>(), lines.ToArray());
        }

        string[] frontMatterLines = lines
            .Skip(1)
            .TakeWhile(i => !i.Contains(PublicationAppScalars.FrontMatterFence))
            .ToArray();
        string[] contentLines = lines
            .Skip(1)
            .SkipWhile(line => !line.Trim().EqualsInvariant(PublicationAppScalars.FrontMatterFence))
            .Skip(1)
            .ToArray();

        logger.LogInformation("Found {Count} front-matter lines and {Count} content lines. Returning...",
            frontMatterLines.Length, contentLines.Length);

        return (frontMatterLines, contentLines);
    }

    /// <summary>
    /// Converts the specified <see cref="FileInfo"/> to a new <see cref="IDocument"/>.
    /// </summary>
    /// <param name="entry">the <see cref="FileInfo"/> publications entry</param>
    /// <param name="title">the <see cref="IDocument.Title"/></param>
    /// <param name="documentPath">the <see cref="IDocument.Path"/></param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    public static IDocument? ToNewIDocument(this FileInfo? entry, string? title, string? documentPath, ILogger logger)
    {
        if (entry == null)
        {
            logger.LogError("Error: the expected entry is not here.");

            return null;
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            logger.LogError("Error: the expected title is not here.");

            return null;
        }

        var inceptDate = DateTime.Now;

        IDocument data = new Document
        {
            FileName = entry.Name,
            IsActive = true,
            InceptDate = inceptDate,
            ModificationDate = inceptDate,
            Path = documentPath,
            Title = title
        };

        return data;
    }

    /// <summary>
    /// Converts the specified <see cref="FileInfo"/>
    /// into a (<see cref="IDocument"/>, <see cref="string"/>) tuple.
    /// </summary>
    /// <param name="entry">the <see cref="FileInfo"/> publications entry</param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    public static (IDocument? frontMatter, string? content)
        ToIDocumentAndAnyContent(this FileInfo? entry, ILogger logger)
    {
        var (jO, content) = entry.ToJsonObjectAndAnyContent(logger);

        jO = jO
            .WithoutConventionalDocumentProperties(logger)
            .WithoutNonNullableDocumentProperties(logger)
            .WithPropertiesRenamed(logger, ("tags", "keywords"));

        IDocument? frontMatter = jO != null ?
            JsonSerializer.Deserialize<Document>(jO.ToJsonString())
            :
            null;

        return (frontMatter, content);
    }

    /// <summary>
    /// Converts the specified <see cref="FileInfo"/>
    /// into a (<see cref="JsonObject"/>, <see cref="string"/>) tuple.
    /// </summary>
    /// <param name="entry">the <see cref="FileInfo"/> publications entry</param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    public static (JsonObject? frontMatter, string? content)
        ToJsonObjectAndAnyContent(this FileInfo? entry, ILogger logger)
    {
        var (frontMatterLines, contentLines) =
            entry.ToFrontMatterLinesAndContentLines(logger);

        string? json = frontMatterLines.LookLikeYamlFrontMatter(logger)
            ? YamlUtility.DeserializeYaml(frontMatterLines.ToYamlString(logger)).ToJsonString()
            : 
            frontMatterLines.LookLikeJsonFrontMatter(logger)
                ? frontMatterLines.ToJsonString(logger)
                :
                null;

        JsonObject? frontMatter = string.IsNullOrWhiteSpace(json) ? null : JsonNode.Parse(json).ToJsonObject(logger);
        string content = contentLines.Aggregate((a, line) => $"{a}{Environment.NewLine}{line}");

        return (frontMatter, content);
    }

    /// <summary>
    /// Converts the specified collection of lines to a JSON string.
    /// </summary>
    /// <param name="lines">collection of lines where <see cref="LookLikeJsonFrontMatter"/> returns <c>true</c></param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    public static string? ToJsonString(this IReadOnlyCollection<string>? lines, ILogger logger)
    {
        if (!lines.LookLikeJsonFrontMatter(logger)) return null;

        return lines?.Aggregate(string.Concat);
    }

    /// <summary>
    /// Converts the specified collection of lines to a YAML string.
    /// </summary>
    /// <param name="lines">collection of lines where <see cref="LookLikeYamlFrontMatter"/> returns <c>true</c></param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    public static string? ToYamlString(this IReadOnlyCollection<string>? lines, ILogger logger)
    {
        if (!lines.LookLikeYamlFrontMatter(logger)) return null;

        return lines?.Aggregate((a, line) => string.Concat(a, MarkdownEntry.NewLine, line));
    }

    /// <summary>
    /// Writes a new entry with JSON front matter to <see cref="FileSystemInfo.FullName"/>
    /// </summary>
    /// <param name="entry">the <see cref="FileInfo"/> publications entry</param>
    /// <param name="title">the <see cref="IDocument.Title"/></param>
    /// <param name="documentPath">the <see cref="IDocument.Path"/></param>
    /// <param name="content">the entry content</param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    public static void WriteNewPublicationEntryWithJsonFrontMatter(this FileInfo? entry, string? title, string? documentPath, string? content, ILogger logger) =>
        entry
            .ToNewIDocument(title, documentPath, logger)
            .WritePublicationEntryWithJsonFrontMatter(entry?.FullName, content, logger);

    /// <summary>
    /// Writes a new entry with YAML front matter to <see cref="FileSystemInfo.FullName"/>
    /// </summary>
    /// <param name="entry">the <see cref="FileInfo"/> publications entry</param>
    /// <param name="title">the <see cref="IDocument.Title"/></param>
    /// <param name="documentPath">the <see cref="IDocument.Path"/></param>
    /// <param name="content">the entry content</param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    public static void WriteNewPublicationEntryWithYamlFrontMatter(this FileInfo? entry, string? title, string? documentPath, string? content, ILogger logger) =>
        entry
            .ToNewIDocument(title, documentPath, logger)
            .WritePublicationEntryWithYamlFrontMatter(entry?.FullName, content, logger);
}
