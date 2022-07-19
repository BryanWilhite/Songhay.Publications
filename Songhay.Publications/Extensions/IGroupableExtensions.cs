namespace Songhay.Publications.Extensions;

/// <summary>
/// Extensions of <see cref="IGroupable"/>.
/// </summary>
public static class GroupableExtensions
{
    /// <summary>
    /// Returns <c>true</c> when the group has the specified name.
    /// </summary>
    /// <param name="group">The group.</param>
    /// <param name="name">The name of the <see cref="IGroupable.GroupId"/>.</param>
    public static bool IsGroup(this IGroupable? group, string? name)
    {
        if (group == null) return false;

        return group.GroupId?.ToLowerInvariant() == name?.ToLowerInvariant();
    }
}
