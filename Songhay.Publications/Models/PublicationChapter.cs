﻿namespace Songhay.Publications.Models;

/// <summary>
/// Defines content for the chapter
/// of an EPUB publication.
/// </summary>
public class PublicationChapter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PublicationChapter"/> class.
    /// </summary>
    /// <param name="chapterPair">The chapter pair.</param>
    /// <param name="chapterTemplate">The chapter template.</param>
    /// <param name="chapterDirectory">The chapter directory.</param>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public PublicationChapter(KeyValuePair<string, string> chapterPair, XDocument chapterTemplate, string chapterDirectory, ILogger logger)
    {
        _logger = logger;
        _chapterPair = chapterPair;
        _chapter = new XDocument(chapterTemplate);
        _chapterDirectoryInfo = new DirectoryInfo(chapterDirectory);
    }

    /// <summary>
    /// Gets the chapter body element.
    /// </summary>
    /// <returns></returns>
    public XElement? GetChapterBodyElement()
    {
        XNamespace xhtml = PublicationNamespaces.Xhtml;

        XElement? e = _chapter.Root?
            .Element(xhtml + "body")?
            .Element(xhtml + "div")?
            .Element(xhtml + "div");

        _logger.LogInformation("{Value}", e?.Value);

        return e;
    }

    /// <summary>
    /// Gets the h1 element.
    /// </summary>
    public XElement? GetH1Element()
    {
        XNamespace xhtml = PublicationNamespaces.Xhtml;

        XElement? e = _chapter.Root?
            .Element(xhtml + "body")?
            .Element(xhtml + "div")?
            .Element(xhtml + "h1");

        _logger.LogInformation("{Value}", e?.Value);

        return e;
    }

    /// <summary>
    /// Gets the title element.
    /// </summary>
    public XElement? GetTitleElement()
    {
        XNamespace xhtml = PublicationNamespaces.Xhtml;

        XElement? e = _chapter.Root?
            .Element(xhtml + "head")?
            .Element(xhtml + "title");

        _logger.LogInformation("{Value}", e?.Value);

        return e;
    }

    /// <summary>
    /// Generates the XHTML.
    /// </summary>
    public string GenerateXhtml()
    {
        XElement? titleElement = GetTitleElement();
        XElement? h1Element = GetH1Element();
        XElement? divPlaceholderElement = GetChapterBodyElement();

        StringBuilder chapterBodyBuilder = new StringBuilder();
        string divPageBreak = """<div style="page-break-before:always;"><span epub:type="pagebreak" /></div>""";

        _chapterDirectoryInfo
            .EnumerateFiles("*.md")
            .Select((markdownFile, i) => new { markdownFile, i })
            .ForEachInEnumerable(a =>
            {
                FileInfo markdownFile = a.markdownFile;
                int i = a.i;

                if (i > 0) chapterBodyBuilder.AppendLine(divPageBreak);

                _logger.LogInformation("    markdown file `{Path}`...", markdownFile);
                string markdown = File.ReadAllText(markdownFile.FullName);
                string raw = Markdown.ToHtml(markdown);
                XElement rawElement = XElement.Parse($"<raw>{raw}</raw>");

                _logger.LogInformation("    looking for h2 elements wrapped by p elements...");
                IEnumerable<XElement> h2Elements = rawElement.Descendants("h2");
                h2Elements.ToArray().ForEachInEnumerable(h2Element =>
                {
                    if (h2Element.Parent?.Name != "p") return;
                    h2Element.Parent.ReplaceWith(h2Element);
                });

                _logger.LogInformation("    looking for h3 elements wrapped by p elements...");
                IEnumerable<XElement> h3Elements = rawElement.Descendants("h3");
                h3Elements.ToArray().ForEachInEnumerable(h3Element =>
                {
                    if (h3Element.Parent?.Name != "p") return;
                    h3Element.Parent.ReplaceWith(h3Element);
                });

                _logger.LogInformation("    looking for white-space-preservation blocks...");
                IEnumerable<XElement> divElements = rawElement
                    .Elements("div")
                    .Where(div =>
                        div.Attribute("class").ToReferenceTypeValueOrThrow().Value.Contains("white-space-preservation"));

                divElements.ForEachInEnumerable(div =>
                {
                    const string u00A0 = " ";
                    const string twoSpaces = "  ";
                    IEnumerable<XElement> pElements = div.Elements("p").Where(p => p.Value.Contains(twoSpaces));
                    pElements.ForEachInEnumerable(p => p.Value = p.Value
                        .Replace("&#160;", u00A0)
                        .Replace(twoSpaces, string.Concat(u00A0, u00A0)));
                });

                rawElement.Elements().ForEachInEnumerable(e => chapterBodyBuilder.Append(e));
            });

        titleElement?.SetValue(_chapterPair.Value);
        h1Element?.SetValue(_chapterPair.Value);

        XNamespace xhtml = PublicationNamespaces.Xhtml;
        XNamespace ops = PublicationNamespaces.IdpfOpenPackagingStructure;

        string chapterBody =
            $"""<div class="chapter-body" xmlns="{xhtml}" xmlns:epub="{ops}">{chapterBodyBuilder}</div>""";

        XElement chapterBodyElement = XElement.Parse(chapterBody);
        divPlaceholderElement?.ReplaceWith(chapterBodyElement.Elements());

        return _chapter.ToString();
    }

    private readonly ILogger _logger;
    private readonly DirectoryInfo _chapterDirectoryInfo;
    private readonly XDocument _chapter;
    private readonly KeyValuePair<string, string> _chapterPair;
}
