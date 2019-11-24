using Newtonsoft.Json.Linq;
using Songhay.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Songhay.Publications.Models
{
    public class DaisyConsortiumNcx
    {
        public DaisyConsortiumNcx(JObject publicationMeta, string isbn13, Dictionary<string, string> chapterSet, string epubOebpsFolder)
        {
            _publicationMeta = publicationMeta;
            _isbn13 = isbn13;
            _chapterSet = chapterSet;
            _ncxDocumentPath = Path.Combine(epubOebpsFolder, "toc.ncx");
            _ncxDocument = XDocument.Load(_ncxDocumentPath);
        }

        public void SetPublicationMeta()
        {
            Console.WriteLine("setting publication meta...");

            var ncx = PublicationNamespaces.DaisyNcx;

            this.SetNcxMeta();
            this.SetNcxDocTitle();

            var navPoints = _ncxDocument.Root
                .Element(ncx + "navMap")
                .Elements(ncx + "navPoint");

            this.SetChapterNavPoints(navPoints);
            this.UpdateNavPointPlayOrder(navPoints);
            EpubUtility.SaveAsUnicodeWithBom(_ncxDocument, _ncxDocumentPath);
        }

        XElement GetNavPoint(string chapterId)
        {
            var ncx = PublicationNamespaces.DaisyNcx;

            return new XElement(ncx + "navPoint",
                new XAttribute("class", "chapter"),
                new XAttribute("id", chapterId),
                new XAttribute("playOrder", string.Empty),
                new XElement(ncx + "navLabel", new XElement(ncx + "text")),
                new XElement(ncx + "content",
                    new XAttribute("src", string.Format("Text/{0}.xhtml", chapterId)))
                );
        }

        void SetChapterNavPoints(IEnumerable<XElement> navPoints)
        {
            Console.WriteLine("setting navPoint elements for chapters...");

            XElement templatedChapterElement = null;
            var newChapterElementList = new List<XElement>();

            _chapterSet.Keys
                .Select((chapterId, i) => new { chapterId, i })
                .ForEachInEnumerable(a =>
                {
                    var chapterId = a.chapterId;
                    var i = a.i;

                    var chapterElement = navPoints.SingleOrDefault(navPoint =>
                    {
                        var id = navPoint.Attribute("id").Value;
                        return chapterId.Equals(id);
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
                        this.SetChapterNavPointText(templatedChapterElement, _chapterSet[chapterId]);
                    }
                    else if (canAddNavPoint)
                    {
                        var @new = GetNavPoint(chapterId);
                        this.SetChapterNavPointText(@new, _chapterSet[chapterId]);
                        newChapterElementList.Add(@new);
                    }
                });

            if (!newChapterElementList.Any()) return;

            Console.WriteLine("adding new elements under templated element...");
            templatedChapterElement.AddAfterSelf(newChapterElementList.ToArray());
        }

        void SetChapterNavPointText(XElement navPoint, string text)
        {
            Console.WriteLine("setting navPoint navLabel text...");

            var ncx = PublicationNamespaces.DaisyNcx;

            var textElement = navPoint
                .Element(ncx + "navLabel")
                .Element(ncx + "text");
            textElement.Value = text;
        }

        void SetNcxDocTitle()
        {
            Console.WriteLine("setting ncx docTitle title...");

            var ncx = PublicationNamespaces.DaisyNcx;
            var title = _publicationMeta["publication"]["title"].Value<string>();

            var textElement = _ncxDocument.Root
                .Element(ncx + "docTitle")
                .Element(ncx + "text");
            textElement.Value = title;
        }

        void SetNcxMeta()
        {
            Console.WriteLine("setting ncx docTitle meta...");

            var ncx = PublicationNamespaces.DaisyNcx;

            var content = _ncxDocument.Root
                .Element(ncx + "head")
                .Element(ncx + "meta")
                .Attribute("content");
            content.Value = _isbn13;
        }

        void UpdateNavPointPlayOrder(IEnumerable<XElement> navPoints)
        {
            Console.WriteLine("updating navPoint playOrder...");

            navPoints
                .Select((navPoint, i) => new { navPoint, i })
                .ForEachInEnumerable(a =>
                {
                    var playOrder = a.navPoint.Attribute("playOrder");
                    playOrder.Value = (a.i + 1).ToString();
                });
        }

        Dictionary<string, string> _chapterSet;
        JObject _publicationMeta;
        XDocument _ncxDocument;
        string _isbn13;
        string _ncxDocumentPath;
    }
}
