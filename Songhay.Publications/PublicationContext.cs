using Newtonsoft.Json.Linq;

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
    public PublicationContext(string templateRoot)
    {
        SetPublicationRoot(templateRoot);
        SetPublicationMetaAndChapterSet();
        SetChapterTemplate(templateRoot);
        SetMarkdownDirectory();
        SetEpubOebpsDirectory();
        SetEpubTextDirectory();
        SetIsbn13();
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
            Console.WriteLine("looking for {0}...", pair.Key);

            if (!Directory.Exists(chapterDirectory))
                Throw($"ERROR: cannot find {chapterDirectory}");

            var chapter = new PublicationChapter(pair, _chapterTemplate, chapterDirectory);
            var xhtml = chapter.GenerateXhtml();
            var path = ProgramFileUtility.GetCombinedPath(_epubTextDirectory, $"{pair.Key}.xhtml",
                fileIsExpected: true);
            Console.WriteLine("writing to {0}...", path);
            File.WriteAllText(path, xhtml, EpubUtility.GetUnicodeWithBomEncoding());
        });
    }

    /// <summary>
    /// Calls <see cref="DaisyConsortiumNcx.SetPublicationMeta"/>
    /// and <see cref="IdpfPackage.SetPublicationMeta"/>.
    /// </summary>
    public void GenerateMeta()
    {
        var ncx = new DaisyConsortiumNcx(_publicationMeta, _isbn13, _chapterSet, _epubOebpsDirectory);
        ncx.SetPublicationMeta();

        var idpf = new IdpfPackage(_publicationMeta, _isbn13, _chapterSet, _epubOebpsDirectory);
        idpf.SetPublicationMeta();
    }

    /// <summary>
    /// Calls <see cref="OebpsTextBiography.Write"/>.
    /// </summary>
    public void WriteBiography()
    {
        Console.WriteLine("writing Biography data...");
        var biography = new OebpsTextBiography(_csxRootInfo.FullName, _epubTextDirectory, _markdownDirectory);
        biography.Write();
    }

    /// <summary>
    /// Calls <see cref="OebpsTextCopyright.Write"/>.
    /// </summary>
    public void WriteCopyright()
    {
        Console.WriteLine("writing copyright data...");
        var copyright = new OebpsTextCopyright(_publicationMeta, _epubTextDirectory);
        copyright.Write();
    }

    /// <summary>
    /// Calls <see cref="OebpsTextDedication.Write"/>.
    /// </summary>
    public void WriteDedication()
    {
        Console.WriteLine("writing dedication data...");
        var dedication = new OebpsTextDedication(_csxRootInfo.FullName, _epubTextDirectory, _markdownDirectory);
        dedication.Write();
    }

    /// <summary>
    /// Writes the <see cref="PublicationFiles.EpubFileTitle"/> file.
    /// </summary>
    public void WriteTitle()
    {
        Console.WriteLine("writing Title data...");

        var jPublication = _publicationMeta.GetJObject("publication");
        var title = jPublication.GetValue<string>("title");
        var author = jPublication.GetValue<string>("author");

        var xhtml = PublicationNamespaces.Xhtml;
        var path = ProgramFileUtility.GetCombinedPath(_epubTextDirectory, PublicationFiles.EpubFileTitle, fileIsExpected: true);
        var titleDocument = XDocument.Load(path);

        var h1Element = titleDocument.Root?
            .Element(xhtml + "body")?
            .Element(xhtml + "div")?
            .Element(xhtml + "h1")
            .ToReferenceTypeValueOrThrow();
        var spanElement = titleDocument.Root?
            .Element(xhtml + "body")?
            .Element(xhtml + "div")?
            .Element(xhtml + "span")
            .ToReferenceTypeValueOrThrow();

        h1Element?.SetValue(title);
        spanElement?.SetValue(author);

        EpubUtility.SaveAsUnicodeWithBom(titleDocument, path);
    }

    /// <summary>
    /// Calls <see cref="OebpsTextToc.Write"/>.
    /// </summary>
    public void WriteToc()
    {
        Console.WriteLine("writing TOC data...");
        var toc = new OebpsTextToc(_publicationMeta, _chapterSet, _epubTextDirectory);
        toc.Write();
    }

    internal static void Throw(string errorMessage)
    {
        throw new Exception(errorMessage);
    }

    internal void SetChapterTemplate(string csxRoot)
    {
        var chapterTemplateFile = ProgramFileUtility.GetCombinedPath(csxRoot, PublicationFiles.EpubTemplateChapter, fileIsExpected: true);
        _chapterTemplate = XDocument.Load(chapterTemplateFile);
    }

    internal void SetEpubOebpsDirectory()
    {
        var epubRoot = ProgramFileUtility.GetCombinedPath(_publicationRoot, "epub", fileIsExpected: false);
        _epubOebpsDirectory = ProgramFileUtility.GetCombinedPath(epubRoot, "OEBPS", fileIsExpected: false);
    }

    internal void SetEpubTextDirectory()
    {
        var epubRoot = ProgramFileUtility.GetCombinedPath(_publicationRoot, "epub", fileIsExpected: false);
        var epubOebpsDirectory = ProgramFileUtility.GetCombinedPath(epubRoot, "OEBPS", fileIsExpected: false);
        _epubTextDirectory = ProgramFileUtility.GetCombinedPath(epubOebpsDirectory, "Text", fileIsExpected: false);
    }

    internal void SetIsbn13()
    {
        Console.WriteLine("setting isbn 13 into form `isbn-000-0-000-00000-0`...");

        var dictionary = _publicationMeta?["publication"]?["identifiers"]?.ToObject<Dictionary<string, string>>();
        var isbn13 = dictionary?
            .TryGetValueWithKey("ISBN-13", throwException: true)
            .ToReferenceTypeValueOrThrow();

        isbn13 = new string(isbn13!.Where(char.IsDigit).ToArray());
        Console.WriteLine("isbn raw: {0}", isbn13);

        isbn13 = Convert.ToInt64(isbn13).ToString("isbn-000-0-000-00000-0");
        Console.WriteLine("isbn formatted: {0}", isbn13);

        _isbn13 = isbn13;
    }

    internal void SetMarkdownDirectory()
    {
        _markdownDirectory = ProgramFileUtility.GetCombinedPath(_publicationRoot, "markdown", fileIsExpected: false);
    }

    internal void SetPublicationMetaAndChapterSet()
    {
        var publicationMetaPath = ProgramFileUtility
            .GetCombinedPath(_publicationRoot, "json", fileIsExpected: false);
        var publicationMetaFile = ProgramFileUtility
            .GetCombinedPath(publicationMetaPath, PublicationFiles.EpubMetadata, fileIsExpected: true);

        _publicationMeta = JObject.Parse(File.ReadAllText(publicationMetaFile));
        _chapterSet = _publicationMeta
            .GetJObject("publication")
            .GetValue<Dictionary<string, string>>("chapterSet");
    }

    internal void SetPublicationRoot(string csxRoot)
    {
        _csxRootInfo = new DirectoryInfo(csxRoot);
        _publicationRoot = _csxRootInfo.Parent.ToReferenceTypeValueOrThrow()!.FullName;
    }

    Dictionary<string, string> _chapterSet;
    DirectoryInfo _csxRootInfo;
    JObject _publicationMeta;
    string _epubOebpsDirectory;
    string _epubTextDirectory;
    string _isbn13;
    string _markdownDirectory;
    string _publicationRoot;
    XDocument _chapterTemplate;
}
