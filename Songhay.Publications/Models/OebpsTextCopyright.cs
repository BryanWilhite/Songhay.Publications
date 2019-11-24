using Newtonsoft.Json.Linq;
using Songhay.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Songhay.Publications.Models
{
    public class OebpsTextCopyright
    {
        public OebpsTextCopyright(JObject publicationMeta, string epubTextDirectory)
        {
            _publicationMeta = publicationMeta;
            _documentPath = PublicationContext.GetCombinedPath(epubTextDirectory, PublicationFiles.EpubFileCopyright, shouldBeFile: true);
            _document = XDocument.Load(_documentPath);
            this.SetSpans();
        }

        public void Write()
        {
            var pubYear = _publicationMeta
                .GetJObject("publication")
                .GetValue<string>("publicationDate");
            pubYear = DateTime.Parse(pubYear).Year.ToString();

            var jPub = _publicationMeta.GetJObject("publication");

            var pubAuthor = jPub.GetValue<string>("author");
            var pubInquiries = jPub.GetValue<string>("inquiries");
            var pubCoverArtCredits = jPub.GetValue<string>("coverArtCredits");
            var pubEpubPublicationDate = jPub.GetValue<string>("epubPublicationDate");

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

        readonly JObject _publicationMeta;
        readonly string _documentPath;
        readonly XDocument _document;
        IEnumerable<XElement> _spans;
    }
}
