namespace Songhay.Publications.Models
{
    /// <summary>
    /// Defines a Publication Index Entry
    /// </summary>
    public interface IIndexEntry : ISegment
    {
        /// <summary>
        /// Index Segments
        /// </summary>
        /// <value></value>
        IIndexEntry[] Segments { get; set; }

        /// <summary>
        /// Index Documents
        /// </summary>
        /// <value></value>
        IDocument[] Documents { get; set; }
    }
}
