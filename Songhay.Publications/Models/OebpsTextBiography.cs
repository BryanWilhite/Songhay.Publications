using Markdig;
using System;
using System.IO;
using System.Xml.Linq;

namespace Songhay.Publications.Models
{
    public class OebpsTextBiography
    {
        public OebpsTextBiography(string csxRoot, string epubTextFolder, string markdownFolder)
        {
            _epubTextFolder = epubTextFolder;
            _markdownFolder = markdownFolder;
            this.SetBiographyTemplate(csxRoot);
        }

        public void Write()
        {
            var xhtml = PublicationNamespaces.Xhtml;

            var xhtmlFile = Path.Combine(_epubTextFolder, "biography.xhtml");
            if (!File.Exists(xhtmlFile))
                PublicationContext.Throw(string.Format("ERROR: cannot find {0}", xhtmlFile));

            var markdownFile = Path.Combine(_markdownFolder, "author-biography.md");
            if (!File.Exists(markdownFile))
                PublicationContext.Throw(string.Format("ERROR: cannot find {0}", markdownFile));

            Console.WriteLine("    markdown file {0}...", markdownFile);
            var markdown = File.ReadAllText(markdownFile);
            var raw = Markdown.ToHtml(markdown);
            var rawElement = XElement.Parse(string.Format(@"<div class=""rx raw tmp"" xmlns=""{0}"">{1}</div>", xhtml, raw));
            var biographyDocument = new XDocument(_biographyTemplate);
            var divElement = biographyDocument.Root
                .Element(xhtml + "body")
                .Element(xhtml + "div")
                .Element(xhtml + "div");

            divElement.ReplaceWith(rawElement.Nodes());

            EpubUtility.SaveAsUnicodeWithBom(biographyDocument, xhtmlFile);
        }

        void SetBiographyTemplate(string csxRoot)
        {
            var biographyTemplateFile = Path.Combine(csxRoot, "biography-template.xhtml");
            if (!File.Exists(biographyTemplateFile))
                PublicationContext.Throw(string.Format("ERROR: cannot find {0}", biographyTemplateFile));

            _biographyTemplate = XDocument.Load(biographyTemplateFile);
        }

        string _epubTextFolder;
        string _markdownFolder;
        string _documentPath;
        XDocument _document;
        XDocument _biographyTemplate;
    }
}
