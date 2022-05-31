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
    public static partial class JObjectExtensions
    {
        static JObjectExtensions() => TraceSource = TraceSources
                .Instance
                .GetTraceSourceFromConfiguredName()
                .WithSourceLevels();

        static readonly TraceSource TraceSource;

        /// <summary>
        /// Gets the publication command.
        /// </summary>
        /// <param name="jObject">The <see cref="JObject"/>.</param>
        /// <returns></returns>
        public static string GetPublicationCommand(this JObject jObject) => jObject.GetValue<string>("command");

        /// <summary>
        /// Converts the <see cref="JObject"/> to presentation <see cref="Segment"/>.
        /// </summary>
        /// <param name="jObject">The j object.</param>
        /// <returns></returns>
        public static Segment ToPresentation(this JObject jObject)
        {
            if (jObject == null) return null;

            var rootProperty = "presentation";

            TraceSource?.TraceVerbose($"Converting {rootProperty} JSON to {nameof(Segment)} with descendants...");

            var jPresentation = jObject.GetJObject(rootProperty);
            var isPostedToServer = jPresentation.GetValue<bool>("is-posted-to-server");
            if (isPostedToServer)
            {
                var postedDate = jPresentation.GetValue<DateTime>("posted-date");
                TraceSource?.TraceVerbose($"This {rootProperty} was already posted on {postedDate}.");
                return null;
            }

            var jSegment = jPresentation.GetValue<JObject>(nameof(Segment));
            var segment = jSegment.FromJObject<ISegment, Segment>();
            if (segment == null)
            {
                TraceSource?.TraceError($"The expected {nameof(Segment)} is not here.");
                return null;
            }

            var clientId = segment.ClientId;
            if (string.IsNullOrEmpty(clientId))
            {
                TraceSource?.TraceError("The expected Client ID is not here.");
                return null;
            }

            TraceSource?.TraceVerbose($"{nameof(Segment)}: {segment}");
            var validationResult = segment.ToValidationResult();
            if (!validationResult.IsValid)
            {
                TraceSource?.TraceError($"{nameof(Segment)} validation error(s)!");
                TraceSource?.TraceError(validationResult.Errors.First().ErrorMessage);
                return null;
            }

            var jDocuments = jSegment.GetJArray(nameof(segment.Documents), throwException: true);
            if (!jDocuments.OfType<JObject>().Any())
            {
                TraceSource?.TraceError($"The expected JObject {nameof(Document)} enumeration is not here.");
                return null;
            }

            jDocuments.OfType<JObject>().ForEachInEnumerable(i =>
            {
                var document = i.FromJObject<IDocument, Document>();
                if (document == null)
                {
                    TraceSource?.TraceError($"The expected {nameof(Document)} is not here.");
                    return;
                }

                if (document.ClientId != clientId)
                {
                    TraceSource?.TraceError($"The expected {nameof(Document)} Client ID is not here.");
                    return;
                }

                TraceSource?.TraceVerbose($"{nameof(Document)}: {document}");
                var validationResultForDocument = document.ToValidationResult();
                if (!validationResultForDocument.IsValid)
                {
                    TraceSource?.TraceError($"{nameof(Document)} validation error(s)!");
                    TraceSource?.TraceError(validationResultForDocument.Errors.First().ErrorMessage);
                    return;
                }

                var jFragments = i.GetJArray(nameof(document.Fragments), throwException: false);
                if ((jFragments == null) || !jFragments.OfType<JObject>().Any())
                {
                    TraceSource?.TraceWarning($"The JObject {nameof(Fragment)} enumeration is not here.");
                    segment.Documents.Add(document);
                    return;
                }

                jFragments.OfType<JObject>().ForEachInEnumerable(j =>
                {
                    var fragment = j.FromJObject<IFragment, Fragment>();
                    if (fragment == null)
                    {
                        TraceSource?.TraceError($"The expected {nameof(Fragment)} is not here.");
                        return;
                    }

                    if (fragment.ClientId != clientId)
                    {
                        TraceSource?.TraceError($"The expected {nameof(Fragment)} Client ID is not here.");
                        return;
                    }

                    TraceSource?.TraceVerbose($"{nameof(Fragment)}: {fragment}");
                    var validationResultForFragment = fragment.ToValidationResult();
                    if (!validationResultForFragment.IsValid)
                    {
                        TraceSource?.TraceError($"{nameof(Fragment)} validation error(s)!");
                        TraceSource?.TraceError(validationResultForFragment.Errors.First().ErrorMessage);
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
