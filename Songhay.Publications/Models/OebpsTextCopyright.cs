namespace Songhay.Publications.Models;

/// <summary>
/// Defines the content to write the
/// <see cref="PublicationFiles.EpubFileCopyright"/> file.
/// </summary>
/// <remarks>
///  Open eBook Publication Structure (OEBPS),
///  is a legacy e-book format which
///  has been superseded by the EPUB format.
///
/// https://en.wikipedia.org/wiki/Open_eBook
/// </remarks>
public class OebpsTextCopyright
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OebpsTextCopyright" /> class.
    /// </summary>
    /// <param name="publicationMeta">The publication meta.</param>
    /// <param name="epubTextDirectory">Conventional <c>epub/OEBPS/Text</c> directory.</param>
    public OebpsTextCopyright(JsonElement publicationMeta, string epubTextDirectory)
    {
        _publicationMeta = publicationMeta;
        _documentPath = ProgramFileUtility.GetCombinedPath(epubTextDirectory, PublicationFiles.EpubFileCopyright, fileIsExpected: true);
        _document = XDocument.Load(_documentPath);
        _spans = GetSpans();
    }

    /// <summary>
    /// Writes the
    /// <see cref="PublicationFiles.EpubFileCopyright"/> file.
    /// </summary>
    public void Write()
    {
        string? pubYear = _publicationMeta
            .GetProperty("publication")
            .GetProperty("publicationDate").GetString();
        pubYear = DateTime.Parse(pubYear!).Year.ToString();

        JsonElement jPub = _publicationMeta.GetProperty("publication");

        string? pubAuthor = jPub.GetProperty("author").GetString();
        string? pubInquiries = jPub.GetProperty("inquiries").GetString();
        string? pubCoverArtCredits = jPub.GetProperty("coverArtCredits").GetString();
        string? pubEpubPublicationDate = jPub.GetProperty("epubPublicationDate").GetString();

        XElement span = GetSpan("pub-year");
        span.Value = pubYear + " ";

        IEnumerable<XElement> spans = GetSpans("pub-author");
        spans.ForEachInEnumerable(i => i.Value = pubAuthor!);

        span = GetSpan("pub-inquiries");
        span.Value = pubInquiries!;

        span = GetSpan("pub-cover-art-credits");
        span.Value = pubCoverArtCredits!;

        span = GetSpan("pub-epub-date");
        span.Value = pubEpubPublicationDate!;

        EpubUtility.SaveAsUnicodeWithBom(_document, _documentPath);
    }

    internal XElement GetSpan(string @class)
    {
        return _spans
            .Single(i =>
            {
                var a = i.Attribute("class");
                return (a != null) && (a.Value == @class);
            });
    }

    internal IEnumerable<XElement> GetSpans(string @class)
    {
        return _spans.Where(i =>
        {
            var a = i.Attribute("class");
            return (a != null) && (a.Value == @class);
        });
    }

    internal IEnumerable<XElement> GetSpans()
    {
        XNamespace xhtml = PublicationNamespaces.Xhtml;

        XElement? bodyDivElement = _document.Root?
            .Element(xhtml + "body")?
            .Element(xhtml + "div")
            .ToReferenceTypeValueOrThrow();

        return bodyDivElement?
            .Descendants(xhtml + "span") ?? [];
    }

    private readonly JsonElement _publicationMeta;
    private readonly string _documentPath;
    private readonly XDocument _document;
    private readonly IEnumerable<XElement> _spans;
}
