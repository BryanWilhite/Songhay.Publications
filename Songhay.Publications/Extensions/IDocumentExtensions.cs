using FluentValidation.Results;

namespace Songhay.Publications.Extensions;

/// <summary>
/// Extensions of <see cref="IDocument"/>
/// </summary>
// ReSharper disable once InconsistentNaming
public static class IDocumentExtensions
{
    static IDocumentExtensions() => TraceSource = TraceSources
        .Instance
        .GetTraceSourceFromConfiguredName()
        .WithSourceLevels();

    static readonly TraceSource? TraceSource;

    /// <summary>
    /// Clones the instance of <see cref="IDocument"/>.
    /// </summary>
    /// <param name="data">The document.</param>
    public static Document? Clone(this IDocument? data) => data?.GetClone(CloneInitializers.Publications) as Document;

    /// <summary>
    /// Returns and traces the first <see cref="IDocument"/>
    /// based on the specified predicate.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="predicate">The predicate.</param>
    public static IDocument? GetDocumentByPredicate(this IEnumerable<IDocument> data, Func<IDocument, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(data);

        var first = data.FirstOrDefault(predicate);

        TraceSource?.TraceVerbose($"{first?.ToDisplayText(showIdOnly: true)}");

        return first;
    }

    /// <summary>
    /// Returns <c>true</c> when the <see cref="IDocument"/>
    /// has any <see cref="Document.Fragments"/>.
    /// </summary>
    /// <param name="data">The data.</param>
    public static bool HasFragments(this IDocument? data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));

        if (data is not Document document) return false;

        if (document.Fragments.Any()) return true;

        TraceSource?.TraceError($"The expected child {nameof(Document.Fragments)} are not here.");

        return false;
    }

    /// <summary>
    /// Determines whether the specified document is template-able.
    /// </summary>
    /// <param name="data">The document.</param>
    /// <returns>
    ///   <c>true</c> if the specified document is template-able; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsTemplatable(this IDocument? data) =>
        data != null && !string.IsNullOrEmpty(data.FileName) && data.FileName.EndsWith(".html");

    /// <summary>
    /// Sets the defaults.
    /// </summary>
    /// <param name="data">The document.</param>
    public static void SetDefaults(this IDocument? data)
    {
        if (data == null) return;

        data.InceptDate = DateTime.Now;
        data.IsActive = true;
        data.IsRoot = false;
        data.ModificationDate = DateTime.Now;
    }

    /// <summary>
    /// Converts <see cref="Document"/> to the conventional publication item.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="templateFileName">Name of the template file.</param>
    public static XElement? ToConventionalPublicationItem(this IDocument? data, string? templateFileName)
    {
        if (data == null) return null;
        if (string.IsNullOrEmpty(templateFileName)) return null;

        return new XElement("item",
            new XAttribute(nameof(Document.SegmentId), data.SegmentId.GetValueOrDefault()),
            new XAttribute(nameof(Document.DocumentId), data.DocumentId.GetValueOrDefault()),
            new XAttribute(nameof(Document.Title), data.Title ?? $"[{nameof(Document.Title)} is null]"),
            new XAttribute("Template", templateFileName),
            new XAttribute("PathAndFileName", string.Concat(data.Path, data.FileName)),
            new XAttribute(nameof(Document.IsRoot), data.IsRoot.GetValueOrDefault()));
    }

    /// <summary>
    /// Converts the <see cref="IDocument"/> into human-readable display text.
    /// </summary>
    /// <param name="data">The data.</param>
    public static string ToDisplayText(this IDocument? data) => data.ToDisplayText(showIdOnly: false);

    /// <summary>
    /// Converts the <see cref="IDocument"/> into a display text.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="showIdOnly">when <c>true</c> then display identifiers only</param>
    public static string ToDisplayText(this IDocument? data, bool showIdOnly)
    {
        if (data == null)
            return $"{nameof(ToDisplayText)}: the specified {nameof(IDocument)} is null.";

        var builder = new StringBuilder();

        var delimiter = string.Empty;

        if (data.DocumentId.HasValue)
        {
            builder.Append($"{nameof(data.DocumentId)}: {data.DocumentId}");
            delimiter = ", ";
        }

        if (!string.IsNullOrWhiteSpace(data.ClientId))
        {
            builder.Append($"{delimiter}{nameof(data.ClientId)}: {data.ClientId}");
            delimiter = ", ";
        }

        if (showIdOnly) return builder.ToString();

        if (!string.IsNullOrEmpty(data.Title))
            builder.Append($"{delimiter}{nameof(data.Title)}: {data.Title}");

        if (data.IsActive.HasValue)
            builder.Append($"{delimiter}{nameof(data.IsActive)}: {data.IsActive}");

        if (data.IsRoot.HasValue)
            builder.Append($"{delimiter}{nameof(data.IsRoot)}: { data.IsRoot}");

        if (!string.IsNullOrEmpty(data.FileName))
            builder.Append($"{delimiter}{nameof(data.Path)}: {data.Path}{delimiter}{nameof(data.FileName)}: {data.FileName}");

        if (!string.IsNullOrEmpty(data.DocumentShortName))
            builder.Append($"{delimiter}{nameof(data.DocumentShortName)}: {data.DocumentShortName}");

        return builder.ToString();
    }

    /// <summary>
    /// Converts the specified <see cref="IDocument"/> to <see cref="MarkdownEntry"/>.
    /// </summary>
    /// <param name="document">the <see cref="IDocument"/></param>
    /// <param name="entryPath">the entry path</param>
    /// <param name="content">the entry content</param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    public static MarkdownEntry? ToMarkdownEntry(this IDocument? document, string? entryPath, string? content, ILogger logger)
    {
        if (document == null)
        {
            logger.LogError("Error: the expected document is not here.");

            return null;
        }

        if (string.IsNullOrWhiteSpace(entryPath))
        {
            logger.LogError("Error: the expected entry path is not here.");

            return null;
        }

        JsonObject? jO = JsonNodeUtility.ConvertToJsonNode(document)?.ToJsonObject(logger).WithoutConventionalDocumentProperties(logger);

        if (jO == null) return null;

        content ??= $@"
# {document.Title}
        ".Trim();

        return new MarkdownEntry
        {
            Content = content,
            FrontMatter = jO,
            EntryFileInfo = new FileInfo(entryPath)
        };
    }

    /// <summary>
    /// Converts the <see cref="IDocument"/> into a menu display item model.
    /// </summary>
    /// <param name="data">The document.</param>
    public static MenuDisplayItemModel? ToMenuDisplayItemModel(this IDocument? data) =>
        data.ToMenuDisplayItemModel(group: null);

    /// <summary>
    /// Converts the <see cref="IDocument"/> into a menu display item model.
    /// </summary>
    /// <param name="data">The document.</param>
    /// <param name="group">The group.</param>
    public static MenuDisplayItemModel? ToMenuDisplayItemModel(this IDocument? data, IGroupable? group)
    {
        if (data == null) return null;

        var @namespace = typeof(PublicationContext).Namespace;

        var dataOut = new MenuDisplayItemModel()
        {
            DisplayText = data.Title,
            GroupDisplayText = (group == null) ? $"{@namespace}.{nameof(Document)}" : group.GroupDisplayText,
            GroupId = (group == null) ? $"{@namespace}.{nameof(Document)}".ToLowerInvariant() : group.GroupId,
            Id = data.DocumentId.GetValueOrDefault(),
            ItemName = data.DocumentShortName
        };

        return dataOut;
    }

    /// <summary>
    /// Converts the <see cref="IDocument"/> data to <see cref="ValidationResult"/>.
    /// </summary>
    /// <param name="data">the <see cref="IDocument"/> data</param>
    public static ValidationResult ToValidationResult(this IDocument? data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (data is not Document instance)
            throw new DataException($"The expected {nameof(Document)} data is not here.");

        var validator = new DocumentValidator();

        return validator.Validate(instance);
    }

    /// <summary>
    /// Converts the specified <see cref="IDocument"/>
    /// to well-formed YAML.
    /// </summary>
    /// <param name="document">the <see cref="IDocument"/></param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    public static string? ToYaml(this IDocument? document, ILogger logger) => document.ToYaml(contentLines: null, logger);

    /// <summary>
    /// Converts the specified <see cref="IDocument"/>
    /// to well-formed YAML.
    /// </summary>
    /// <param name="document">the <see cref="IDocument"/></param>
    /// <param name="contentLines">the collection of content lines</param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    public static string? ToYaml(this IDocument? document, IReadOnlyCollection<string>? contentLines, ILogger logger)
    {
        if (document == null) return null;

        ISerializer serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithAttributeOverride<IDocument>(d => d.Tag!, new YamlIgnoreAttribute())
            .Build();

        logger.LogInformation("Serializing to YAML (ignoring the {Name} property...", nameof(IDocument.Tag));

        string yaml = serializer.Serialize(document, typeof(IDocument));

        logger.LogInformation("Trying to parse the {Name} property as JSON (`{Json}`)...", nameof(IDocument.Tag), document.Tag.Truncate(32));

        JsonObject? jO = null;
        bool parsedJsonObject = false;

        if (!string.IsNullOrWhiteSpace(document.Tag))
        {
            try
            {
                jO = JsonNode.Parse(document.Tag)?.AsObject();
                parsedJsonObject = true;
            }
            catch (Exception e)
            {
                logger.LogError("Error: JSON parsing failed! Message: `{Message}` Returning...", e.Message);
            }
        }

        jO ??= new JsonObject();

        string? additionalYaml = jO
            .WithExtract(contentLines, 255, logger)
            .WithPropertiesRenamed(logger, ("keywords", "tags"))
            .ToYaml(logger);

        yaml = string.Concat(yaml, additionalYaml);

        if (!parsedJsonObject && !string.IsNullOrWhiteSpace(document.Tag))
        {
            logger.LogInformation("The {Name} property is probably not JSON. Content will be treated as YAML tags..", nameof(IDocument.Tag));
            string[] tags = document.Tag.Split(',', ';', '|', ' ');
            yaml = string.Concat(yaml, "tags: [ ", string.Join(',', tags), " ]");
        }

        return yaml.Trim();
    }

    /// <summary>
    /// Returns <see cref="IDocument"/> with default values.
    /// </summary>
    /// <param name="data">The data.</param>
    public static IDocument? WithDefaults(this IDocument? data)
    {
        data.SetDefaults();

        return data;
    }

    /// <summary>
    /// Returns <see cref="IDocument" />
    /// after the specified edit <see cref="Action{IDocument}" />.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="editAction">The edit action.</param>
    public static IDocument WithEdit(this IDocument? data, Action<IDocument>? editAction)
    {
        editAction?.Invoke(data.ToReferenceTypeValueOrThrow());

        return data.ToReferenceTypeValueOrThrow();
    }

    /// <summary>
    /// Writes an entry with JSON front matter to the specified entry path.
    /// </summary>
    /// <param name="document">the <see cref="IDocument"/></param>
    /// <param name="entryPath">the entry path</param>
    /// <param name="content">the entry content</param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    public static void WritePublicationEntryWithJsonFrontMatter(this IDocument? document, string? entryPath, string? content, ILogger logger)
    {
        MarkdownEntry? entry = document.ToMarkdownEntry(entryPath, content, logger);

        if (entry?.EntryFileInfo == null) return;

        logger.LogInformation("Writing to `{Path}`...", entryPath);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        string json = entry.FrontMatter.ToJsonString(options);
        string frontMatter = $@"
{PublicationAppScalars.FrontMatterFence}json
{json}
{PublicationAppScalars.FrontMatterFence}
".Trim();
        if (string.IsNullOrWhiteSpace(content))
        {
            content = $@"
{frontMatter}
# {document?.Title}
".TrimStart();
        }
        else if (!content.TrimStart().StartsWith(PublicationAppScalars.FrontMatterFence))
        {
            content = $@"
{frontMatter}
{content.Trim()}
".TrimStart();
        }

        File.WriteAllText(entry.EntryFileInfo.FullName, content);
    }

    /// <summary>
    /// Writes an entry with YAML front matter to the specified entry path.
    /// </summary>
    /// <param name="document">the <see cref="IDocument"/></param>
    /// <param name="entryPath">the entry path</param>
    /// <param name="content">the entry content</param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    public static void WritePublicationEntryWithYamlFrontMatter(this IDocument? document, string? entryPath, string? content, ILogger logger)
    {
        if (document == null)
        {
            logger.LogError("Error: the expected document is not here.");

            return;
        }

        if (string.IsNullOrWhiteSpace(entryPath))
        {
            logger.LogError("Error: the expected entry path is not here.");

            return;
        }

        logger.LogInformation("Writing to `{Path}`...", entryPath);

        string yaml = document.ToYaml(PublicationLinesUtility.ConvertToLines(content), logger) ?? string.Empty;
        string frontMatter = $@"
{PublicationAppScalars.FrontMatterFence}
{yaml}
{PublicationAppScalars.FrontMatterFence}
".Trim();

        if (string.IsNullOrWhiteSpace(content))
        {
            content = $@"
{frontMatter}
# {document.Title}
".TrimStart();
        }
        else if (!content.TrimStart().StartsWith(PublicationAppScalars.FrontMatterFence))
        {
            content = $@"
{frontMatter}
{content.Trim()}
".TrimStart();
        }

        File.WriteAllText(entryPath, content);
    }
}
