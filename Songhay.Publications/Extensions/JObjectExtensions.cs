using Newtonsoft.Json.Linq;
using Songhay.Diagnostics;
using Songhay.Publications.Models;
using Songhay.Extensions;
using System;
using System.Diagnostics;
using System.Linq;

namespace Songhay.Publications.Extensions
{
    /// <summary>
    /// Extensions of <see cref="JObject"/>
    /// </summary>
    public static class JObjectExtensions
    {
        static JObjectExtensions() => traceSource = TraceSources
                .Instance
                .GetTraceSourceFromConfiguredName()
                .WithSourceLevels();

        static readonly TraceSource traceSource;

        /// <summary>
        /// Converts the <see cref="JObject"/> to presentation <see cref="Segment"/>.
        /// </summary>
        /// <param name="jObject">The j object.</param>
        /// <returns></returns>
        public static Segment ToPresentation(this JObject jObject)
        {
            if (jObject == null) return null;

            var rootProperty = "presentation";

            traceSource?.TraceVerbose($"Converting {rootProperty} JSON to {nameof(Segment)} with descendants...");

            var jPresentation = jObject.GetJObject(rootProperty);
            var isPostedToServer = jPresentation.GetValue<bool>("is-posted-to-server");
            if (isPostedToServer)
            {
                var postedDate = jPresentation.GetValue<DateTime>("posted-date");
                traceSource?.TraceVerbose($"This {rootProperty} was already posted on {postedDate}.");
                return null;
            }

            var jSegment = jPresentation.GetValue<JObject>(nameof(Segment));
            var segment = jSegment.FromJObject<ISegment, Segment>();
            if (segment == null)
            {
                traceSource?.TraceError($"The expected {nameof(Segment)} is not here.");
                return null;
            }

            var clientId = segment.ClientId;
            if (string.IsNullOrEmpty(clientId))
            {
                traceSource?.TraceError("The expected Client ID is not here.");
                return null;
            }

            traceSource?.TraceVerbose($"{nameof(Segment)}: {segment}");
            var validationResults = segment.ToValidationResults();
            if (validationResults.Any())
            {
                traceSource?.TraceError($"{nameof(Segment)} validation error(s)!");
                traceSource?.TraceError(validationResults.ToDisplayString());
                return null;
            }

            var jDocuments = jSegment.GetJArray(nameof(segment.Documents), throwException: true);
            if (!jDocuments.OfType<JObject>().Any())
            {
                traceSource?.TraceError($"The expected JObject {nameof(Document)} enumeration is not here.");
                return null;
            }

            jDocuments.OfType<JObject>().ForEachInEnumerable(i =>
            {
                var document = i.FromJObject<IDocument, Document>();
                if (document == null)
                {
                    traceSource?.TraceError($"The expected {nameof(Document)} is not here.");
                    return;
                }

                if (document.ClientId != clientId)
                {
                    traceSource?.TraceError($"The expected {nameof(Document)} Client ID is not here.");
                    return;
                }

                traceSource?.TraceVerbose($"{nameof(Document)}: {document}");
                var validationResultsForDocument = document.ToValidationResults();
                if (validationResultsForDocument.Any())
                {
                    traceSource?.TraceError($"{nameof(Document)} validation error(s)!");
                    traceSource?.TraceError(validationResultsForDocument.ToDisplayString());
                    return;
                }

                var jFragments = i.GetJArray(nameof(document.Fragments), throwException: false);
                if ((jFragments == null) || !jFragments.OfType<JObject>().Any())
                {
                    traceSource?.TraceWarning($"The JObject {nameof(Fragment)} enumeration is not here.");
                    segment.Documents.Add(document);
                    return;
                }

                jFragments.OfType<JObject>().ForEachInEnumerable(j =>
                {
                    var fragment = j.FromJObject<IFragment, Fragment>();
                    if (fragment == null)
                    {
                        traceSource?.TraceError($"The expected {nameof(Fragment)} is not here.");
                        return;
                    }

                    if (fragment.ClientId != clientId)
                    {
                        traceSource?.TraceError($"The expected {nameof(Fragment)} Client ID is not here.");
                        return;
                    }

                    traceSource?.TraceVerbose($"{nameof(Fragment)}: {fragment}");
                    var validationResultsForFragment = fragment.ToValidationResults();
                    if (validationResultsForFragment.Any())
                    {
                        traceSource?.TraceError($"{nameof(Fragment)} validation error(s)!");
                        traceSource?.TraceError(validationResultsForFragment.ToDisplayString());
                        return;
                    }

                    document.Fragments.Add(fragment);
                });

                segment.Documents.Add(document);
            });

            return segment;
        }
    }
}
