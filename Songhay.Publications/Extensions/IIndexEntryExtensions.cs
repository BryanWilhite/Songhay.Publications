using System.Text.Json.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Songhay.Publications.Extensions;

/// <summary>
/// Extensions of <see cref="IIndexEntry"/>.
/// </summary>
public static class IndexEntryExtensions
{
    /// <summary>
    /// Converts the <see cref="IEnumerable{IIndexEntry}"/>
    /// to a JSON <see cref="string"/>
    /// with conventional <see cref="System.Text.Json.JsonSerializerOptions"/>.
    /// </summary>
    /// <param name="data">The index data.</param>
    public static string ToJson(this IEnumerable<IIndexEntry> data)
    {
        ArgumentNullException.ThrowIfNull(data);

        JsonSerializerOptions options = new(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
        };

        var json = JsonSerializer.Serialize(data, options);

        return json;
    }
}
