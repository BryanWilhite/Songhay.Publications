using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Songhay.Publications.Models;

/// <summary>
/// Defines the content to write the
/// <see cref="PublicationFiles.EpubFileToc"/> file.
/// </summary>
/// <remarks>
///  Open eBook Publication Structure (OEBPS),
///  is a legacy e-book format which
///  has been superseded by the EPUB format.
///
/// https://en.wikipedia.org/wiki/Open_eBook
/// </remarks>
public class OebpsTextToc
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OebpsTextToc"/> class.
    /// </summary>
    /// <param name="publicationMeta">deserialized <see cref="PublicationFiles.EpubMetadata"/></param>
    /// <param name="chapterSet">chapter data</param>
    /// <param name="epubTextDirectory">conventional <c>epub/OEBPS/Text</c> directory</param>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public OebpsTextToc(JsonElement publicationMeta, Dictionary<string, string> chapterSet, string epubTextDirectory, ILogger? logger)
    {
        _logger = logger;

        _publicationMeta = publicationMeta;
        _chapterSet = chapterSet;
        _documentPath = ProgramFileUtility.GetCombinedPath(epubTextDirectory, PublicationFiles.EpubFileToc, fileIsExpected: true);
        _document = XDocument.Load(_documentPath);
    }

    /// <summary>
    /// Writes the
    /// <see cref="PublicationFiles.EpubFileToc"/> file.
    /// </summary>
    public void Write()
    {
        var jPublication = _publicationMeta.GetProperty("publication");
        var title = jPublication.GetProperty("title").GetString();
        var author = jPublication.GetProperty("author").GetString();

        var xhtml = PublicationNamespaces.Xhtml;

        var h2Element = _document.Root?
            .Element(xhtml + "body")?
            .Element(xhtml + "div")?
            .Element(xhtml + "h2")
            .ToReferenceTypeValueOrThrow();
        var spanElement = _document.Root?
            .Element(xhtml + "body")?
            .Element(xhtml + "div")?
            .Element(xhtml + "h3")?
            .Element(xhtml + "span")
            .ToReferenceTypeValueOrThrow();

        h2Element?.SetValue(title!);
        spanElement?.SetValue(author!);

        SetTocAnchors(_document);

        EpubUtility.SaveAsUnicodeWithBom(_document, _documentPath);
    }

    internal XElement GetTocAnchor(string chapterId)
    {
        var xhtml = PublicationNamespaces.Xhtml;
        var hrefTemplate = GetTocHrefTemplate();

        return new XElement(xhtml + "a",
            new XAttribute("href", string.Format(hrefTemplate, chapterId)),
            GetTocChapterValue(chapterId)
        );
    }

    internal string GetTocChapterValue(string chapterId) => _chapterSet[chapterId];

    internal string GetTocHrefTemplate() => "../Text/{0}.xhtml";

    internal void SetTocAnchor(XElement? a, string chapterId)
    {
        var hrefTemplate = GetTocHrefTemplate();

        a?.SetValue(GetTocChapterValue(chapterId));

        var hrefAttribute = a?.Attribute("href");

        hrefAttribute?.SetValue(string.Format(hrefTemplate, chapterId));
    }

    internal void SetTocAnchors(XDocument tocDocument)
    {
        _logger?.LogInformation("setting TOC chapter anchors...");

        var xhtml = PublicationNamespaces.Xhtml;

        var anchors = tocDocument.Root?
            .Element(xhtml + "body")?
            .Element(xhtml + "div")?
            .Elements(xhtml + "a")
            .ToReferenceTypeValueOrThrow();

        XElement? templatedChapterElement = null;
        var newChapterElementList = new List<XElement>();
        var hrefTemplate = GetTocHrefTemplate();

        _chapterSet.Keys
            .Select((chapterId, i) => new { chapterId, i })
            .ForEachInEnumerable(a =>
            {
                var chapterId = a.chapterId;
                var i = a.i;

                var chapterElement = anchors?.SingleOrDefault(item =>
                {
                    var href = item.Attribute("href")?.Value;

                    return href == string.Format(hrefTemplate, chapterId);
                });

                var canAddNavPoint = chapterElement == null && i > 0;
                var isFirstChapterIdError = chapterElement == null && i == 0;
                var isFirstChapterId = chapterElement != null && i == 0;

                if (isFirstChapterIdError)
                {
                    PublicationContext.Throw($"ERROR: cannot find templated element {chapterId}");
                }
                else if (isFirstChapterId)
                {
                    templatedChapterElement = chapterElement;
                    SetTocAnchor(templatedChapterElement, chapterId);
                }
                else if (canAddNavPoint)
                {
                    var @new = GetTocAnchor(chapterId);
                    SetTocAnchor(@new, chapterId);
                    newChapterElementList.Add(new XElement(xhtml + "br"));
                    newChapterElementList.Add(@new);
                }
            });

        if (!newChapterElementList.Any()) return;

        _logger?.LogInformation("adding new elements under templated element...");
        templatedChapterElement?.AddAfterSelf(newChapterElementList.OfType<object>().ToArray());
    }

    readonly ILogger? _logger;
    readonly Dictionary<string, string> _chapterSet;
    readonly JsonElement _publicationMeta;
    readonly string _documentPath;
    readonly XDocument _document;
}
