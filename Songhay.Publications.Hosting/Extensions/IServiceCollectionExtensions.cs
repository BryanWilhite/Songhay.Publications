using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Songhay.Abstractions;
using Songhay.Extensions;
using Songhay.Publications.Activities;

namespace Songhay.Publications.Hosting.Extensions;

/// <summary>
/// Extensions of <see cref="IServiceCollection"/>
/// </summary>
// ReSharper disable once InconsistentNaming
public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="PublicationsHostedService"/>, configured by the specified Activity name.
    /// </summary>
    /// <param name="services">the <see cref="IServiceCollection"/></param>
    /// <param name="activityName">The name of the activity</param>
    public static void AddPublicationsHostedService(this IServiceCollection? services, string? activityName)
    {
        ArgumentNullException.ThrowIfNull(services);
        activityName.ThrowWhenNullOrWhiteSpace();

        switch (activityName)
        {
            case nameof(MarkdownEntryActivity):
                services.AddSingleton<IActivityTask, MarkdownEntryActivity>();
                break;

            case nameof(SearchIndexActivity):
                services.AddSingleton<IActivityTask, SearchIndexActivity>();
                break;
        }

        services.AddSingleton<IHostedService, PublicationsHostedService>();
    }
}
