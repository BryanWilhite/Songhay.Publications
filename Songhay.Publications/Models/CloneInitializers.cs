namespace Songhay.Publications.Models;

/// <summary>
/// Centralizes initializers for <see cref="CloneExtensions"/>.
/// </summary>
public static class CloneInitializers
{
    static CloneInitializers()
    {
        Publications = new Dictionary<Type, Func<object, object>>
        {
            { typeof(ISegment), _ => new Segment() },
            { typeof(IDocument), _ => new Document() },
            { typeof(IFragment), _ => new Fragment() },
            { typeof(IIndexKeyword), _ => new IndexKeyword() }
        };
    }

    /// <summary>
    /// Gets initializers for Publications.
    /// </summary>
    public static Dictionary<Type, Func<object, object>> Publications { get; }
}
