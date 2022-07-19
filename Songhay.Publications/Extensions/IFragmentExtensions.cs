using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Songhay.Publications.Extensions;

/// <summary>
/// Extensions of <see cref="IFragment"/>
/// </summary>
public static class FragmentExtensions
{
    static FragmentExtensions() => TraceSource = TraceSources
        .Instance
        .GetTraceSourceFromConfiguredName()
        .WithSourceLevels();

    static readonly TraceSource? TraceSource;

    /// <summary>
    /// Clones the instance of <see cref="IFragment"/>.
    /// </summary>
    /// <param name="data">The document.</param>
    public static Fragment? Clone(this IFragment? data) => data?.GetClone(CloneInitializers.Publications) as Fragment;

    /// <summary>
    /// Returns and traces the first <see cref="IFragment"/>
    /// based on the specified predicate.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="predicate">The predicate.</param>
    public static IFragment? GetFragmentByPredicate(this IEnumerable<IFragment>? data, Func<IFragment, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(data);

        var first = data.FirstOrDefault(predicate);

        TraceSource?.TraceVerbose($"{first?.ToDisplayText(showIdOnly: true)}");

        return first;
    }

    /// <summary>
    /// Sets the defaults.
    /// </summary>
    /// <param name="data">The fragment.</param>
    public static void SetDefaults(this IFragment? data)
    {
        if (data == null) return;

        data.InceptDate = DateTime.Now;
        data.IsActive = true;
        data.ModificationDate = DateTime.Now;
    }

    /// <summary>
    /// Converts the <see cref="IFragment"/> into human-readable display text.
    /// </summary>
    /// <param name="data">The data.</param>
    public static string ToDisplayText(this IFragment? data) => data.ToDisplayText(showIdOnly: false);

    /// <summary>
    /// Converts the <see cref="IFragment"/> into a display text.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="showIdOnly">when <c>true</c> then display identifiers only</param>
    public static string ToDisplayText(this IFragment? data, bool showIdOnly)
    {
        if (data == null)
            return $"{nameof(ToDisplayText)}: the specified {nameof(IFragment)} is null.";

        var builder = new StringBuilder();

        var delimiter = string.Empty;

        if (data.FragmentId.HasValue)
        {
            builder.Append($"{nameof(data.FragmentId)}: {data.FragmentId}");
            delimiter = ", ";
        }

        if (!string.IsNullOrWhiteSpace(data.ClientId))
        {
            builder.Append($"{delimiter}{nameof(data.ClientId)}: {data.ClientId}");
            delimiter = ", ";
        }

        if (showIdOnly) return builder.ToString();

        if (!string.IsNullOrWhiteSpace(data.FragmentName))
            builder.Append($"{delimiter}{nameof(data.FragmentName)}: {data.FragmentName}");

        if (data.IsActive.HasValue)
            builder.Append($"{delimiter}{nameof(data.IsActive)}: {data.IsActive}");

        if (data.DocumentId.HasValue)
            builder.Append($"{delimiter}{nameof(data.DocumentId)}: {data.DocumentId}");

        if (!string.IsNullOrEmpty(data.FragmentDisplayName))
            builder.Append($"{delimiter}{nameof(data.FragmentDisplayName)}: {data.FragmentDisplayName}");

        if (!string.IsNullOrEmpty(data.Content))
            builder.Append($"{delimiter}{nameof(data.Content)}: {data.Content.Truncate(32)}");

        if (data.PrevFragmentId.HasValue)
            builder.Append($"{delimiter}{nameof(data.PrevFragmentId)}: {data.PrevFragmentId}");

        if (data.NextFragmentId.HasValue)
            builder.Append($"{delimiter}{nameof(data.NextFragmentId)}: {data.NextFragmentId}");

        if (data.IsNext.HasValue)
            builder.Append($"{delimiter}{nameof(data.IsNext)}: {data.IsNext}");

        if (data.IsPrevious.HasValue)
            builder.Append($"{delimiter}{nameof(data.IsPrevious)}: {data.IsPrevious}");

        if (data.IsWrapper.HasValue)
            builder.Append($"{delimiter}{nameof(data.IsWrapper)}: {data.IsWrapper}");

        return builder.ToString();
    }

    /// <summary>
    /// Converts the <see cref="IFragment" /> to <see cref="JObject" />.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="useJavaScriptCase">when <c>true</c> use “camel” casing.</param>
    public static JObject? ToJObject(this IFragment? data, bool useJavaScriptCase)
    {
        if (data == null) return null;

        var settings = JsonSerializationUtility
            .GetConventionalResolver<IFragment>(useJavaScriptCase)
            .ToJsonSerializerSettings();

        //TODO: consider making these optional:
        settings.MissingMemberHandling = MissingMemberHandling.Ignore;
        settings.NullValueHandling = NullValueHandling.Ignore;

        var jO = JObject.FromObject(data, JsonSerializer.Create(settings));

        return jO;
    }

    /// <summary>
    /// Converts the <see cref="IFragment"/> into a menu display item model.
    /// </summary>
    /// <param name="data">The fragment.</param>
    public static MenuDisplayItemModel? ToMenuDisplayItemModel(this IFragment? data) =>
        data.ToMenuDisplayItemModel(group: null, copyFragmentContent: false);

    /// <summary>
    /// Converts the <see cref="IFragment" /> into a menu display item model.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="copyFragmentContent">if set to <c>true</c> include <see cref="IFragment" /> content.</param>
    /// <returns></returns>
    public static MenuDisplayItemModel? ToMenuDisplayItemModel(this IFragment? data, bool copyFragmentContent)
    {
        return data.ToMenuDisplayItemModel(group: null, copyFragmentContent: copyFragmentContent);
    }

    /// <summary>
    /// Converts the <see cref="IFragment"/> into a menu display item model.
    /// </summary>
    /// <param name="data">The fragment.</param>
    /// <param name="group">The group.</param>
    public static MenuDisplayItemModel? ToMenuDisplayItemModel(this IFragment? data, IGroupable? group) =>
        data.ToMenuDisplayItemModel(group, copyFragmentContent: false);

    /// <summary>
    /// Converts the <see cref="IFragment"/> into a menu display item model.
    /// </summary>
    /// <param name="data">The document.</param>
    /// <param name="group">The group.</param>
    /// <param name="copyFragmentContent">if set to <c>true</c> include <see cref="IFragment"/> content.</param>
    public static MenuDisplayItemModel? ToMenuDisplayItemModel(this IFragment? data, IGroupable? group,
        bool copyFragmentContent)
    {
        if (data == null) return null;

        var @namespace = typeof(PublicationContext).Namespace;

        var dataOut = new MenuDisplayItemModel()
        {
            DisplayText = data.FragmentDisplayName,
            GroupDisplayText = group == null ? $"{@namespace}.{nameof(Fragment)}" : group.GroupDisplayText,
            GroupId = group == null ? $"{@namespace}.{nameof(Fragment)}".ToLowerInvariant() : group.GroupId,
            Id = data.FragmentId.GetValueOrDefault(),
            ItemName = data.FragmentName
        };
        if (copyFragmentContent) dataOut.Description = data.Content;

        return dataOut;
    }

    /// <summary>
    /// Converts the <see cref="IDocument"/> data to <see cref="ValidationResult"/>.
    /// </summary>
    /// <param name="data">the <see cref="IDocument"/> data</param>
    public static ValidationResult ToValidationResult(this IFragment? data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (data is not Fragment instance)
            throw new DataException($"The expected {nameof(Fragment)} data is not here.");

        var validator = new FragmentValidator();

        return validator.Validate(instance);
    }

    /// <summary>
    /// Returns <see cref="IFragment"/> with default values.
    /// </summary>
    /// <param name="data">The data.</param>
    public static IFragment? WithDefaults(this IFragment? data)
    {
        data.SetDefaults();

        return data;
    }

    /// <summary>
    /// Returns <see cref="IFragment" />
    /// after the specified edit <see cref="Action{IFragment}" />.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="editAction">The edit action.</param>
    public static IFragment WithEdit(this IFragment? data, Action<IFragment>? editAction)
    {
        editAction?.Invoke(data.ToReferenceTypeValueOrThrow());

        return data.ToReferenceTypeValueOrThrow();
    }
}
