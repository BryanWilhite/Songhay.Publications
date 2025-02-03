namespace Songhay.Publications;

/// <summary>
/// Shared routines for `YamlDotNet`.
/// </summary>
public static class YamlUtility
{
    /// <summary>
    /// Returns the <see cref="IDictionary{TKey,TValue}"/>
    /// from the specified YAML <see cref="string"/>.
    /// </summary>
    /// <param name="yaml">the YAML <see cref="string"/></param>
    /// <returns>
    /// Returns <see cref="IDictionary{TKey,TValue}"/>
    /// where <c>TKey</c> and <c>TValue</c> are converted from <see cref="object"/>
    /// to <see cref="string"/>
    /// </returns>
    public static IDictionary<string, object>? DeserializeYaml(string? yaml)
    {
        if (string.IsNullOrWhiteSpace(yaml)) return new Dictionary<string, object>();

        IDeserializer deserializer = new DeserializerBuilder().Build();
        IDictionary<object, object>? yO = deserializer.Deserialize(yaml) as IDictionary<object, object>;

        return yO?.ToHashSet().ToDictionary(kv => $"{kv.Key}", kv => kv.Value);
    }
}
