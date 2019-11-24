﻿using Markdig;
using System;
using System.IO;
using System.Xml.Linq;

namespace Songhay.Publications.Models
{
    /// <summary>
    /// Defines the content to write the
    /// <see cref="PublicationFiles.EpubFileDedication"/> file.
    /// </summary>
    /// <remarks>
    ///  Open eBook Publication Structure (OEBPS),
    ///  is a legacy e-book format which
    ///  has been superseded by the EPUB format.
    ///
    /// https://en.wikipedia.org/wiki/Open_eBook
    /// </remarks>
    public class OebpsTextDedication
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OebpsTextDedication"/> class.
        /// </summary>
        /// <param name="templateRoot">the root directory of the EPUB template files</param>
        /// <param name="epubTextDirectory">conventional <c>epub/OEBPS/Text</c> directory</param>
        /// <param name="markdownDirectory">conventional <c>markdown</c> directory</param>
        public OebpsTextDedication(string templateRoot, string epubTextDirectory, string markdownDirectory)
        {
            _epubTextDirectory = epubTextDirectory;
            _markdownDirectory = markdownDirectory;
            this.SetTemplate(templateRoot);
        }

        /// <summary>
        /// Writes the
        /// <see cref="PublicationFiles.EpubFileDedication"/> file.
        /// </summary>
        public void Write()
        {
            var xhtml = PublicationNamespaces.Xhtml;

            var xhtmlFile = PublicationContext.GetCombinedPath(_epubTextDirectory, PublicationFiles.EpubFileDedication, shouldBeFile: true);
            var markdownFile = PublicationContext.GetCombinedPath(_markdownDirectory, PublicationFiles.EpubMarkdownDedication, shouldBeFile: true);

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

        internal void SetTemplate(string csxRoot)
        {
            var dedicationTemplateFile = PublicationContext.GetCombinedPath(csxRoot, PublicationFiles.EpubTemplateDedication, shouldBeFile: true);
            _dedicationTemplate = XDocument.Load(dedicationTemplateFile);
        }

        readonly string _epubTextDirectory;
        readonly string _markdownDirectory;
        XDocument _dedicationTemplate;
    }
}
