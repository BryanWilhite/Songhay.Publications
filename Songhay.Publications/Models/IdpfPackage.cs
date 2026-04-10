namespace Songhay.Publications.Models;

/// <summary>
/// Defines the content to write the
/// International Digital Publishing Forum
/// <see cref="PublicationFiles.IdpfcOpfManifest"/> file.
/// </summary>
public class IdpfPackage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IdpfPackage"/> class.
    /// </summary>
    /// <param name="publicationMeta">deserialized <see cref="PublicationFiles.EpubMetadata"/></param>
    /// <param name="isbn13">International Standard Book Number (ISBN)</param>
    /// <param name="chapterSet">chapter data</param>
    /// <param name="epubOebpsDirectory">conventional <c>epub/OEBPS</c> directory</param>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public IdpfPackage(JsonElement publicationMeta, string? isbn13, Dictionary<string, string>? chapterSet, string? epubOebpsDirectory, ILogger logger)
    {
        _logger = logger;

        _publicationMeta = publicationMeta;
        _isbn13 = isbn13.ToReferenceTypeValueOrThrow();
        _chapterSet = chapterSet.ToReferenceTypeValueOrThrow();
        _idpfDocumentPath = ProgramFileUtility
            .GetCombinedPath(epubOebpsDirectory, PublicationFiles.IdpfcOpfManifest, fileIsExpected: true)
            .ToReferenceTypeValueOrThrow();
        _idpfDocument = XDocument.Load(_idpfDocumentPath).ToReferenceTypeValueOrThrow();
    }

    /// <summary>
    /// Writes the
    /// <see cref="PublicationFiles.IdpfcOpfManifest"/> file.
    /// </summary>
    public void SetPublicationMeta()
    {
        SetDublinCoreMeta();
        SetManifestItemElementsForChapters();
        SetSpineItemRefElementsForChapters();

        EpubUtility.SaveAsUnicodeWithBom(_idpfDocument, _idpfDocumentPath);
    }

    internal XElement GetItemRef(string chapterId)
    {
        XNamespace opf = PublicationNamespaces.IdpfOpenPackagingFormat;

        return new XElement(opf + "itemref",
            new XAttribute("idref", chapterId));
    }

    internal XElement GetManifestItem(string chapterId)
    {
        XNamespace opf = PublicationNamespaces.IdpfOpenPackagingFormat;

        return new XElement(opf + "item",
            new XAttribute("href", $"Text/{chapterId}.xhtml"),
            new XAttribute("id", chapterId),
            new XAttribute("media-type", "application/xhtml+xml")
        );
    }

    internal void SetDublinCoreMeta()
    {
        XNamespace dc = PublicationNamespaces.DublinCore;
        XNamespace opf = PublicationNamespaces.IdpfOpenPackagingFormat;

        XElement metadataElement = (_idpfDocument.Root?
            .Element(opf + "metadata"))
            .ToReferenceTypeValueOrThrow();

        XElement? titleElement = metadataElement.Element(dc + "title");
        XElement? identifierElement = metadataElement.Element(dc + "identifier");
        XElement? creatorElement = metadataElement.Element(dc + "creator");
        XElement? publisherElement = metadataElement.Element(dc + "publisher");
        XElement? dateElement = metadataElement.Element(dc + "date");

        JsonElement jPublication = _publicationMeta.GetProperty("publication");

        titleElement?.SetValue(jPublication.GetProperty("title").GetString()!);
        identifierElement?.SetValue(_isbn13);
        creatorElement?.SetValue(jPublication.GetProperty("author").GetString()!);
        publisherElement?.SetValue(jPublication.GetProperty("publisher").GetString()!);
        dateElement?.SetValue(jPublication.GetProperty("publicationDate").GetString()!);
    }

    internal void SetManifestItem(XElement item, string id)
    {
        string href = $"Text/{id}.xhtml";
        XAttribute? hrefAttribute = item.Attribute("href");
        XAttribute? idAttribute = item.Attribute("id");

        hrefAttribute?.SetValue(href);
        idAttribute?.SetValue(id);
    }

    internal void SetManifestItemElementsForChapters()
    {
        _logger.LogInformation("setting manifest item elements for chapters...");

        XNamespace opf = PublicationNamespaces.IdpfOpenPackagingFormat;

        IEnumerable<XElement> items = (_idpfDocument.Root?
            .Element(opf + "manifest")?
            .Elements(opf + "item"))
            .ToReferenceTypeValueOrThrow();

        XElement? templatedChapterElement = null;
        List<XElement> newChapterElementList = new List<XElement>();

        _chapterSet.Keys
            .Select((chapterId, i) => new { chapterId, i })
            .ForEachInEnumerable(a =>
            {
                string chapterId = a.chapterId;
                int i = a.i;

                XElement? chapterElement = items.SingleOrDefault(item =>
                {
                    string? id = item.Attribute("id")?.Value;

                    return chapterId.Equals(id);
                });

                bool canAddNavPoint = (chapterElement == null) && (i > 0);
                bool isFirstChapterIdError = (chapterElement == null) && (i == 0);
                bool isFirstChapterId = (chapterElement != null) && (i == 0);

                if (isFirstChapterIdError)
                {
                    PublicationContext.Throw($"ERROR: cannot find templated element {chapterId}");
                }
                else if (isFirstChapterId)
                {
                    templatedChapterElement = chapterElement;
                    SetManifestItem(templatedChapterElement.ToReferenceTypeValueOrThrow(), chapterId);
                }
                else if (canAddNavPoint)
                {
                    XElement @new = GetManifestItem(chapterId);
                    SetManifestItem(@new, chapterId);
                    newChapterElementList.Add(@new);
                }
            });

        if (newChapterElementList.Count == 0) return;

        _logger.LogInformation("adding new elements under templated element...");
        templatedChapterElement?.AddAfterSelf(newChapterElementList.OfType<object>().ToArray());
    }

    internal void SetSpineItemRef(XElement itemRef, string idref)
    {
        XAttribute? idrefAttribute = itemRef.Attribute("idref");

        if(idrefAttribute == null) _logger.LogErrorForMissingData<XAttribute>();

        idrefAttribute?.SetValue(idref);
    }

    internal void SetSpineItemRefElementsForChapters()
    {
        _logger.LogInformation("setting spine itemref elements for chapters...");

        XNamespace opf = PublicationNamespaces.IdpfOpenPackagingFormat;

        IEnumerable<XElement> itemRefs = (_idpfDocument.Root?
            .Element(opf + "spine")?
            .Elements(opf + "itemref"))
            .ToReferenceTypeValueOrThrow();

        XElement? templatedChapterElement = null;
        List<XElement> newChapterElementList = new List<XElement>();

        _chapterSet.Keys
            .Select((chapterId, i) => new { chapterId, i })
            .ForEachInEnumerable(a =>
            {
                string chapterId = a.chapterId;
                int i = a.i;
                XElement? chapterElement = itemRefs.SingleOrDefault(itemRef =>
                {
                    string? id = itemRef.Attribute("idref")?.Value;

                    return chapterId.Equals(id);
                });

                bool canAddNavPoint = chapterElement == null && i > 0;
                bool isFirstChapterIdError = chapterElement == null && i == 0;
                bool isFirstChapterId = chapterElement != null && i == 0;

                if (isFirstChapterIdError)
                {
                    PublicationContext.Throw($"ERROR: cannot find templated element {chapterId}");
                }
                else if (isFirstChapterId)
                {
                    templatedChapterElement = chapterElement;
                    SetSpineItemRef(templatedChapterElement.ToReferenceTypeValueOrThrow(), chapterId);
                }
                else if (canAddNavPoint)
                {
                    XElement @new = GetItemRef(chapterId);
                    SetSpineItemRef(@new, chapterId);
                    newChapterElementList.Add(@new);
                }
            });

        if (newChapterElementList.Count == 0) return;

        _logger.LogInformation("adding new elements under templated element...");
        templatedChapterElement?.AddAfterSelf(newChapterElementList.OfType<object>().ToArray());
    }

    private readonly ILogger _logger;
    private readonly Dictionary<string, string> _chapterSet;
    private readonly JsonElement _publicationMeta;
    private readonly string _isbn13;
    private readonly string _idpfDocumentPath;
    private readonly XDocument _idpfDocument;
}
