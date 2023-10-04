using System.Text.Json;

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
}
