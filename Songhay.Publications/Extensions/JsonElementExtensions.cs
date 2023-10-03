using System.Text.Json;

// ReSharper disable once CheckNamespace
namespace Songhay.Extensions;

/// <summary>
/// Extensions of
/// </summary>
public static class JsonElementExtensions
{
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
}
