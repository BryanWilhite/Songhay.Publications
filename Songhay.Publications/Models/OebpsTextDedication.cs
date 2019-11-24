using Markdig;
using System;
using System.IO;
using System.Xml.Linq;

namespace Songhay.Publications.Models
{
    public class OebpsTextDedication
    {
        public OebpsTextDedication(string csxRoot, string epubTextFolder, string markdownFolder)
        {
            _epubTextFolder = epubTextFolder;
            _markdownFolder = markdownFolder;
            this.SetTemplate(csxRoot);
        }

        public void Write()
        {
            var xhtml = PublicationNamespaces.Xhtml;

            var xhtmlFile = Path.Combine(_epubTextFolder, "dedication.xhtml");
            if (!File.Exists(xhtmlFile))
                PublicationContext.Throw(string.Format("ERROR: cannot find {0}", xhtmlFile));

            var markdownFile = Path.Combine(_markdownFolder, "author-dedication.md");
            if (!File.Exists(markdownFile))
                PublicationContext.Throw(string.Format("ERROR: cannot find {0}", markdownFile));

            Console.WriteLine("    markdown file {0}...", markdownFile);
            var markdown = File.ReadAllText(markdownFile);
            var raw = Markdown.ToHtml(markdown);
            var rawElement = XElement.Parse(string.Format(@"<div class=""rx raw tmp"" xmlns=""{0}"">{1}</div>", xhtml, raw));
            var dedicationDocument = new XDocument(_dedicationTemplate);
            var divElement = dedicationDocument.Root
                .Element(xhtml + "body")
                .Element(xhtml + "div")
                .Element(xhtml + "div");

            divElement.ReplaceWith(rawElement.Nodes());

            EpubUtility.SaveAsUnicodeWithBom(dedicationDocument, xhtmlFile);
        }

        void SetTemplate(string csxRoot)
        {
            var dedicationTemplateFile = Path.Combine(csxRoot, "dedication-template.xhtml");
            if (!File.Exists(dedicationTemplateFile))
                PublicationContext.Throw(string.Format("ERROR: cannot find {0}", dedicationTemplateFile));

            _dedicationTemplate = XDocument.Load(dedicationTemplateFile);
        }

        string _epubTextFolder;
        string _markdownFolder;
        string _documentPath;
        XDocument _document;
        XDocument _dedicationTemplate;
    }
}
