namespace Songhay.Publications.Extensions;

/// <summary>
/// Extensions of <see cref="JsonNode"/>
/// </summary>
public static class JsonNodeExtensions
{
    /// <summary>
    /// Copies the specified <see cref="JsonNode"/>
    /// by calling <see cref="JsonNode.ToJsonString"/>
    /// </summary>
    /// <param name="node">The node.</param>
    /// <remarks>
    /// This method is not efficient. For details, see https://stackoverflow.com/a/71000222/22944.
    /// </remarks>
    public static JsonNode? CopyNode(this JsonNode? node) //TODO: move to SonghayCore
    {
        JsonValueKind kind = node.GetJsonValueKind();

        string defaultJson = kind switch
        {
            JsonValueKind.Array => "[]",
            JsonValueKind.Object => "{}",
            _ => string.Empty
        };

        return JsonNode.Parse(node?.ToJsonString() ?? defaultJson);
    }

    /// <summary>
    /// Returns <c>false</c> when the specified property name does not exist.
    /// </summary>
    /// <param name="node">the <see cref="JsonNode"/></param>
    /// <param name="propertyName">the property name</param>
    /// <returns></returns>
    public static bool HasProperty(this JsonNode? node, string? propertyName)
    {
        if (node == null) return false;
        if (string.IsNullOrWhiteSpace(propertyName)) return false;

        return node[propertyName] != null;
    }

    /// <summary>
    /// Converts the specified <see cref="object"/>
    /// to a <see cref="JsonNode"/>.
    /// </summary>
    /// <param name="data">any <see cref="object"/></param>
    /// <returns></returns>
    public static JsonNode? ToJsonNode(this object? data)
    {
        if (data == null) return JsonNode.Parse("{}");

        var json = JsonSerializer.Serialize(data);

        return JsonNode.Parse(json);
    }
}
