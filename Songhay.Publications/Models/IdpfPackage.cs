﻿using Newtonsoft.Json.Linq;
using Songhay.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Songhay.Publications.Models
{
    public class IdpfPackage
    {
        public IdpfPackage(JObject publicationMeta, string isbn13, Dictionary<string, string> chapterSet, string epubOebpsFolder)
        {
            _publicationMeta = publicationMeta;
            _isbn13 = isbn13;
            _chapterSet = chapterSet;
            _idpfDocumentPath = Path.Combine(epubOebpsFolder, "content.opf");
            _idpfDocument = XDocument.Load(_idpfDocumentPath);
        }

        public void SetPublicationMeta()
        {
            this.SetDublinCoreMeta();
            this.SetManifestItemElementsForChapters();
            this.SetSpineItemRefElementsForChapters();

            EpubUtility.SaveAsUnicodeWithBom(_idpfDocument, _idpfDocumentPath);
        }

        XElement GetItemRef(string chapterId)
        {
            var opf = PublicationNamespaces.IdpfOpenPackagingFormat;

            return new XElement(opf + "itemref",
                new XAttribute("idref", chapterId));
        }

        XElement GetManifestItem(string chapterId)
        {
            var opf = PublicationNamespaces.IdpfOpenPackagingFormat;

            return new XElement(opf + "item",
                new XAttribute("href", string.Format("Text/{0}.xhtml", chapterId)),
                new XAttribute("id", chapterId),
                new XAttribute("media-type", "application/xhtml+xml")
                );
        }

        void SetDublinCoreMeta()
        {
            var dc = PublicationNamespaces.DublinCore;
            var opf = PublicationNamespaces.IdpfOpenPackagingFormat;

            var metadataElement = _idpfDocument.Root
                .Element(opf + "metadata");

            var titleElement = metadataElement.Element(dc + "title");
            var identifierElement = metadataElement.Element(dc + "identifier");
            var creatorElement = metadataElement.Element(dc + "creator");
            var publisherElement = metadataElement.Element(dc + "publisher");
            var dateElement = metadataElement.Element(dc + "date");

            titleElement.Value = _publicationMeta["publication"]["title"].Value<string>();
            identifierElement.Value = _isbn13;
            creatorElement.Value = _publicationMeta["publication"]["author"].Value<string>();
            publisherElement.Value = _publicationMeta["publication"]["publisher"].Value<string>();
            dateElement.Value = _publicationMeta["publication"]["publicationDate"].Value<string>();
        }

        void SetManifestItem(XElement item, string id)
        {
            var href = string.Format("Text/{0}.xhtml", id);
            var hrefAttribute = item.Attribute("href");
            var idAttribute = item.Attribute("id");

            hrefAttribute.Value = href;
            idAttribute.Value = id;
        }

        void SetManifestItemElementsForChapters()
        {
            Console.WriteLine("setting manifest item elements for chapters...");

            var opf = PublicationNamespaces.IdpfOpenPackagingFormat;

            var items = _idpfDocument.Root
                .Element(opf + "manifest")
                .Elements(opf + "item");

            XElement templatedChapterElement = null;
            var newChapterElementList = new List<XElement>();

            _chapterSet.Keys
                .Select((chapterId, i) => new { chapterId, i })
                .ForEachInEnumerable(a =>
                {
                    var chapterId = a.chapterId;
                    var i = a.i;

                    var chapterElement = items.SingleOrDefault(item =>
                    {
                        var id = item.Attribute("id").Value;
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
                        this.SetManifestItem(templatedChapterElement, chapterId);
                    }
                    else if (canAddNavPoint)
                    {
                        var @new = GetManifestItem(chapterId);
                        this.SetManifestItem(@new, chapterId);
                        newChapterElementList.Add(@new);
                    }
                });

            if (!newChapterElementList.Any()) return;

            Console.WriteLine("adding new elements under templated element...");
            templatedChapterElement.AddAfterSelf(newChapterElementList.ToArray());
        }

        void SetSpineItemref(XElement itemref, string idref)
        {
            var idrefAttribute = itemref.Attribute("idref");
            idrefAttribute.Value = idref;
        }

        void SetSpineItemRefElementsForChapters()
        {
            Console.WriteLine("setting spine itemref elements for chapters...");

            var opf = PublicationNamespaces.IdpfOpenPackagingFormat;

            var itemrefs = _idpfDocument.Root
                .Element(opf + "spine")
                .Elements(opf + "itemref");

            XElement templatedChapterElement = null;
            var newChapterElementList = new List<XElement>();

            _chapterSet.Keys
                .Select((chapterId, i) => new { chapterId, i })
                .ForEachInEnumerable(a =>
                {
                    var chapterId = a.chapterId;
                    var i = a.i;
                    var chapterElement = itemrefs.SingleOrDefault(itemref =>
                    {
                        var id = itemref.Attribute("idref").Value;
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
                        this.SetSpineItemref(templatedChapterElement, chapterId);
                    }
                    else if (canAddNavPoint)
                    {
                        var @new = GetItemRef(chapterId);
                        this.SetSpineItemref(@new, chapterId);
                        newChapterElementList.Add(@new);
                    }
                });

            if (!newChapterElementList.Any()) return;

            Console.WriteLine("adding new elements under templated element...");
            templatedChapterElement.AddAfterSelf(newChapterElementList.ToArray());
        }

        Dictionary<string, string> _chapterSet;
        JObject _publicationMeta;
        string _isbn13;
        string _idpfDocumentPath;
        XDocument _idpfDocument;
    }
}
