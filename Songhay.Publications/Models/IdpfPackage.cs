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
    public IdpfPackage(JsonElement publicationMeta, string? isbn13, Dictionary<string, string>? chapterSet, string? epubOebpsDirectory, ILogger? logger)
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
        var opf = PublicationNamespaces.IdpfOpenPackagingFormat;

        return new XElement(opf + "itemref",
            new XAttribute("idref", chapterId));
    }

    internal XElement GetManifestItem(string chapterId)
    {
        var opf = PublicationNamespaces.IdpfOpenPackagingFormat;

        return new XElement(opf + "item",
            new XAttribute("href", $"Text/{chapterId}.xhtml"),
            new XAttribute("id", chapterId),
            new XAttribute("media-type", "application/xhtml+xml")
        );
    }

    internal void SetDublinCoreMeta()
    {
        var dc = PublicationNamespaces.DublinCore;
        var opf = PublicationNamespaces.IdpfOpenPackagingFormat;

        var metadataElement = (_idpfDocument.Root?
            .Element(opf + "metadata"))
            .ToReferenceTypeValueOrThrow();

        var titleElement = metadataElement.Element(dc + "title");
        var identifierElement = metadataElement.Element(dc + "identifier");
        var creatorElement = metadataElement.Element(dc + "creator");
        var publisherElement = metadataElement.Element(dc + "publisher");
        var dateElement = metadataElement.Element(dc + "date");

        var jPublication = _publicationMeta.GetProperty("publication");

        titleElement?.SetValue(jPublication.GetProperty("title").GetString()!);
        identifierElement?.SetValue(_isbn13);
        creatorElement?.SetValue(jPublication.GetProperty("author").GetString()!);
        publisherElement?.SetValue(jPublication.GetProperty("publisher").GetString()!);
        dateElement?.SetValue(jPublication.GetProperty("publicationDate").GetString()!);
    }

    internal void SetManifestItem(XElement item, string id)
    {
        var href = $"Text/{id}.xhtml";
        var hrefAttribute = item.Attribute("href");
        var idAttribute = item.Attribute("id");

        hrefAttribute?.SetValue(href);
        idAttribute?.SetValue(id);
    }

    internal void SetManifestItemElementsForChapters()
    {
        _logger?.LogInformation("setting manifest item elements for chapters...");

        var opf = PublicationNamespaces.IdpfOpenPackagingFormat;

        var items = (_idpfDocument.Root?
            .Element(opf + "manifest")?
            .Elements(opf + "item"))
            .ToReferenceTypeValueOrThrow();

        XElement? templatedChapterElement = null;
        var newChapterElementList = new List<XElement>();

        _chapterSet.Keys
            .Select((chapterId, i) => new { chapterId, i })
            .ForEachInEnumerable(a =>
            {
                var chapterId = a.chapterId;
                var i = a.i;

                var chapterElement = items.SingleOrDefault(item =>
                {
                    var id = item.Attribute("id")?.Value;

                    return chapterId.Equals(id);
                });

                var canAddNavPoint = (chapterElement == null) && (i > 0);
                var isFirstChapterIdError = (chapterElement == null) && (i == 0);
                var isFirstChapterId = (chapterElement != null) && (i == 0);

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
                    var @new = GetManifestItem(chapterId);
                    SetManifestItem(@new, chapterId);
                    newChapterElementList.Add(@new);
                }
            });

        if (!newChapterElementList.Any()) return;

        _logger?.LogInformation("adding new elements under templated element...");
        templatedChapterElement?.AddAfterSelf(newChapterElementList.OfType<object>().ToArray());
    }

    internal void SetSpineItemref(XElement itemref, string idref)
    {
        var idrefAttribute = itemref.Attribute("idref");
        idrefAttribute?.SetValue(idref);
    }

    internal void SetSpineItemRefElementsForChapters()
    {
        _logger?.LogInformation("setting spine itemref elements for chapters...");

        var opf = PublicationNamespaces.IdpfOpenPackagingFormat;

        var itemrefs = (_idpfDocument.Root?
            .Element(opf + "spine")?
            .Elements(opf + "itemref"))
            .ToReferenceTypeValueOrThrow();

        XElement? templatedChapterElement = null;
        var newChapterElementList = new List<XElement>();

        _chapterSet.Keys
            .Select((chapterId, i) => new { chapterId, i })
            .ForEachInEnumerable(a =>
            {
                var chapterId = a.chapterId;
                var i = a.i;
                var chapterElement = itemrefs.SingleOrDefault(itemref =>
                {
                    var id = itemref.Attribute("idref")?.Value;

                    return chapterId.Equals(id);
                });

                var canAddNavPoint = (chapterElement == null) && (i > 0);
                var isFirstChapterIdError = (chapterElement == null) && (i == 0);
                var isFirstChapterId = (chapterElement != null) && (i == 0);

                if (isFirstChapterIdError)
                {
                    PublicationContext.Throw($"ERROR: cannot find templated element {chapterId}");
                }
                else if (isFirstChapterId)
                {
                    templatedChapterElement = chapterElement;
                    SetSpineItemref(templatedChapterElement.ToReferenceTypeValueOrThrow(), chapterId);
                }
                else if (canAddNavPoint)
                {
                    var @new = GetItemRef(chapterId);
                    SetSpineItemref(@new, chapterId);
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
    readonly string _isbn13;
    readonly string _idpfDocumentPath;
    readonly XDocument _idpfDocument;
}
