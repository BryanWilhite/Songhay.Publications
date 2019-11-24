using Markdig;
using Songhay.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Songhay.Publications.Models
{
    public class PublicationChapter
    {
        public PublicationChapter(KeyValuePair<string, string> chapterPair, XDocument chapterTemplate, string chapterDirectory)
        {
            _chapterPair = chapterPair;
            _chapter = new XDocument(chapterTemplate);
            _chapterDirectoryInfo = new DirectoryInfo(chapterDirectory);
        }

        public XElement GetChapterBodyElement()
        {
            var xhtml = PublicationNamespaces.Xhtml;

            var e = _chapter.Root
                .Element(xhtml + "body")
                .Element(xhtml + "div")
                .Element(xhtml + "div");
            Console.WriteLine(e.Value);
            return e;
        }

        public XElement GetH1Element()
        {
            var xhtml = PublicationNamespaces.Xhtml;

            var e = _chapter.Root
                .Element(xhtml + "body")
                .Element(xhtml + "div")
                .Element(xhtml + "h1");
            Console.WriteLine(e.Value);
            return e;
        }

        public XElement GetTitleElement()
        {
            var xhtml = PublicationNamespaces.Xhtml;

            var e = _chapter.Root
                .Element(xhtml + "head")
                .Element(xhtml + "title");
            Console.WriteLine(e.Value);
            return e;
        }

        public string GenerateXhtml()
        {
            var titleElement = GetTitleElement();
            var h1Element = GetH1Element();
            var divPlaceholderElement = GetChapterBodyElement();

            var chapterBodyBuilder = new StringBuilder();
            var divPageBreak = @"<div style=""page-break-before:always;""><span epub:type=""pagebreak"" /></div>";

            _chapterDirectoryInfo
                .EnumerateFiles("*.md")
                .Select((markdownFile, i) => new { markdownFile, i })
                .ForEachInEnumerable(a =>
                {
                    var markdownFile = a.markdownFile;
                    var i = a.i;

                    if (i > 0) chapterBodyBuilder.AppendLine(divPageBreak);

                    Console.WriteLine("    markdown file {0}...", markdownFile);
                    var markdown = File.ReadAllText(markdownFile.FullName);
                    var raw = Markdown.ToHtml(markdown);
                    var rawElement = XElement.Parse(string.Format("<raw>{0}</raw>", raw));

                    Console.WriteLine("    looking for h2 elements wrapped by p elements...");
                    var h2Elements = rawElement.Descendants("h2");
                    h2Elements.ToArray().ForEachInEnumerable(h2Element =>
                    {
                        if (h2Element.Parent.Name != "p") return;
                        h2Element.Parent.ReplaceWith(h2Element);
                    });

                    Console.WriteLine("    looking for h3 elements wrapped by p elements...");
                    var h3Elements = rawElement.Descendants("h3");
                    h3Elements.ToArray().ForEachInEnumerable(h3Element =>
                    {
                        if (h3Element.Parent.Name != "p") return;
                        h3Element.Parent.ReplaceWith(h3Element);
                    });

                    Console.WriteLine("    looking for white-space-preservation blocks...");
                    var divElements = rawElement
                        .Elements("div")
                        .Where(div => div.Attribute("class").Value.Contains("white-space-preservation"));

                    divElements.ForEachInEnumerable(div =>
                    {
                        var u00A0 = " ";
                        var twoSpaces = "  ";
                        var pElements = div.Elements("p").Where(p => p.Value.Contains(twoSpaces));
                        pElements.ForEachInEnumerable(p => p.Value = p.Value
                            .Replace("&#160;", u00A0)
                            .Replace(twoSpaces, string.Concat(u00A0, u00A0)));
                    });

                    rawElement.Elements().ForEachInEnumerable(e => chapterBodyBuilder.Append(e.ToString()));
                });

            titleElement.Value = _chapterPair.Value;
            h1Element.Value = _chapterPair.Value;

            var xhtml = PublicationNamespaces.Xhtml;
            var ops = PublicationNamespaces.IdpfOpenPackagingStructure;

            var chapterBody = string.Format(
                @"<div class=""chapter-body"" xmlns=""{1}"" xmlns:epub=""{2}"">{0}</div>",
                chapterBodyBuilder.ToString(), xhtml, ops);

            var chapterBodyElement = XElement.Parse(chapterBody);
            divPlaceholderElement.ReplaceWith(chapterBodyElement.Elements());

            return _chapter.ToString();
        }

        readonly DirectoryInfo _chapterDirectoryInfo;
        readonly XDocument _chapter;
        KeyValuePair<string, string> _chapterPair;
    }
}
