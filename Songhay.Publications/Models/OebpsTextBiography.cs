using Markdig;
using System;
using System.IO;
using System.Xml.Linq;

namespace Songhay.Publications.Models
{
    public class OebpsTextBiography
    {
        public OebpsTextBiography(string csxRoot, string epubTextDirectory, string markdownDirectory)
        {
            _epubTextDirectory = epubTextDirectory;
            _markdownDirectory = markdownDirectory;
            this.SetBiographyTemplate(csxRoot);
        }

        public void Write()
        {
            var xhtml = PublicationNamespaces.Xhtml;

            var xhtmlFile = PublicationContext.GetCombinedPath(_epubTextDirectory, PublicationFiles.EpubFileBiography, shouldBeFile: true);
            var markdownFile = PublicationContext.GetCombinedPath(_markdownDirectory, PublicationFiles.EpubMarkdownBiography, shouldBeFile: true);

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

        internal void SetBiographyTemplate(string csxRoot)
        {
            var biographyTemplateFile = PublicationContext.GetCombinedPath(csxRoot, PublicationFiles.EpubTemplateBiography, shouldBeFile: true);
            _biographyTemplate = XDocument.Load(biographyTemplateFile);
        }

        readonly string _epubTextDirectory;
        readonly string _markdownDirectory;
        XDocument _biographyTemplate;
    }
}
