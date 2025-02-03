namespace Songhay.Publications;

/// <summary>
/// Defines the EPUB Publication Context
/// </summary>
/// <remarks>
/// EPUB is a technical standard published
/// by the International Digital Publishing Forum (IDPF).
/// </remarks>
public class PublicationContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PublicationContext"/> class.
    /// </summary>
    /// <param name="templateRoot">the root directory of the EPUB template files</param>
    /// <param name="logger">the <see cref="ILogger{TCategoryName}"/></param>
    public PublicationContext(string templateRoot, ILogger<PublicationContext>? logger)
    {
        _logger = logger;

        (_csxRootInfo, _publicationRoot) = GetCsxRootInfoAndPublicationRoot(templateRoot);
        (_publicationMeta, _chapterSet) = GetPublicationMetaAndChapterSet();
        _chapterTemplate = GetChapterTemplate(templateRoot);
        _markdownDirectory = GetMarkdownDirectory();
        _epubOebpsDirectory = GetEpubOebpsDirectory();
        _epubTextDirectory = GetEpubTextDirectory();
        _isbn13 = GetIsbn13();
    }

    /// <summary>
    /// Generates EPUB chapters
    /// from <see cref="PublicationFiles.EpubMetadata"/>.
    /// </summary>
    public void GenerateChapters()
    {
        _chapterSet.ToList().ForEach(pair =>
        {
            var chapterDirectory =
                ProgramFileUtility.GetCombinedPath(_markdownDirectory, pair.Value, fileIsExpected: false);
            _logger?.LogInformation("looking for `{Key}`...", pair.Key);

            if (!Directory.Exists(chapterDirectory))
                Throw($"ERROR: cannot find {chapterDirectory}");

            var chapter = new PublicationChapter(pair, _chapterTemplate, chapterDirectory, _logger);
            var xhtml = chapter.GenerateXhtml();
            var path = ProgramFileUtility.GetCombinedPath(_epubTextDirectory, $"{pair.Key}.xhtml",
                fileIsExpected: true);
            _logger?.LogInformation("writing to `{Path}`...", path);
            File.WriteAllText(path, xhtml, EpubUtility.GetUnicodeWithBomEncoding());
        });
    }

    /// <summary>
    /// Calls <see cref="DaisyConsortiumNcx.SetPublicationMeta"/>
    /// and <see cref="IdpfPackage.SetPublicationMeta"/>.
    /// </summary>
    public void GenerateMeta()
    {
        var ncx = new DaisyConsortiumNcx(_publicationMeta, _isbn13, _chapterSet, _epubOebpsDirectory, _logger);
        ncx.SetPublicationMeta();

        var idpf = new IdpfPackage(_publicationMeta, _isbn13, _chapterSet, _epubOebpsDirectory, _logger);
        idpf.SetPublicationMeta();
    }

    /// <summary>
    /// Calls <see cref="OebpsTextBiography.Write"/>.
    /// </summary>
    public void WriteBiography()
    {
        _logger?.LogInformation("writing Biography data...");
        var biography = new OebpsTextBiography(_csxRootInfo.FullName, _epubTextDirectory, _markdownDirectory, _logger);
        biography.Write();
    }

    /// <summary>
    /// Calls <see cref="OebpsTextCopyright.Write"/>.
    /// </summary>
    public void WriteCopyright()
    {
        _logger?.LogInformation("writing copyright data...");
        var copyright = new OebpsTextCopyright(_publicationMeta, _epubTextDirectory);
        copyright.Write();
    }

    /// <summary>
    /// Calls <see cref="OebpsTextDedication.Write"/>.
    /// </summary>
    public void WriteDedication()
    {
        _logger?.LogInformation("writing dedication data...");
        var dedication = new OebpsTextDedication(_csxRootInfo.FullName, _epubTextDirectory, _markdownDirectory, _logger);
        dedication.Write();
    }

    /// <summary>
    /// Writes the <see cref="PublicationFiles.EpubFileTitle"/> file.
    /// </summary>
    public void WriteTitle()
    {
        _logger?.LogInformation("writing Title data...");

        var jPublication = _publicationMeta.GetProperty("publication");
        var title = jPublication.GetProperty("title").GetString();
        var author = jPublication.GetProperty("author").GetString();

        var xhtml = PublicationNamespaces.Xhtml;
        var path = ProgramFileUtility.GetCombinedPath(_epubTextDirectory, PublicationFiles.EpubFileTitle, fileIsExpected: true);
        var titleDocument = XDocument.Load(path);

        var h1Element = (titleDocument.Root?
            .Element(xhtml + "body")?
            .Element(xhtml + "div")?
            .Element(xhtml + "h1"))
            .ToReferenceTypeValueOrThrow();
        var spanElement = (titleDocument.Root?
            .Element(xhtml + "body")?
            .Element(xhtml + "div")?
            .Element(xhtml + "span"))
            .ToReferenceTypeValueOrThrow();

        h1Element.SetValue(title!);
        spanElement.SetValue(author!);

        EpubUtility.SaveAsUnicodeWithBom(titleDocument, path);
    }

    /// <summary>
    /// Calls <see cref="OebpsTextToc.Write"/>.
    /// </summary>
    public void WriteToc()
    {
        _logger?.LogInformation("writing TOC data...");
        var toc = new OebpsTextToc(_publicationMeta, _chapterSet, _epubTextDirectory, _logger);
        toc.Write();
    }

    internal static void Throw(string errorMessage) => throw new Exception(errorMessage);

    internal XDocument GetChapterTemplate(string csxRoot)
    {
        var chapterTemplateFile = ProgramFileUtility.GetCombinedPath(csxRoot, PublicationFiles.EpubTemplateChapter, fileIsExpected: true);

        return XDocument.Load(chapterTemplateFile);
    }

    internal string GetEpubOebpsDirectory()
    {
        var epubRoot = ProgramFileUtility.GetCombinedPath(_publicationRoot, "epub", fileIsExpected: false);

        return ProgramFileUtility.GetCombinedPath(epubRoot, "OEBPS", fileIsExpected: false);
    }

    internal string GetEpubTextDirectory()
    {
        var epubRoot = ProgramFileUtility.GetCombinedPath(_publicationRoot, "epub", fileIsExpected: false);
        var epubOebpsDirectory = ProgramFileUtility.GetCombinedPath(epubRoot, "OEBPS", fileIsExpected: false);

        return ProgramFileUtility.GetCombinedPath(epubOebpsDirectory, "Text", fileIsExpected: false);
    }

    internal string GetIsbn13()
    {
        _logger?.LogInformation("setting isbn 13 into form `isbn-000-0-000-00000-0`...");

        var dictionary = _publicationMeta.GetProperty("publication").GetProperty("identifiers").ToObject<Dictionary<string, string>>();
        var isbn13 = dictionary.TryGetValueWithKey("ISBN-13", throwException: true).ToReferenceTypeValueOrThrow();

        isbn13 = new string(isbn13.Where(char.IsDigit).ToArray());
        _logger?.LogInformation("isbn raw: {Number}", isbn13);

        isbn13 = Convert.ToInt64(isbn13).ToString("isbn-000-0-000-00000-0");
        _logger?.LogInformation("isbn formatted: {Number}", isbn13);

        return  isbn13;
    }

    internal string GetMarkdownDirectory() =>
        ProgramFileUtility.GetCombinedPath(_publicationRoot, "markdown", fileIsExpected: false);

    internal (JsonElement publicationMeta, Dictionary<string, string> chapterSet) GetPublicationMetaAndChapterSet()
    {
        var publicationMetaPath = ProgramFileUtility
            .GetCombinedPath(_publicationRoot, "json", fileIsExpected: false);
        var publicationMetaFile = ProgramFileUtility
            .GetCombinedPath(publicationMetaPath, PublicationFiles.EpubMetadata, fileIsExpected: true);

        var publicationMeta = JsonDocument.Parse(File.ReadAllText(publicationMetaFile)).RootElement;
        var chapterSet = publicationMeta
            .GetProperty("publication")
            .GetProperty("chapterSet")
            .ToObject<Dictionary<string, string>>()
            .ToReferenceTypeValueOrThrow();

        return (publicationMeta, chapterSet);
    }

    internal (DirectoryInfo csxRootInfo, string publicationRoot) GetCsxRootInfoAndPublicationRoot(string csxRoot)
    {
        var csxRootInfo = new DirectoryInfo(csxRoot);
        var publicationRoot = csxRootInfo.Parent.ToReferenceTypeValueOrThrow().FullName;

        return (csxRootInfo, publicationRoot);
    }

    readonly Dictionary<string, string> _chapterSet;
    readonly DirectoryInfo _csxRootInfo;
    readonly JsonElement _publicationMeta;
    readonly string _epubOebpsDirectory;
    readonly string _epubTextDirectory;
    readonly string _isbn13;
    readonly string _markdownDirectory;
    readonly string _publicationRoot;
    readonly XDocument _chapterTemplate;
    readonly ILogger<PublicationContext>? _logger;
}
