using System.Text.RegularExpressions;

namespace Songhay.Publications;

/// <summary>
/// Shared routines for Publication Lines.
/// </summary>
public static class PublicationLinesUtility
{
    /// <summary>
    /// Converts the specified content lines
    /// to a Publications extract.
    /// </summary>
    /// <param name="contentLines">the collection of content lines</param>
    /// <param name="length">The string-length of the extract.</param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    public static string? ConvertToExtract(IReadOnlyCollection<string>? contentLines, int length, ILogger logger)
    {
        if (contentLines == null) return null;

        logger.LogInformation("Converting content lines to extract...");

        logger.LogInformation("Skipping beginning content lines with markdown headers and line breaks...");
        string content = contentLines
            .SkipWhile(line => Regex.IsMatch(line, @"^[\n\r\#]"))
            .Aggregate(string.Empty, (a, i) => $"{a}{MarkdownEntry.NewLine}{i}");

        logger.LogInformation("Removing any `div` blocks...");
        content = Regex.Replace(content, @"<div[^>]+>(.|\n)+?</div>\n", string.Empty);

        logger.LogInformation("Removing any self-closing elements...");
        content = Regex.Replace(content, @"<[^>]+>", string.Empty);

        logger.LogInformation("Removing any headers that were not at the beginning...");
        content = Regex.Replace(content, @"#[^#\n]+", string.Empty);

        logger.LogInformation("Removing any notification blocks...");
        content = Regex.Replace(content, @">[^>\n]+", string.Empty);

        logger.LogInformation("Calling {Class}.{Method}...", nameof(Markdown), nameof(Markdown.ToPlainText));
        content = Markdown.ToPlainText(content).Replace("\n", " ").Replace("\r", " ").Replace("  ", " ").Trim();

        return content.Length > length ? string.Concat(content[..length], "…") : content; 
    }

    /// <summary>
    /// Converts the specified <see cref="string"/>
    /// into Publication lines.
    /// </summary>
    /// <param name="content">the content <see cref="string"/></param>
    public static IReadOnlyCollection<string> ConvertToLines(string? content)
    {
        string[] delimiter = [$"{MarkdownEntry.NewLine}{MarkdownEntry.NewLine}"];

        string[]? lines = content?.Trim()
            .Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

        return lines ?? Enumerable.Empty<string>().ToArray();
    }
}
