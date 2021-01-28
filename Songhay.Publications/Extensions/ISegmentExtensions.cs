using System;
using System.Linq;
using System.Text;
using CloneExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        /// Converts the <see cref="ISegment"/> into a display text.
        /// </summary>
        /// <param name="data">The data.</param>
        public static string ToDisplayText(this ISegment data)
        {
            if (data == null) return null;

            var builder = new StringBuilder($"{nameof(data.SegmentId)}: {data?.SegmentId}");
            builder.Append($", {nameof(data.SegmentName)}: {data?.SegmentName}");
            builder.Append($", {nameof(data.IsActive)}: {data?.IsActive}");
            builder.Append($", {nameof(data.ParentSegmentId)}: {data?.ParentSegmentId}");
            builder.Append($", {nameof(data.InceptDate)}: {data?.InceptDate}");

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

            var jO = JObject.FromObject(data, JsonSerializer.Create(settings));
            return jO;
        }

        /// <summary>
        /// Converts the <see cref="ISegment" /> to <see cref="JObject" />
        /// in the shape of a Publications Index.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="useJavaScriptCase">when <c>true</c> use “camel” casing.</param>
        /// <returns></returns>
        public static JObject ToPublicationIndexJObject(this ISegment data, bool useJavaScriptCase)
        {
            var jSegment = data.ToJObject(useJavaScriptCase);

            var segment = data as Segment;
            if (segment == null) return jSegment;

            if (segment.Segments != null)
            {
                var jSegmentArray = new JArray(segment.Segments.Select(i => i.ToPublicationIndexJObject(useJavaScriptCase)));

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
                Id = data.SegmentId,
                ItemName = data.SegmentName
            };
            return dataOut;
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