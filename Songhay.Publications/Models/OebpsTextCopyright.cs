using Newtonsoft.Json.Linq;
using Songhay.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Songhay.Publications.Models
{
    public class OebpsTextCopyright
    {
        public OebpsTextCopyright(JObject publicationMeta, string epubTextFolder)
        {
            _publicationMeta = publicationMeta;
            _documentPath = Path.Combine(epubTextFolder, "copyright.xhtml");
            _document = XDocument.Load(_documentPath);
            this.SetSpans();
        }

        public void Write()
        {
            var pubYear = _publicationMeta["publication"]["publicationDate"].Value<string>();
            pubYear = DateTime.Parse(pubYear).Year.ToString();

            var pubAuthor = _publicationMeta["publication"]["author"].Value<string>();
            var pubInquiries = _publicationMeta["publication"]["inquiries"].Value<string>();
            var pubCoverArtCredits = _publicationMeta["publication"]["coverArtCredits"].Value<string>();
            var pubEpubPublicationDate = _publicationMeta["publication"]["epubPublicationDate"].Value<string>();

            var span = GetSpan("pub-year");
            span.Value = pubYear + " ";

            var spans = GetSpans("pub-author");
            spans.ForEachInEnumerable(i => i.Value = pubAuthor);

            span = GetSpan("pub-inquiries");
            span.Value = pubInquiries;

            span = GetSpan("pub-cover-art-credits");
            span.Value = pubCoverArtCredits;

            span = GetSpan("pub-epub-date");
            span.Value = pubEpubPublicationDate;

            EpubUtility.SaveAsUnicodeWithBom(_document, _documentPath);
        }

        XElement GetSpan(string @class)
        {
            return _spans
                .Single(i =>
                {
                    var a = i.Attribute("class");
                    return (a != null) && (a.Value == @class);
                });
        }

        IEnumerable<XElement> GetSpans(string @class)
        {
            return _spans.Where(i =>
            {
                var a = i.Attribute("class");
                return (a != null) && (a.Value == @class);
            });
        }

        void SetSpans()
        {
            var xhtml = PublicationNamespaces.Xhtml;

            var bodyDivElement = _document.Root
                .Element(xhtml + "body")
                .Element(xhtml + "div");

            _spans = bodyDivElement
                .Descendants(xhtml + "span");
        }

        IEnumerable<XElement> _spans;
        JObject _publicationMeta;
        string _documentPath;
        XDocument _document;
    }
}
