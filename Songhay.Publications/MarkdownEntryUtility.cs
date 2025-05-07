namespace Songhay.Publications;

/// <summary>
/// Shares routines for <see cref="MarkdownEntry"/>.
/// </summary>
public static class MarkdownEntryUtility
{
    /// <summary>
    /// Generates or overwrites a <see cref="MarkdownEntry"/> file
    /// at the specified entry root.
    /// </summary>
    /// <param name="entryRoot">The conventional directory of <see cref="MarkdownEntry"/> drafts.</param>
    /// <param name="title">see <see cref="MarkdownEntryExtensions.WithNew11TyFrontMatter"/>.</param>
    public static MarkdownEntry GenerateEntryFor11Ty(string entryRoot, string title)
    {
        var tag = JsonNodeUtility.ConvertToJsonNode(new { extract = string.Empty })?.ToString();

        return GenerateEntryFor11Ty(entryRoot, title, DateTime.Now, "./entry/", tag);
    }

    /// <summary>
    /// Generates or overwrites a <see cref="MarkdownEntry"/> file
    /// at the specified entry root.
    /// </summary>
    /// <param name="entryRoot">The conventional directory of <see cref="MarkdownEntry"/> drafts.</param>
    /// <param name="title">see <see cref="MarkdownEntryExtensions.WithNew11TyFrontMatter"/>.</param>
    /// <param name="inceptDate">see <see cref="MarkdownEntryExtensions.WithNew11TyFrontMatter"/>.</param>
    /// <param name="path">see <see cref="MarkdownEntryExtensions.WithNew11TyFrontMatter"/>.</param>
    /// <param name="tag">see <see cref="MarkdownEntryExtensions.WithNew11TyFrontMatter"/>.</param>
    public static MarkdownEntry GenerateEntryFor11Ty(string? entryRoot, string? title, DateTime inceptDate, string? path,
        string? tag)
    {
        if (!Directory.Exists(entryRoot))
            throw new DirectoryNotFoundException($"The expected entry root directory, `{entryRoot ?? "[null]"}`, is not here.");

        var entry = new MarkdownEntry()
            .WithNew11TyFrontMatter(title, inceptDate, path, tag)
            .WithContentHeader();

        File.WriteAllText($"{entryRoot}/{entry.FrontMatter["clientId"]}.md", entry.ToFinalEdit());

        return entry;
    }

    /// <summary>
    /// Publishes a <see cref="MarkdownEntry"/>
    /// from the specified entry root
    /// to the specified presentation root
    /// for the eleventy pipeline.
    /// </summary>
    /// <param name="entryRoot">The conventional directory of <see cref="MarkdownEntry"/> drafts.</param>
    /// <param name="presentationRoot">The presentation target directory for publication.</param>
    /// <param name="fileName">The name of the <see cref="MarkdownEntry"/> file in the entry directory.</param>
    /// <returns>
    /// Returns the path of the published file.
    /// </returns>
    public static string PublishEntryFor11Ty(string entryRoot, string presentationRoot, string fileName) =>
        PublishEntryFor11Ty(entryRoot, presentationRoot, fileName, DateTime.Now);

    /// <summary>
    /// Publishes a <see cref="MarkdownEntry"/>
    /// from the specified entry root
    /// to the specified presentation root
    /// for the eleventy pipeline.
    /// </summary>
    /// <param name="entryRoot">The conventional directory of <see cref="MarkdownEntry"/> drafts.</param>
    /// <param name="presentationRoot">The presentation target directory for publication.</param>
    /// <param name="fileName">The name of the <see cref="MarkdownEntry"/> file in the entry directory.</param>
    /// <param name="publicationDate">The <see cref="DateTime"/> of publication.</param>
    /// <returns>
    /// Returns the path of the published file.
    /// </returns>
    /// <remarks>
    /// When the publication date is one day later or more than the entry incept date
    /// new eleventy <see cref="MarkdownEntry.FrontMatter"/> will be generated
    /// and the presentation file will be renamed accordingly.
    /// </remarks>
    public static string PublishEntryFor11Ty(string entryRoot, string presentationRoot, string fileName,
        DateTime publicationDate)
    {
        if (!Directory.Exists(entryRoot))
            throw new DirectoryNotFoundException($"The expected entry root directory, `{entryRoot}`, is not here.");

        if (!Directory.Exists(presentationRoot))
            throw new DirectoryNotFoundException($"The expected presentation root directory, `{presentationRoot}`, is not here.");

        if (string.IsNullOrWhiteSpace(fileName))
            throw new NullReferenceException("The expected file name is not here.");

        if (!fileName.EndsWith(".md"))
            throw new FormatException("The expected file name format, `*.md`, is not here.");

        var rootInfo = new DirectoryInfo(entryRoot);

        var draftInfo = rootInfo.GetFiles().FirstOrDefault(i => i.Name.EqualsInvariant(fileName));
        if (draftInfo == null)
            throw new FileNotFoundException($"The expected file, `{fileName}`, under `{rootInfo.FullName}` is not here.");

        var draftEntry = draftInfo.ToMarkdownEntry();
        var inceptDate = draftEntry.FrontMatter["date"].ToReferenceTypeValueOrThrow().GetValue<DateTime>();
        var clientId = draftEntry.FrontMatter["clientId"].ToReferenceTypeValueOrThrow().GetValue<string>();
        var path = draftEntry.FrontMatter["path"]?.GetValue<string>().Replace(clientId, string.Empty);

        if ((publicationDate - inceptDate).Days >= 1)
        {
            var title = draftEntry.FrontMatter["title"]?.GetValue<string>();
            var tag = draftEntry.FrontMatter["tag"]?.GetValue<string>();
            draftEntry.WithNew11TyFrontMatter(title, publicationDate, path, tag);
        }

        var combinedPath = ProgramFileUtility.GetCombinedPath(presentationRoot,
            $"{draftEntry.FrontMatter["clientId"].ToReferenceTypeValueOrThrow().GetValue<string>()}.md");
        File.WriteAllText(combinedPath, draftEntry.ToFinalEdit());
        draftInfo.Delete();

        return combinedPath;
    }
}
