using System.Text.RegularExpressions;

namespace Songhay.Publications.Extensions;

/// <summary>
/// Extensions of <see cref="MarkdownEntry" />.
/// </summary>
public static partial class MarkdownEntryExtensions
{
    /// <summary>
    /// Effectively validates <see cref="MarkdownEntry" />.
    /// </summary>
    /// <param name="entry">The <see cref="MarkdownEntry" /> entry.</param>
    public static MarkdownEntry DoNullCheck(this MarkdownEntry? entry) =>
        entry.ToReferenceTypeValueOrThrow().DoNullCheckForContent();

    /// <summary>
    /// Effectively validates <see cref="MarkdownEntry.Content" />.
    /// </summary>
    /// <param name="entry">The <see cref="MarkdownEntry" /> entry.</param>
    public static MarkdownEntry DoNullCheckForContent(this MarkdownEntry? entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        return string.IsNullOrWhiteSpace(entry.Content)
            ? throw new NullReferenceException($"The expected {nameof(MarkdownEntry.Content)} is not here.")
            : entry;
    }

    /// <summary>
    /// Converts the <see cref="MarkdownEntry"/>
    /// to an extract of the specified length
    /// </summary>
    /// <param name="entry">The <see cref="MarkdownEntry" />.</param>
    /// <param name="length">The string-length of the extract.</param>
    public static string ToExtract(this MarkdownEntry? entry, int length)
    {
        string[] paragraphs = entry.ToParagraphs();
        int skip = paragraphs.Length > 1 ? 1 : 0;
        string content = paragraphs.Skip(skip).Aggregate(string.Empty, (a, i) => $"{a} {i}");
        content = MarkupRegex().Replace(content, string.Empty);
        content = Markdown.ToPlainText(content);

        return content.Length > length ?
            string.Concat(content[..length], "…") :
            content;
    }

    /// <summary>
    /// Converts the specified <see cref="MarkdownEntry" />
    /// into the final edit <see cref="String" />
    /// </summary>
    /// <param name="entry">The <see cref="MarkdownEntry" /> entry.</param>
    public static string ToFinalEdit(this MarkdownEntry? entry)
    {
        entry.DoNullCheck();

        string finalEdit = string.Concat(
            "---json",
            MarkdownEntry.NewLine,
            entry?.FrontMatter.ToString().Trim(),
            MarkdownEntry.NewLine,
            "---",
            MarkdownEntry.NewLine,
            MarkdownEntry.NewLine,
            entry?.Content?.Trim(),
            MarkdownEntry.NewLine
        );

        return finalEdit;
    }

    /// <summary>
    /// Converts the specified <see cref="FileInfo" />
    /// into <see cref="MarkdownEntry" />
    /// </summary>
    /// <param name="entry">The <see cref="FileInfo" /> entry.</param>
    public static MarkdownEntry ToMarkdownEntry(this FileInfo entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        if (!File.Exists(entry.FullName)) throw new NullReferenceException($"The expected {nameof(FileInfo)} path is not here.");

        const string frontTop = "---json";
        const string frontBottom = "---";
        string[] lines = File.ReadAllLines(entry.FullName);

        if (lines.Length == 0) throw new FormatException($"File {entry.Name} is empty.");
        if (lines.First().Trim() != frontTop) throw new FormatException("The expected entry format is not here [front matter top].");
        if (!lines.Contains(frontBottom)) throw new FormatException("The expected entry format is not here [front matter bottom].");

        string json = lines
            .Skip(1)
            .TakeWhile(i => !i.Contains(frontBottom))
            .Aggregate((a, i) => $"{a}{MarkdownEntry.NewLine}{i}");

        string content = lines
            .SkipWhile(i => !i.Equals(frontBottom))
            .Skip(1)
            .Aggregate((a, i) => $"{a}{MarkdownEntry.NewLine}{i}");

        JsonObject frontMatter =
            JsonNodeUtility.ConvertToJsonNode(new { error = "front matter was not found", file = entry.FullName })
                .ToReferenceTypeValueOrThrow()
                .AsObject();

        try
        {
            frontMatter = JsonNode.Parse(json).ToReferenceTypeValueOrThrow().AsObject();
        }
        catch (JsonException ex)
        {
            frontMatter["error"] = JsonValue.Create(ex.Message);
        }

        MarkdownEntry mdEntry = new MarkdownEntry
        {
            EntryFileInfo = entry,
            FrontMatter = frontMatter,
            Content = content
        };

        return mdEntry;
    }

    /// <summary>
    /// Converts <see cref="MarkdownEntry.Content" /> to paragraphs
    /// </summary>
    /// <param name="entry">The <see cref="MarkdownEntry" /> entry.</param>
    public static string[] ToParagraphs(this MarkdownEntry? entry)
    {
        entry.DoNullCheck();

        var delimiter = new[] { $"{MarkdownEntry.NewLine}{MarkdownEntry.NewLine}" };

        var paragraphs = entry?.Content?.Trim()
            .Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

        return paragraphs ?? Enumerable.Empty<string>().ToArray();
    }

    /// <summary>
    /// Sets the modification date of the <see cref="MarkdownEntry" />.
    /// </summary>
    /// <param name="entry">The <see cref="MarkdownEntry" /> entry.</param>
    /// <param name="date">The touch <see cref="DateTime" />.</param>
    public static MarkdownEntry Touch(this MarkdownEntry? entry, DateTime date)
    {
        return entry.WithEdit(i =>
        {
            i.ToReferenceTypeValueOrThrow();

            i.FrontMatter["modificationDate"] = JsonValue.Create(date);
        });
    }

    /// <summary>
    /// Returns the <see cref="MarkdownEntry"/>
    /// with the conventional eleventy extract
    /// of the specified length.
    /// </summary>
    /// <param name="entry">The <see cref="MarkdownEntry" />.</param>
    /// <param name="length">The string-length of the extract.</param>
    public static MarkdownEntry With11TyExtract(this MarkdownEntry? entry, int length) =>
        entry.WithEdit(i =>
        {
            string? UpdateExtractAndReturnTag(string tag, string e)
            {
                const string extractPropertyName = "extract";

                JsonNode? jO = tag.TrimStart().StartsWith('{') ?
                    JsonNode.Parse(tag) :
                    JsonNodeUtility.ConvertToJsonNode(new { legacy = tag });

                if (!jO.HasProperty(extractPropertyName))
                {
                    jO?.AsObject().Add(extractPropertyName, null);
                }

                if(jO != null) jO[extractPropertyName] = e;

                return jO?.ToString();
            }

            const string tagPropertyName = "tag";
            var tagString = i.FrontMatter[tagPropertyName].ToReferenceTypeValueOrThrow()
                .GetValue<string>();

            var extract = i.ToExtract(length);

            i.ToReferenceTypeValueOrThrow().FrontMatter[tagPropertyName] = string.IsNullOrWhiteSpace(tagString) ?
                    JsonNodeUtility.ConvertToJsonNode(new { extract })?.ToString()
                    :
                    UpdateExtractAndReturnTag(tagString, extract)
                ;
        });

    /// <summary>
    /// Returns the <see cref="MarkdownEntry"/>
    /// based on <see cref="MarkdownEntry.FrontMatter"/>
    /// with a <c>title</c> property.
    /// </summary>
    /// <param name="entry">The <see cref="MarkdownEntry" /> entry.</param>
    public static MarkdownEntry WithContentHeader(this MarkdownEntry? entry) => entry.WithContentHeader(headerLevel: 1);

    /// <summary>
    /// Returns the <see cref="MarkdownEntry"/>
    /// based on <see cref="MarkdownEntry.FrontMatter"/>
    /// with a <c>title</c> property.
    /// </summary>
    /// <param name="entry">The <see cref="MarkdownEntry" /> entry.</param>
    /// <param name="headerLevel">The header level.</param>
    public static MarkdownEntry WithContentHeader(this MarkdownEntry? entry, int headerLevel)
    {
        entry.ToReferenceTypeValueOrThrow();

        const string propertyName = "title";

        if (!entry.FrontMatter.HasProperty(propertyName))
            throw new FormatException($"The expected date property, `{propertyName}`, is not here.");

        headerLevel = headerLevel == 0 ? 1 : Math.Abs(headerLevel);
        var markdownHeader = new string(Enumerable.Repeat('#', headerLevel > 6 ? 6 : headerLevel).ToArray());

        return entry.WithEdit(i =>
            i.Content = $"{markdownHeader} {i.FrontMatter[propertyName]}{MarkdownEntry.NewLine}{MarkdownEntry.NewLine}");
    }

    /// <summary>
    /// Edits the <see cref="MarkdownEntry" /> with the specified edit action.
    /// </summary>
    /// <param name="entry">The <see cref="MarkdownEntry" />.</param>
    /// <param name="editAction">the edit <see cref="Action{MarkdownEntry}" />.</param>
    public static MarkdownEntry WithEdit(this MarkdownEntry? entry, Action<MarkdownEntry>? editAction)
    {
        editAction?.Invoke(entry.ToReferenceTypeValueOrThrow());

        return entry.ToReferenceTypeValueOrThrow();
    }

    /// <summary>
    /// Returns the <see cref="MarkdownEntry" /> with conventional 11ty frontmatter.
    /// </summary>
    /// <param name="entry">The <see cref="MarkdownEntry" /> entry.</param>
    /// <param name="title">The title of th entry.</param>
    /// <param name="inceptDate">The incept date of the entry.</param>
    /// <param name="path">The path to the entry.</param>
    /// <param name="tag">The tag of the entry.</param>
    public static MarkdownEntry WithNew11TyFrontMatter(this MarkdownEntry? entry, string? title, DateTime inceptDate,
        string? path, string? tag) =>
        entry
            .ToReferenceTypeValueOrThrow()
            .WithNewFrontMatter(title, inceptDate,
                documentId: 0, fileName: "index.html", path: path, segmentId: 0, tag: tag)
            .WithEdit(i => i.FrontMatter["clientId"] = $"{inceptDate:yyyy-MM-dd}-{i.FrontMatter["clientId"]}")
            .WithEdit(i => i.FrontMatter["documentShortName"] = i.FrontMatter["clientId"]?.GetValue<string>())
            .WithEdit(i => i.FrontMatter["path"] = $"{i.FrontMatter["path"]}{i.FrontMatter["clientId"]}");

    /// <summary>
    /// Returns the <see cref="MarkdownEntry" /> with conventional frontmatter.
    /// </summary>
    /// <param name="entry">The <see cref="MarkdownEntry" /> entry.</param>
    /// <param name="title">The title of th entry.</param>
    /// <param name="inceptDate">The incept date of the entry.</param>
    /// <param name="documentId">the DBMS ID of the entry.</param>
    /// <param name="fileName">the file name (with extension) of the entry.</param>
    /// <param name="path">The path to the entry.</param>
    /// <param name="segmentId">the DBMS ID of the Publications Segment.</param>
    /// <param name="tag">The tag of the entry.</param>
    public static MarkdownEntry WithNewFrontMatter(this MarkdownEntry? entry, string? title, DateTime inceptDate,
        int documentId, string? fileName, string? path, int segmentId, string? tag)
    {
        ArgumentNullException.ThrowIfNull(entry);

        string slug = title.ToBlogSlug();
        tag ??= string.Empty;

        var fm = new
        {
            documentId,
            title,
            documentShortName = slug,
            fileName,
            path,
            date = inceptDate.ToIso8601String(),
            modificationDate = inceptDate.ToIso8601String(),
            templateId = 0,
            segmentId,
            isRoot = false,
            isActive = true,
            sortOrdinal = 0,
            clientId = slug,
            tag
        };

        entry.FrontMatter = JsonNodeUtility.ConvertToJsonNode(fm).ToReferenceTypeValueOrThrow().AsObject();

        return entry;
    }

    [GeneratedRegex(@"<[^>]+>")]
    private static partial Regex MarkupRegex();
}
