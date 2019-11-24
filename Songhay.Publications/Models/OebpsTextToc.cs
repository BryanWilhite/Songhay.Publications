using Newtonsoft.Json.Linq;
using Songhay.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Songhay.Publications.Models
{
    public class OebpsTextToc
    {
        public OebpsTextToc(JObject publicationMeta, Dictionary<string, string> chapterSet, string epubTextDirectory)
        {
            _publicationMeta = publicationMeta;
            _chapterSet = chapterSet;
            _documentPath = PublicationContext.GetCombinedPath(epubTextDirectory, PublicationFiles.EpubFileToc, shouldBeFile: true);
            _document = XDocument.Load(_documentPath);
        }

        public void Write()
        {
            var jPublication = _publicationMeta.GetJObject("publication");
            var title = jPublication.GetValue<string>("title");
            var author = jPublication.GetValue<string>("author");

            var xhtml = PublicationNamespaces.Xhtml;

            var h2Element = _document.Root
                .Element(xhtml + "body")
                .Element(xhtml + "div")
                .Element(xhtml + "h2");
            var spanElement = _document.Root
                .Element(xhtml + "body")
                .Element(xhtml + "div")
                .Element(xhtml + "h3")
                .Element(xhtml + "span");

            h2Element.Value = title;
            spanElement.Value = author;

            this.SetTocAnchors(_document);

            EpubUtility.SaveAsUnicodeWithBom(_document, _documentPath);
        }

        internal XElement GetTOCAnchor(string chapterId)
        {
            var xhtml = PublicationNamespaces.Xhtml;
            var hrefTemplate = GetTOCHrefTemplate();

            return new XElement(xhtml + "a",
                new XAttribute("href", string.Format(hrefTemplate, chapterId)),
                GetTOCChapterValue(chapterId)
                );
        }

        internal string GetTOCChapterValue(string chapterId)
        {
            return _chapterSet[chapterId];
        }

        internal string GetTOCHrefTemplate()
        {
            return "../Text/{0}.xhtml";
        }

        internal void SetTocAnchor(XElement a, string chapterId)
        {
            var hrefTemplate = GetTOCHrefTemplate();

            a.Value = GetTOCChapterValue(chapterId);

            var hrefAttribute = a.Attribute("href");
            hrefAttribute.Value = string.Format(hrefTemplate, chapterId);
        }

        internal void SetTocAnchors(XDocument tocDocument)
        {
            Console.WriteLine("setting TOC chapter anchors...");

            var xhtml = PublicationNamespaces.Xhtml;

            var anchors = tocDocument.Root
                .Element(xhtml + "body")
                .Element(xhtml + "div")
                .Elements(xhtml + "a");

            XElement templatedChapterElement = null;
            var newChapterElementList = new List<XElement>();
            var hrefTemplate = GetTOCHrefTemplate();

            _chapterSet.Keys
                .Select((chapterId, i) => new { chapterId, i })
                .ForEachInEnumerable(a =>
                {
                    var chapterId = a.chapterId;
                    var i = a.i;

                    var chapterElement = anchors.SingleOrDefault(item =>
                    {
                        var href = item.Attribute("href").Value;
                        return href == string.Format(hrefTemplate, chapterId);
                    });

                    var canAddNavPoint = (chapterElement == null) && (i > 0);
                    var isFirstChapterIdError = (chapterElement == null) && (i == 0);
                    var isFirstChapterId = (chapterElement != null) && (i == 0);

                    if (isFirstChapterIdError)
                    {
                        PublicationContext.Throw(string.Format("ERROR: cannot find templated element {0}", chapterId));
                    }
                    else if (isFirstChapterId)
                    {
                        templatedChapterElement = chapterElement;
                        this.SetTocAnchor(templatedChapterElement, chapterId);
                    }
                    else if (canAddNavPoint)
                    {
                        var @new = GetTOCAnchor(chapterId);
                        this.SetTocAnchor(@new, chapterId);
                        newChapterElementList.Add(new XElement(xhtml + "br"));
                        newChapterElementList.Add(@new);
                    }
                });

            if (!newChapterElementList.Any()) return;

            Console.WriteLine("adding new elements under templated element...");
            templatedChapterElement.AddAfterSelf(newChapterElementList.ToArray());
        }

        readonly Dictionary<string, string> _chapterSet;
        readonly JObject _publicationMeta;
        readonly string _documentPath;
        readonly XDocument _document;
    }
}
