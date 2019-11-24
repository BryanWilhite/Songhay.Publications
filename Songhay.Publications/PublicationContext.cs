using Newtonsoft.Json.Linq;
using Songhay.Extensions;
using Songhay.Publications.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Songhay.Publications
{
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
            this.SetPublicationRoot(templateRoot);
            this.SetPublicationMetaAndChapterSet();
            this.SetChapterTemplate(templateRoot);
            this.SetMarkdownDirectory();
            this.SetEpubOebpsDirectory();
            this.SetEpubTextDirectory();
            this.SetIsbn13();
        }

        /// <summary>
        /// Generates EPUB chapters
        /// from <see cref="PublicationFiles.EpubMetadata"/>.
        /// </summary>
        public void GenerateChapters()
        {
            _chapterSet.ToList().ForEach(pair =>
            {
                var chapterDirectory = GetCombinedPath(_markdownDirectory, pair.Value, shouldBeFile: false);
                Console.WriteLine("looking for {0}...", pair.Key);

                if (!Directory.Exists(chapterDirectory))
                    Throw(string.Format("ERROR: cannot find {0}", chapterDirectory));

                var chapter = new PublicationChapter(pair, _chapterTemplate, chapterDirectory);
                var xhtml = chapter.GenerateXhtml();
                var path = GetCombinedPath(_epubTextDirectory, string.Format("{0}.xhtml", pair.Key), shouldBeFile: true);
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
            var path = GetCombinedPath(_epubTextDirectory, PublicationFiles.EpubFileTitle, shouldBeFile: true);
            var titleDocument = XDocument.Load(path);

            var h1Element = titleDocument.Root
                .Element(xhtml + "body")
                .Element(xhtml + "div")
                .Element(xhtml + "h1");
            var spanElement = titleDocument.Root
                .Element(xhtml + "body")
                .Element(xhtml + "div")
                .Element(xhtml + "span");

            h1Element.Value = title;
            spanElement.Value = author;

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

        internal static string GetCombinedPath(string root, string path, bool shouldBeFile)
        { //TODO consider adding to Core
            var combinedPath = FrameworkFileUtility.GetCombinedPath(root, path);

            if(shouldBeFile)
            {
                if (!File.Exists(combinedPath))
                    throw new FileNotFoundException($"The expected file, `{combinedPath ?? "[null]"}`, is not here.");
            }
            else
            {
                if (!Directory.Exists(combinedPath))
                    throw new DirectoryNotFoundException($"The expected directory, `{combinedPath ?? "[null]"}`, is not here.");
            }

            return combinedPath;
        }

        internal static void Throw(string errorMessage)
        {
            throw new Exception(errorMessage);
        }

        internal void SetChapterTemplate(string csxRoot)
        {
            var chapterTemplateFile = PublicationContext.GetCombinedPath(csxRoot, PublicationFiles.EpubTemplateChapter, shouldBeFile: true);
            _chapterTemplate = XDocument.Load(chapterTemplateFile);
        }

        internal void SetEpubOebpsDirectory()
        {
            var epubRoot = GetCombinedPath(_publicationRoot, "epub", shouldBeFile: false);
            _epubOebpsDirectory = GetCombinedPath(epubRoot, "OEBPS", shouldBeFile: false);
        }

        internal void SetEpubTextDirectory()
        {
            var epubRoot = GetCombinedPath(_publicationRoot, "epub", shouldBeFile: false);
            var epubOebpsDirectory = GetCombinedPath(epubRoot, "OEBPS", shouldBeFile: false);
            _epubTextDirectory = GetCombinedPath(epubOebpsDirectory, "Text", shouldBeFile: false);
        }

        internal void SetIsbn13()
        {
            Console.WriteLine("setting isbn 13 into form `isbn-000-0-000-00000-0`...");

            var dictionary = _publicationMeta["publication"]["identifiers"].ToObject<Dictionary<string, string>>();
            var isbn13 = dictionary["ISBN-13"];

            isbn13 = new string(isbn13.Where(i => char.IsDigit(i)).ToArray());
            Console.WriteLine("isbn raw: {0}", isbn13);

            isbn13 = Convert.ToInt64(isbn13).ToString("isbn-000-0-000-00000-0");
            Console.WriteLine("isbn formatted: {0}", isbn13);

            _isbn13 = isbn13;
        }

        internal void SetMarkdownDirectory()
        {
            _markdownDirectory = GetCombinedPath(_publicationRoot, "markdown", shouldBeFile: false);
        }

        internal void SetPublicationMetaAndChapterSet()
        {
            var publicationMetaPath = GetCombinedPath(_publicationRoot, "json", shouldBeFile: false);
            var publicationMetaFile = GetCombinedPath(publicationMetaPath, PublicationFiles.EpubMetadata, shouldBeFile: true);

            _publicationMeta = JObject.Parse(File.ReadAllText(publicationMetaFile));
            _chapterSet = _publicationMeta
                .GetJObject("publication")
                .GetValue<Dictionary<string, string>>("chapterSet");
        }

        internal void SetPublicationRoot(string csxRoot)
        {
            _csxRootInfo = new DirectoryInfo(csxRoot);
            _publicationRoot = _csxRootInfo.Parent.FullName;
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
}
