namespace Songhay.Publications.Extensions;

/// <summary>
/// Extensions of <see cref="JsonElement"/>.
/// </summary>
public static partial class JsonElementExtensions
{
    /// <summary>
    /// Gets the publication command.
    /// </summary>
    /// <param name="element">The <see cref="JsonElement"/>.</param>
    public static string? GetPublicationCommand(this JsonElement element) => element.GetProperty("command").GetString();

    /// <summary>
    /// Converts the specified <see cref="JsonElement"/>
    /// to a boxed <see cref="object"/> of boxed objects.
    /// </summary>
    /// <param name="element">the <see cref="JsonElement"/></param>
    /// <remarks>
    /// This method helps the <c>YamlDotNet</c> <see cref="SerializerBuilder"/>
    /// serialize <see cref="JsonElement"/> to YAML
    /// without “shadow-converting” all property values to <see cref="string"/>.
    ///
    /// See <see cref="ToYaml"/>.
    /// </remarks>
    public static object? ToBoxedValue(this JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Array => element.EnumerateArray().Select(e => e.ToBoxedValue()),
            JsonValueKind.Object => element.EnumerateObject().Select(p => p.Value.ToBoxedValue()),
            JsonValueKind.False => element.GetBoolean(),
            JsonValueKind.True => element.GetBoolean(),
            JsonValueKind.Number => element.GetDouble(),
            JsonValueKind.String => element.GetString(),
            _ => null
        };
    }

    /// <summary>
    /// Converts the specified <see cref="JsonElement"/>
    /// to <c>TObject</c>.
    /// </summary>
    /// <param name="element">the specified <see cref="JsonElement"/></param>
    /// <typeparam name="TObject">the type to convert to</typeparam>
    /// <returns></returns>
    public static TObject? ToObject<TObject>(this JsonElement element)
    {
        var json = element.GetRawText();

        return JsonSerializer.Deserialize<TObject>(json);
    }

    /// <summary>
    /// Converts the specified <see cref="JsonElement"/>
    /// to a YAML <see cref="string"/>.
    /// </summary>
    /// <param name="element">the <see cref="JsonElement"/></param>
    public static string ToYaml(this JsonElement element)
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var dict = element
            .EnumerateObject()
            .ToDictionary(p => p.Name, p => p.Value.ToBoxedValue());

        string yaml = serializer.Serialize(dict);

        return yaml;
    }
}
