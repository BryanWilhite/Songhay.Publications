namespace Songhay.Publications.Models
{
    /// <summary>
    /// Defines the Publication Index
    /// </summary>
    public interface IIndexEntry : ISegment
    {
        /// <summary>
        /// Child Index Entries
        /// </summary>
        IIndexEntry[] Segments { get; set; }

        /// <summary>
        /// Index Child Documents
        /// </summary>
        IDocument[] Documents { get; set; }
    }
}
