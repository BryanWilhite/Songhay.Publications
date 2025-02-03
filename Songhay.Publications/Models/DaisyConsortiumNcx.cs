namespace Songhay.Publications.Models;

/// <summary>
/// Defines the contents needed to write
/// the <see cref="PublicationFiles.DaisyConsortiumNcxToc"/> file.
/// </summary>
public class DaisyConsortiumNcx
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DaisyConsortiumNcx"/> class.
    /// </summary>
    /// <param name="publicationMeta">The publication meta.</param>
    /// <param name="isbn13">The isbn13.</param>
    /// <param name="chapterSet">The chapter set.</param>
    /// <param name="epubOebpsDirectory">The epub oebps directory.</param>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public DaisyConsortiumNcx(JsonElement publicationMeta, string? isbn13, Dictionary<string, string>? chapterSet, string? epubOebpsDirectory, ILogger? logger)
    {
        _logger = logger;

        _publicationMeta = publicationMeta;
        _isbn13 = isbn13.ToReferenceTypeValueOrThrow();
        _chapterSet = chapterSet.ToReferenceTypeValueOrThrow();
        _ncxDocumentPath = ProgramFileUtility.GetCombinedPath(epubOebpsDirectory, PublicationFiles.DaisyConsortiumNcxToc, fileIsExpected: true);
        _ncxDocument = XDocument.Load(_ncxDocumentPath);
    }

    /// <summary>
    /// Writes the <see cref="PublicationFiles.DaisyConsortiumNcxToc"/> file.
    /// </summary>
    public void SetPublicationMeta()
    {
        _logger?.LogInformation("setting publication meta...");

        var ncx = PublicationNamespaces.DaisyNcx;

        SetNcxMeta();
        SetNcxDocTitle();

        var navPoints = (_ncxDocument.Root?
            .Element(ncx + "navMap")?
            .Elements(ncx + "navPoint")
            .ToArray()).ToReferenceTypeValueOrThrow();

        SetChapterNavPoints(navPoints);
        UpdateNavPointPlayOrder(navPoints);
        EpubUtility.SaveAsUnicodeWithBom(_ncxDocument, _ncxDocumentPath);
    }

    internal XElement GetNavPoint(string chapterId)
    {
        var ncx = PublicationNamespaces.DaisyNcx;

        return new XElement(ncx + "navPoint",
            new XAttribute("class", "chapter"),
            new XAttribute("id", chapterId),
            new XAttribute("playOrder", string.Empty),
            new XElement(ncx + "navLabel", new XElement(ncx + "text")),
            new XElement(ncx + "content",
                new XAttribute("src", $"Text/{chapterId}.xhtml"))
        );
    }

    internal void SetChapterNavPoints(IEnumerable<XElement> navPoints)
    {
        _logger?.LogInformation("setting navPoint elements for chapters...");

        XElement? templatedChapterElement = null;
        var newChapterElementList = new List<XElement>();

        _chapterSet.Keys
            .Select((chapterId, i) => new { chapterId, i })
            .ForEachInEnumerable(a =>
            {
                var chapterId = a.chapterId;
                var i = a.i;

                var chapterElement = navPoints.SingleOrDefault(navPoint =>
                {
                    var id = navPoint.Attribute("id")?.Value;
                    return chapterId.Equals(id);
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
                    SetChapterNavPointText(
                        templatedChapterElement.ToReferenceTypeValueOrThrow(),
                        _chapterSet[chapterId]);
                }
                else if (canAddNavPoint)
                {
                    var @new = GetNavPoint(chapterId);
                    SetChapterNavPointText(@new, _chapterSet[chapterId]);
                    newChapterElementList.Add(@new);
                }
            });

        if (!newChapterElementList.Any()) return;

        _logger?.LogInformation("adding new elements under templated element...");
        templatedChapterElement?.AddAfterSelf(newChapterElementList.OfType<object>().ToArray());
    }

    internal void SetChapterNavPointText(XElement navPoint, string text)
    {
        _logger?.LogInformation("setting navPoint navLabel text...");

        var ncx = PublicationNamespaces.DaisyNcx;

        var textElement = navPoint
            .Element(ncx + "navLabel")?
            .Element(ncx + "text");
        textElement?.SetValue(text);
    }

    internal void SetNcxDocTitle()
    {
        _logger?.LogInformation("setting ncx docTitle title...");

        var ncx = PublicationNamespaces.DaisyNcx;
        var title = _publicationMeta
            .GetProperty("publication")
            .GetProperty("title")
            .GetString();

        var textElement = _ncxDocument.Root?
            .Element(ncx + "docTitle")?
            .Element(ncx + "text");
        textElement?.SetValue(title!);
    }

    internal void SetNcxMeta()
    {
        _logger?.LogInformation("setting ncx docTitle meta...");

        var ncx = PublicationNamespaces.DaisyNcx;

        var content = _ncxDocument.Root?
            .Element(ncx + "head")?
            .Element(ncx + "meta")?
            .Attribute("content");
        content?.SetValue(_isbn13);
    }

    internal void UpdateNavPointPlayOrder(IEnumerable<XElement> navPoints)
    {
        _logger?.LogInformation("updating navPoint playOrder...");

        navPoints
            .Select((navPoint, i) => new { navPoint, i })
            .ForEachInEnumerable(a =>
            {
                var playOrder = a.navPoint.Attribute("playOrder");
                playOrder?.SetValue((a.i + 1).ToString());
            });
    }

    readonly ILogger? _logger;
    readonly Dictionary<string, string> _chapterSet;
    readonly JsonElement _publicationMeta;
    readonly XDocument _ncxDocument;
    readonly string _isbn13;
    readonly string _ncxDocumentPath;
}
