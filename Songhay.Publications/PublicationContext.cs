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
    public PublicationContext(string templateRoot, ILogger<PublicationContext> logger)
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
            string chapterDirectory =
                ProgramFileUtility.GetCombinedPath(_markdownDirectory, pair.Value, fileIsExpected: false);
            _logger.LogInformation("looking for `{Key}`...", pair.Key);

            if (!Directory.Exists(chapterDirectory))
                Throw($"ERROR: cannot find {chapterDirectory}");

            PublicationChapter chapter = new PublicationChapter(pair, _chapterTemplate, chapterDirectory, _logger);
            string xhtml = chapter.GenerateXhtml();
            string path = ProgramFileUtility.GetCombinedPath(_epubTextDirectory, $"{pair.Key}.xhtml",
                fileIsExpected: true);
            _logger.LogInformation("writing to `{Path}`...", path);
            File.WriteAllText(path, xhtml, EpubUtility.GetUnicodeWithBomEncoding());
        });
    }

    /// <summary>
    /// Calls <see cref="DaisyConsortiumNcx.SetPublicationMeta"/>
    /// and <see cref="IdpfPackage.SetPublicationMeta"/>.
    /// </summary>
    public void GenerateMeta()
    {
        DaisyConsortiumNcx ncx = new DaisyConsortiumNcx(_publicationMeta, _isbn13, _chapterSet, _epubOebpsDirectory, _logger);
        ncx.SetPublicationMeta();

        IdpfPackage idpf = new IdpfPackage(_publicationMeta, _isbn13, _chapterSet, _epubOebpsDirectory, _logger);
        idpf.SetPublicationMeta();
    }

    /// <summary>
    /// Calls <see cref="OebpsTextBiography.Write"/>.
    /// </summary>
    public void WriteBiography()
    {
        _logger.LogInformation("writing Biography data...");
        OebpsTextBiography biography = new OebpsTextBiography(_csxRootInfo.FullName, _epubTextDirectory, _markdownDirectory, _logger);
        biography.Write();
    }

    /// <summary>
    /// Calls <see cref="OebpsTextCopyright.Write"/>.
    /// </summary>
    public void WriteCopyright()
    {
        _logger.LogInformation("writing copyright data...");
        OebpsTextCopyright copyright = new OebpsTextCopyright(_publicationMeta, _epubTextDirectory);
        copyright.Write();
    }

    /// <summary>
    /// Calls <see cref="OebpsTextDedication.Write"/>.
    /// </summary>
    public void WriteDedication()
    {
        _logger.LogInformation("writing dedication data...");
        OebpsTextDedication dedication = new OebpsTextDedication(_csxRootInfo.FullName, _epubTextDirectory, _markdownDirectory, _logger);
        dedication.Write();
    }

    /// <summary>
    /// Writes the <see cref="PublicationFiles.EpubFileTitle"/> file.
    /// </summary>
    public void WriteTitle()
    {
        _logger.LogInformation("writing Title data...");

        JsonElement jPublication = _publicationMeta.GetProperty("publication");
        string? title = jPublication.GetProperty("title").GetString();
        string? author = jPublication.GetProperty("author").GetString();

        XNamespace xhtml = PublicationNamespaces.Xhtml;
        string path = ProgramFileUtility.GetCombinedPath(_epubTextDirectory, PublicationFiles.EpubFileTitle, fileIsExpected: true);
        XDocument titleDocument = XDocument.Load(path);

        XElement h1Element = (titleDocument.Root?
            .Element(xhtml + "body")?
            .Element(xhtml + "div")?
            .Element(xhtml + "h1"))
            .ToReferenceTypeValueOrThrow();
        XElement spanElement = (titleDocument.Root?
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
        _logger.LogInformation("writing TOC data...");
        OebpsTextToc toc = new OebpsTextToc(_publicationMeta, _chapterSet, _epubTextDirectory, _logger);
        toc.Write();
    }

    internal static void Throw(string errorMessage) => throw new Exception(errorMessage);

    internal XDocument GetChapterTemplate(string csxRoot)
    {
        string chapterTemplateFile = ProgramFileUtility.GetCombinedPath(csxRoot, PublicationFiles.EpubTemplateChapter, fileIsExpected: true);

        return XDocument.Load(chapterTemplateFile);
    }

    internal string GetEpubOebpsDirectory()
    {
        string epubRoot = ProgramFileUtility.GetCombinedPath(_publicationRoot, "epub", fileIsExpected: false);

        return ProgramFileUtility.GetCombinedPath(epubRoot, "OEBPS", fileIsExpected: false);
    }

    internal string GetEpubTextDirectory()
    {
        string epubRoot = ProgramFileUtility.GetCombinedPath(_publicationRoot, "epub", fileIsExpected: false);
        string epubOebpsDirectory = ProgramFileUtility.GetCombinedPath(epubRoot, "OEBPS", fileIsExpected: false);

        return ProgramFileUtility.GetCombinedPath(epubOebpsDirectory, "Text", fileIsExpected: false);
    }

    internal string GetIsbn13()
    {
        _logger.LogInformation("setting isbn 13 into form `isbn-000-0-000-00000-0`...");

        Dictionary<string, string>? dictionary = _publicationMeta.GetProperty("publication").GetProperty("identifiers").ToObject<Dictionary<string, string>>();
        string isbn13 = dictionary.TryGetValueWithKey("ISBN-13", throwException: true).ToReferenceTypeValueOrThrow();

        isbn13 = new string(isbn13.Where(char.IsDigit).ToArray());
        _logger.LogInformation("isbn raw: {Number}", isbn13);

        isbn13 = Convert.ToInt64(isbn13).ToString("isbn-000-0-000-00000-0");
        _logger.LogInformation("isbn formatted: {Number}", isbn13);

        return  isbn13;
    }

    internal string GetMarkdownDirectory() =>
        ProgramFileUtility.GetCombinedPath(_publicationRoot, "markdown", fileIsExpected: false);

    internal (JsonElement publicationMeta, Dictionary<string, string> chapterSet) GetPublicationMetaAndChapterSet()
    {
        string publicationMetaPath = ProgramFileUtility
            .GetCombinedPath(_publicationRoot, "json", fileIsExpected: false);
        string publicationMetaFile = ProgramFileUtility
            .GetCombinedPath(publicationMetaPath, PublicationFiles.EpubMetadata, fileIsExpected: true);

        using var jDoc = JsonDocument.Parse(File.ReadAllText(publicationMetaFile));
        JsonElement publicationMeta = jDoc.RootElement;
        Dictionary<string, string> chapterSet = publicationMeta
            .GetProperty("publication")
            .GetProperty("chapterSet")
            .ToObject<Dictionary<string, string>>()
            .ToReferenceTypeValueOrThrow();

        return (publicationMeta, chapterSet);
    }

    internal static (DirectoryInfo csxRootInfo, string publicationRoot) GetCsxRootInfoAndPublicationRoot(string csxRoot)
    {
        DirectoryInfo csxRootInfo = new DirectoryInfo(csxRoot);
        string publicationRoot = csxRootInfo.Parent.ToReferenceTypeValueOrThrow().FullName;

        return (csxRootInfo, publicationRoot);
    }

    private readonly Dictionary<string, string> _chapterSet;
    private readonly DirectoryInfo _csxRootInfo;
    private readonly JsonElement _publicationMeta;
    private readonly string _epubOebpsDirectory;
    private readonly string _epubTextDirectory;
    private readonly string _isbn13;
    private readonly string _markdownDirectory;
    private readonly string _publicationRoot;
    private readonly XDocument _chapterTemplate;
    private readonly ILogger<PublicationContext> _logger;
}
