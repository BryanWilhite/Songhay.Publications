using Newtonsoft.Json.Linq;
using Songhay.Publications.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Songhay.Publications
{
    public class PublicationContext
    {
        public PublicationContext(string csxRoot)
        {
            this.SetPublicationRoot(csxRoot);
            this.SetPublicationMetaAndChapterSet();
            this.SetChapterTemplate(csxRoot);
            this.SetMarkdownFolder();
            this.SetEpubOebpsFolder();
            this.SetEpubTextFolder();
            this.SetIsbn13();
        }

        public void GenerateChapters()
        {
            _chapterSet.ToList().ForEach(pair =>
            {
                var chapterFolder = Path.Combine(_markdownFolder, pair.Value);
                Console.WriteLine("looking for {0}...", pair.Key);

                if (!Directory.Exists(chapterFolder))
                    Throw(string.Format("ERROR: cannot find {0}", chapterFolder));

                var chapter = new PublicationChapter(pair, _chapterTemplate, chapterFolder);
                var xhtml = chapter.GenerateXhtml();
                var path = Path.Combine(_epubTextFolder, string.Format("{0}.xhtml", pair.Key));
                Console.WriteLine("writing to {0}...", path);
                File.WriteAllText(path, xhtml, EpubUtility.GetUnicodeWithBomEncoding());
            });
        }

        public void GenerateMeta()
        {
            var ncx = new DaisyConsortiumNcx(_publicationMeta, _isbn13, _chapterSet, _epubOebpsFolder);
            ncx.SetPublicationMeta();

            var idpf = new IdpfPackage(_publicationMeta, _isbn13, _chapterSet, _epubOebpsFolder);
            idpf.SetPublicationMeta();
        }

        public void WriteBiography()
        {
            Console.WriteLine("writing Biography data...");
            var biography = new OebpsTextBiography(_csxRootInfo.FullName, _epubTextFolder, _markdownFolder);
            biography.Write();
        }

        public void WriteCopyright()
        {
            Console.WriteLine("writing copyright data...");
            var copyright = new OebpsTextCopyright(_publicationMeta, _epubTextFolder);
            copyright.Write();
        }

        public void WriteDedication()
        {
            Console.WriteLine("writing dedication data...");
            var dedication = new OebpsTextDedication(_csxRootInfo.FullName, _epubTextFolder, _markdownFolder);
            dedication.Write();
        }

        public void WriteTitle()
        {
            Console.WriteLine("writing Title data...");

            var title = _publicationMeta["publication"]["title"].Value<string>();
            var author = _publicationMeta["publication"]["author"].Value<string>();

            var xhtml = PublicationNamespaces.Xhtml;
            var path = Path.Combine(_epubTextFolder, "title.xhtml");
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

        public void WriteToc()
        {
            Console.WriteLine("writing TOC data...");
            var toc = new OebpsTextToc(_publicationMeta, _chapterSet, _epubTextFolder);
            toc.Write();
        }

        internal static void Throw(string errorMessage)
        {
            throw new Exception(errorMessage);
        }

        internal void SetChapterTemplate(string csxRoot)
        {
            var chapterTemplateFile = Path.Combine(csxRoot, "chapter-template.xhtml");
            if (!File.Exists(chapterTemplateFile))
                Throw(string.Format("ERROR: cannot find {0}", chapterTemplateFile));

            _chapterTemplate = XDocument.Load(chapterTemplateFile);
        }

        internal void SetEpubOebpsFolder()
        {
            _epubOebpsFolder = Path.Combine(_publicationRoot, "epub", "OEBPS");
            if (!Directory.Exists(_epubOebpsFolder))
                Throw(string.Format("ERROR: cannot find {0}", _epubOebpsFolder));
        }

        internal void SetEpubTextFolder()
        {
            _epubTextFolder = Path.Combine(_publicationRoot, "epub", "OEBPS", "Text");
            if (!Directory.Exists(_epubTextFolder))
                Throw(string.Format("ERROR: cannot find {0}", _epubTextFolder));
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

        internal void SetMarkdownFolder()
        {
            _markdownFolder = Path.Combine(_publicationRoot, "markdown");
        }

        internal void SetPublicationMetaAndChapterSet()
        {
            var publicationMetaFile = Path.Combine(_publicationRoot, "json", "publication-meta.json");
            if (!File.Exists(publicationMetaFile))
                Throw(string.Format("ERROR: cannot find {0}", publicationMetaFile));

            _publicationMeta = JObject.Parse(File.ReadAllText(publicationMetaFile));
            _chapterSet = _publicationMeta["publication"]["chapterSet"].ToObject<Dictionary<string, string>>();
        }

        internal void SetPublicationRoot(string csxRoot)
        {
            _csxRootInfo = new DirectoryInfo(csxRoot);
            _publicationRoot = _csxRootInfo.Parent.FullName;
        }

        Dictionary<string, string> _chapterSet;
        DirectoryInfo _csxRootInfo;
        JObject _publicationMeta;
        string _epubOebpsFolder;
        string _epubTextFolder;
        string _isbn13;
        string _markdownFolder;
        string _publicationRoot;
        XDocument _chapterTemplate;
    }
}
