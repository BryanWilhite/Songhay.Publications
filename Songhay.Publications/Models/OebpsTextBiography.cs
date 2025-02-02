namespace Songhay.Publications.Models;

/// <summary>
/// Defines the content to write the
/// <see cref="PublicationFiles.EpubFileBiography"/> file.
/// </summary>
/// <remarks>
///  Open eBook Publication Structure (OEBPS),
///  is a legacy e-book format which
///  has been superseded by the EPUB format.
///
/// https://en.wikipedia.org/wiki/Open_eBook
/// </remarks>
public class OebpsTextBiography
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OebpsTextBiography"/> class.
    /// </summary>
    /// <param name="templateRoot">the root directory of the EPUB template files</param>
    /// <param name="epubTextDirectory">conventional <c>epub/OEBPS/Text</c> directory</param>
    /// <param name="markdownDirectory">conventional <c>markdown</c> directory</param>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public OebpsTextBiography(string templateRoot, string epubTextDirectory, string markdownDirectory, ILogger? logger)
    {
        _logger = logger;

        _epubTextDirectory = epubTextDirectory;
        _markdownDirectory = markdownDirectory;
        _biographyTemplate = GetBiographyTemplate(templateRoot);
    }

    /// <summary>
    /// Writes the
    /// <see cref="PublicationFiles.EpubFileBiography"/> file.
    /// </summary>
    public void Write()
    {
        var xhtml = PublicationNamespaces.Xhtml;

        var xhtmlFile = ProgramFileUtility.GetCombinedPath(_epubTextDirectory, PublicationFiles.EpubFileBiography, fileIsExpected: true);
        var markdownFile = ProgramFileUtility.GetCombinedPath(_markdownDirectory, PublicationFiles.EpubMarkdownBiography, fileIsExpected: true);

        _logger?.LogInformation("    markdown file `{Path}`...", markdownFile);
        var markdown = File.ReadAllText(markdownFile);
        var raw = Markdown.ToHtml(markdown);
        var rawElement = XElement.Parse($@"<div class=""rx raw tmp"" xmlns=""{xhtml}"">{raw}</div>");
        var biographyDocument = new XDocument(_biographyTemplate);
        var divElement = biographyDocument.Root?
            .Element(xhtml + "body")?
            .Element(xhtml + "div")?
            .Element(xhtml + "div")
            .ToReferenceTypeValueOrThrow();

        divElement?.ReplaceWith(rawElement.Nodes());

        EpubUtility.SaveAsUnicodeWithBom(biographyDocument, xhtmlFile);
    }

    internal static XDocument GetBiographyTemplate(string csxRoot)
    {
        var biographyTemplateFile = ProgramFileUtility.GetCombinedPath(csxRoot, PublicationFiles.EpubTemplateBiography, fileIsExpected: true);

        return XDocument.Load(biographyTemplateFile);
    }

    readonly ILogger? _logger;
    readonly string _epubTextDirectory;
    readonly string _markdownDirectory;
    readonly XDocument _biographyTemplate;
}
