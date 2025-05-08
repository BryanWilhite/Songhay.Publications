namespace Songhay.Publications.Models;

/// <summary>
/// Defines the content to write the
/// <see cref="PublicationFiles.EpubFileDedication"/> file.
/// </summary>
/// <remarks>
///  Open eBook Publication Structure (OEBPS),
///  is a legacy e-book format which
///  has been superseded by the EPUB format.
///
/// https://en.wikipedia.org/wiki/Open_eBook
/// </remarks>
public class OebpsTextDedication
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OebpsTextDedication"/> class.
    /// </summary>
    /// <param name="templateRoot">the root directory of the EPUB template files</param>
    /// <param name="epubTextDirectory">conventional <c>epub/OEBPS/Text</c> directory</param>
    /// <param name="markdownDirectory">conventional <c>markdown</c> directory</param>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public OebpsTextDedication(string templateRoot, string epubTextDirectory, string markdownDirectory, ILogger logger)
    {
        _logger = logger;

        _epubTextDirectory = epubTextDirectory;
        _markdownDirectory = markdownDirectory;
        _dedicationTemplate = GetTemplate(templateRoot);
    }

    /// <summary>
    /// Writes the
    /// <see cref="PublicationFiles.EpubFileDedication"/> file.
    /// </summary>
    public void Write()
    {
        XNamespace xhtml = PublicationNamespaces.Xhtml;

        string xhtmlFile = ProgramFileUtility.GetCombinedPath(_epubTextDirectory, PublicationFiles.EpubFileDedication, fileIsExpected: true);
        string markdownFile = ProgramFileUtility.GetCombinedPath(_markdownDirectory, PublicationFiles.EpubMarkdownDedication, fileIsExpected: true);

        _logger.LogInformation("    markdown file `{Path}`...", markdownFile);
        string markdown = File.ReadAllText(markdownFile);
        string raw = Markdown.ToHtml(markdown);
        XElement rawElement = XElement.Parse($@"<div class=""rx raw tmp"" xmlns=""{xhtml}"">{raw}</div>");
        XDocument dedicationDocument = new XDocument(_dedicationTemplate);
        XElement? divElement = dedicationDocument.Root?
            .Element(xhtml + "body")?
            .Element(xhtml + "div")?
            .Element(xhtml + "div")
            .ToReferenceTypeValueOrThrow();

        divElement?.ReplaceWith(rawElement.Nodes());

        EpubUtility.SaveAsUnicodeWithBom(dedicationDocument, xhtmlFile);
    }

    internal static XDocument GetTemplate(string csxRoot)
    {
        var dedicationTemplateFile = ProgramFileUtility.GetCombinedPath(csxRoot, PublicationFiles.EpubTemplateDedication, fileIsExpected: true);

        return XDocument.Load(dedicationTemplateFile);
    }

    private readonly ILogger _logger;
    private readonly string _epubTextDirectory;
    private readonly string _markdownDirectory;
    private readonly XDocument _dedicationTemplate;
}
