namespace Songhay.Publications.Activities;

/// <summary>
/// Implementation of <see cref="ActivitiesGetter"/>
/// for Songhay Publications.
/// </summary>
/// <seealso cref="ActivitiesGetter" />
public sealed class PublicationsActivitiesGetter : ActivitiesGetter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PublicationsActivitiesGetter"/> class.
    /// </summary>
    /// <param name="args">The arguments.</param>
    public PublicationsActivitiesGetter(string[] args) : base(args)
    {
        this.LoadActivities(new Dictionary<string, Lazy<IActivity>>
        {
            {
                nameof(SearchIndexActivity),
                new Lazy<IActivity>(() => new SearchIndexActivity())
            },
            {
                nameof(MarkdownEntryActivity),
                new Lazy<IActivity>(() => new MarkdownEntryActivity())
            },
        });
    }
}