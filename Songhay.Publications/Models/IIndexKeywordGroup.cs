namespace Songhay.Publications.Models
{
    /// <summary>
    /// Publication Index Keyword Group
    /// </summary>
    public interface IIndexKeywordGroup : ITemporal
    {
        /// <summary>
        /// Index Keyword Group ID
        /// </summary>
        /// <value></value>
        int? Id { get; set; }

        /// <summary>
        /// Index Keyword Group Client ID
        /// </summary>
        /// <value></value>
        string ClientId { get; set; }

        /// <summary>
        /// Index Keyword Group Name
        /// </summary>
        /// <value></value>
        string Name { get; set; }
    }
}