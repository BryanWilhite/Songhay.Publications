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
        if (data == null) throw new ArgumentNullException(nameof(data));

        var options = new System.Text.Json.JsonSerializerOptions
        {
            IgnoreNullValues = true,
            WriteIndented = true,
        };

        var json = System.Text.Json.JsonSerializer.Serialize(data, options);

        return json;
    }
}