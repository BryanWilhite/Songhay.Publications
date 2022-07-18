namespace Songhay.Publications.Models;

/// <summary>
/// Centralizes initializers for <see cref="CloneExtensions"/>
/// </summary>
public static class CloneInitializers
{
    static CloneInitializers()
    {
        Publications = new Dictionary<Type, Func<object, object>>
        {
            { typeof(ISegment), o => new Segment() },
            { typeof(IDocument), o => new Document() },
            { typeof(IFragment), o => new Fragment() },
            { typeof(IIndexKeyword), o => new IndexKeyword() },
        };
    }

    /// <summary>
    /// Gets initializers for Publications
    /// </summary>
    public static Dictionary<Type, Func<object, object>> Publications { get; }
}