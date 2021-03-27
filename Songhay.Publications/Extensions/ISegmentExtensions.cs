using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CloneExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Songhay.Diagnostics;
using Songhay.Extensions;
using Songhay.Models;
using Songhay.Publications.Models;

namespace Songhay.Publications.Extensions
{
    /// <summary>
    /// Extensions of <see cref="ISegment"/>
    /// </summary>
    public static class ISegmentExtensions
    {
        static ISegmentExtensions() => traceSource = TraceSources
            .Instance
            .GetTraceSourceFromConfiguredName()
            .WithSourceLevels();

        static readonly TraceSource traceSource;

        /// <summary>
        /// Clones the instance of <see cref="ISegment"/>.
        /// </summary>
        /// <param name="data">The document.</param>
        /// <returns><see cref="Segment"/></returns>
        public static Segment Clone(this ISegment data)
        {
            return data?.GetClone(CloneInitializers.Publications) as Segment;
        }

        /// <summary>
        /// Returns and traces the first <see cref="ISegment"/>
        /// based on the specified predicate.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public static ISegment GetSegmentByPredicate(this IEnumerable<ISegment> data, Func<ISegment, bool> predicate)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            var first = data.FirstOrDefault(predicate);

            traceSource?.TraceVerbose(first.ToDisplayText(showIdOnly: true));

            return first;
        }

        /// <summary>
        /// Returns <c>true</c> when the <see cref="ISegment"/>
        /// has any <see cref="Segment.Documents"/>.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool HasDocuments(this ISegment data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            var segment = data as Segment;
            if (segment == null) return false;

            if (!segment.Documents.Any())
            {
                traceSource?.TraceError($"The expected child {nameof(Segment.Documents)} are not here.");
                return false;
            };

            return true;
        }

        /// <summary>
        /// Sets the defaults.
        /// </summary>
        /// <param name="data">The data.</param>
        public static void SetDefaults(this ISegment data)
        {
            if (data == null) return;

            data.InceptDate = DateTime.Now;
            data.IsActive = true;
        }

        /// <summary>
        /// Converts the <see cref="ISegment"/> into human-readable display text.
        /// </summary>
        /// <param name="data">The data.</param>
        public static string ToDisplayText(this ISegment data)
        {
            return data.ToDisplayText(showIdOnly: false);
        }

        /// <summary>
        /// Converts the <see cref="ISegment"/> into human-readable display text.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="showIdOnly">when <c>true</c> then display identifiers only</param>
        public static string ToDisplayText(this ISegment data, bool showIdOnly)
        {
            if (data == null)
                return $"{nameof(ToDisplayText)}: the specified {nameof(ISegment)} is null.";

            var builder = new StringBuilder();

            var delimiter = string.Empty;

            if (data.SegmentId.HasValue)
            {
                builder.Append($"{nameof(data.SegmentId)}: {data?.SegmentId}");
                delimiter = ", ";
            }

            if (!string.IsNullOrWhiteSpace(data.ClientId))
            {
                builder.Append($"{delimiter}{nameof(data.ClientId)}: {data?.ClientId}");
                delimiter = ", ";
            }

            if (!showIdOnly)
            {
                if (!string.IsNullOrWhiteSpace(data.SegmentName))
                    builder.Append($"{delimiter}{nameof(data.SegmentName)}: {data?.SegmentName}");

                if (data.IsActive.HasValue)
                    builder.Append($"{delimiter}{nameof(data.IsActive)}: {data?.IsActive}");

                if (data.ParentSegmentId.HasValue)
                    builder.Append($"{delimiter}{nameof(data.ParentSegmentId)}: {data?.ParentSegmentId}");

                if (data.InceptDate.HasValue)
                    builder.Append($"{delimiter}{nameof(data.InceptDate)}: {data?.InceptDate}");
            }

            return builder.ToString();
        }

        /// <summary>
        /// Converts the <see cref="ISegment" /> to <see cref="JObject" />.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="useJavaScriptCase">when <c>true</c> use “camel” casing.</param>
        /// <returns></returns>
        public static JObject ToJObject(this ISegment data, bool useJavaScriptCase)
        {
            if (data == null) return null;

            var settings = JsonSerializationUtility
                .GetConventionalResolver<ISegment>(useJavaScriptCase)
                .ToJsonSerializerSettings();

            //TODO: consider making these optional:
            settings.MissingMemberHandling = MissingMemberHandling.Ignore;
            settings.NullValueHandling = NullValueHandling.Ignore;

            var jO = JObject.FromObject(data, JsonSerializer.Create(settings));

            return jO;
        }

        /// <summary>
        /// Converts the <see cref="ISegment"/> into a menu display item model.
        /// </summary>
        /// <param name="data">The data.</param>
        public static MenuDisplayItemModel ToMenuDisplayItemModel(this ISegment data)
        {
            return data.ToMenuDisplayItemModel(group: null);
        }

        /// <summary>
        /// Converts the <see cref="ISegment"/> into a menu display item model.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="group">The group.</param>
        public static MenuDisplayItemModel ToMenuDisplayItemModel(this ISegment data, IGroupable group)
        {
            if (data == null) return null;

            var @namespace = typeof(PublicationContext).Namespace;

            var dataOut = new MenuDisplayItemModel()
            {
                GroupDisplayText = (group == null) ? $"{@namespace}.{nameof(Segment)}" : group.GroupDisplayText,
                GroupId = (group == null) ? $"{@namespace}.{nameof(Segment)}".ToLowerInvariant() : group.GroupId,
                Id = data.SegmentId.GetValueOrDefault(),
                ItemName = data.SegmentName
            };
            return dataOut;
        }

        /// <summary>
        /// Converts the <see cref="ISegment" /> to <see cref="JObject" />
        /// in the shape of a Publications Index Entry.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="useJavaScriptCase">when <c>true</c> use “camel” casing.</param>
        /// <returns></returns>
        public static JObject ToPublicationIndexEntryJObject(this ISegment data, bool useJavaScriptCase)
        {
            var jSegment = data.ToJObject(useJavaScriptCase);

            var segment = data as Segment;
            if (segment == null) return jSegment;

            if (segment.Segments != null)
            {
                var jSegmentArray = new JArray(segment.Segments.Select(i => i.ToPublicationIndexEntryJObject(useJavaScriptCase)));

                jSegment[nameof(Segment.Segments).ToLowerInvariant()] = jSegmentArray;
            }

            if (segment.Documents != null)
            {
                var jDocumentArray = new JArray(segment.Documents.Select(i => i.ToJObject(useJavaScriptCase)));

                jSegment[nameof(Segment.Documents).ToLowerInvariant()] = jDocumentArray;
            }

            return jSegment;
        }

        /// <summary>
        /// Returns <see cref="ISegment"/> with default values.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static ISegment WithDefaults(this ISegment data)
        {
            data.SetDefaults();
            return data;
        }

        /// <summary>
        /// Returns <see cref="ISegment" />
        /// after the specified edit <see cref="Action{ISegment}" />.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="editAction">The edit action.</param>
        /// <returns></returns>
        public static ISegment WithEdit(this ISegment data, Action<ISegment> editAction)
        {
            editAction?.Invoke(data);
            return data;
        }
    }
}