﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;


namespace Songhay.Publications.Extensions;

/// <summary>
/// Extensions of <see cref="ISegment"/>
/// </summary>
public static class SegmentExtensions
{
    static SegmentExtensions() => TraceSource = TraceSources
        .Instance
        .GetTraceSourceFromConfiguredName()
        .WithSourceLevels();

    static readonly TraceSource? TraceSource;

    /// <summary>
    /// Clones the instance of <see cref="ISegment"/>.
    /// </summary>
    /// <param name="data">The document.</param>
    public static Segment? Clone(this ISegment? data) => data?.GetClone(CloneInitializers.Publications) as Segment;

    /// <summary>
    /// Returns and traces the first <see cref="ISegment"/>
    /// based on the specified predicate.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="predicate">The predicate.</param>
    public static ISegment? GetSegmentByPredicate(this IEnumerable<ISegment>? data, Func<ISegment, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(data);

        var first = data.FirstOrDefault(predicate);

        TraceSource?.TraceVerbose(first.ToDisplayText(showIdOnly: true));

        return first;
    }

    /// <summary>
    /// Returns <c>true</c> when the <see cref="ISegment"/>
    /// has any <see cref="Segment.Documents"/>.
    /// </summary>
    /// <param name="data"></param>
    public static bool HasDocuments(this ISegment? data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));

        if (data is not Segment segment) return false;

        if (segment.Documents.Any()) return true;

        TraceSource?.TraceError($"The expected child {nameof(Segment.Documents)} are not here.");

        return false;
    }

    /// <summary>
    /// Sets the defaults.
    /// </summary>
    /// <param name="data">The data.</param>
    public static void SetDefaults(this ISegment? data)
    {
        if (data == null) return;

        data.InceptDate = DateTime.Now;
        data.IsActive = true;
    }

    /// <summary>
    /// Converts the <see cref="ISegment"/> into human-readable display text.
    /// </summary>
    /// <param name="data">The data.</param>
    public static string ToDisplayText(this ISegment? data) => data.ToDisplayText(showIdOnly: false);

    /// <summary>
    /// Converts the <see cref="ISegment"/> into human-readable display text.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="showIdOnly">when <c>true</c> then display identifiers only</param>
    public static string ToDisplayText(this ISegment? data, bool showIdOnly)
    {
        if (data == null)
            return $"{nameof(ToDisplayText)}: the specified {nameof(ISegment)} is null.";

        var builder = new StringBuilder();

        var delimiter = string.Empty;

        if (data.SegmentId.HasValue)
        {
            builder.Append($"{nameof(data.SegmentId)}: {data.SegmentId}");
            delimiter = ", ";
        }

        if (!string.IsNullOrWhiteSpace(data.ClientId))
        {
            builder.Append($"{delimiter}{nameof(data.ClientId)}: {data.ClientId}");
            delimiter = ", ";
        }

        if (!showIdOnly)
        {
            if (!string.IsNullOrWhiteSpace(data.SegmentName))
                builder.Append($"{delimiter}{nameof(data.SegmentName)}: {data.SegmentName}");

            if (data.IsActive.HasValue)
                builder.Append($"{delimiter}{nameof(data.IsActive)}: {data.IsActive}");

            if (data.ParentSegmentId.HasValue)
                builder.Append($"{delimiter}{nameof(data.ParentSegmentId)}: {data.ParentSegmentId}");

            if (data.InceptDate.HasValue)
                builder.Append($"{delimiter}{nameof(data.InceptDate)}: {data.InceptDate}");
        }

        return builder.ToString();
    }

    /// <summary>
    /// Converts the <see cref="ISegment" /> to <see cref="JObject" />.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="useJavaScriptCase">when <c>true</c> use “camel” casing.</param>
    public static JObject? ToJObject(this ISegment? data, bool useJavaScriptCase)
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
    public static MenuDisplayItemModel? ToMenuDisplayItemModel(this ISegment? data) =>
        data.ToMenuDisplayItemModel(group: null);

    /// <summary>
    /// Converts the <see cref="ISegment"/> into a menu display item model.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="group">The group.</param>
    public static MenuDisplayItemModel? ToMenuDisplayItemModel(this ISegment? data, IGroupable? group)
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
    /// Converts the <see cref="IEnumerable{ISegment}" /> to <see cref="IEnumerable{IIndexEntry}" />.
    /// </summary>
    /// <param name="data">The data.</param>
    public static IEnumerable<IIndexEntry> ToPublicationIndexEntries(this IEnumerable<ISegment> data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return data.Select(i => i.ToPublicationIndexEntry());
    }

    /// <summary>
    /// Converts the <see cref="ISegment" /> to <see cref="IIndexEntry" />.
    /// </summary>
    /// <param name="data">The data.</param>
    public static IIndexEntry ToPublicationIndexEntry(this ISegment? data)
    {
        if(data == null) throw new ArgumentNullException(nameof(data));

        if (data is not Segment instance)
            throw new DataException($"The expected {nameof(Segment)} data is not here.");

        return new IndexEntry(instance)
        {
            Segments = instance
                .Segments
                .Select(s => s.ToPublicationIndexEntry())
                .ToArray(),
        };
    }

    /// <summary>
    /// Converts the <see cref="ISegment" /> to <see cref="JObject" />
    /// in the shape of a Publications Index Entry.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="useJavaScriptCase">when <c>true</c> use “camel” casing.</param>
    public static JObject ToPublicationIndexEntryJObject(this ISegment? data, bool useJavaScriptCase)
    {
        var jSegment = data.ToJObject(useJavaScriptCase).ToReferenceTypeValueOrThrow();

        if (data is not Segment segment) return jSegment;

        if (segment.Segments.Any())
        {
            var jSegmentArray = new JArray(segment.Segments.Select(i => i.ToPublicationIndexEntryJObject(useJavaScriptCase)));

            jSegment[nameof(Segment.Segments).ToLowerInvariant()] = jSegmentArray;
        }

        if (!segment.Documents.Any()) return jSegment;
        {
            var jDocumentArray = new JArray(segment.Documents.Select(i => i.ToJObject(useJavaScriptCase)));

            jSegment[nameof(Segment.Documents).ToLowerInvariant()] = jDocumentArray;
        }

        return jSegment;
    }

    /// <summary>
    /// Converts the <see cref="IDocument"/> data to <see cref="ValidationResult"/>.
    /// </summary>
    /// <param name="data">the <see cref="IDocument"/> data</param>
    public static ValidationResult ToValidationResult(this ISegment? data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (data is not Segment instance)
            throw new DataException($"The expected {nameof(Segment)} data is not here.");

        var validator = new SegmentValidator();

        return validator.Validate(instance);
    }

    /// <summary>
    /// Returns <see cref="ISegment"/> with default values.
    /// </summary>
    /// <param name="data">The data.</param>
    public static ISegment? WithDefaults(this ISegment? data)
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
    public static ISegment WithEdit(this ISegment? data, Action<ISegment>? editAction)
    {
        editAction?.Invoke(data.ToReferenceTypeValueOrThrow());

        return data.ToReferenceTypeValueOrThrow();
    }
}
