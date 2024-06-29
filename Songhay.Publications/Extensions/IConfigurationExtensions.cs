using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Songhay.Publications.Extensions;

/// <summary>
/// Extensions of <see cref="IConfiguration"/>
/// </summary>
// ReSharper disable once InconsistentNaming
public static class IConfigurationExtensions
{
    /// <summary>
    /// Converts <see cref="ProgramArgs"/> to Presentation <see cref="DirectoryInfo"/>.
    /// </summary>
    /// <param name="configuration">the <see cref="IConfiguration"/></param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    public static (DirectoryInfo presentationInfo, FileInfo settingsInfo) ToPresentationAndSettingsInfo(
        this IConfiguration? configuration, ILogger? logger)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        string? basePath = configuration.GetBasePathValue();

        if (!Directory.Exists(basePath))
            throw new DirectoryNotFoundException($"The expected base path, `{basePath}`, is not here.");

        logger?.LogInformation("Verifying conventional markdown {Name} directory...",
            MarkdownPresentationDirectories.DirectoryNamePresentationShell);

        DirectoryInfo presentationShellInfo = new(basePath);
        presentationShellInfo.VerifyDirectory(MarkdownPresentationDirectories.DirectoryNamePresentationShell);

        logger?.LogInformation("Verifying all conventional markdown directories...");

        DirectoryInfo presentationInfo = presentationShellInfo.Parent.ToReferenceTypeValueOrThrow();
        presentationInfo.HasAllConventionalMarkdownPresentationDirectories();

        logger?.LogInformation("Verifying settings file...");

        string settingsFilePath = configuration.GetSettingsFilePath();

        logger?.LogInformation("Found settings file `{Path}`. Returning...", settingsFilePath);

        FileInfo settingsInfo = File.Exists(settingsFilePath) ?
            new(settingsFilePath)
            :
            presentationShellInfo.FindFile(settingsFilePath).ToReferenceTypeValueOrThrow();

        return (presentationInfo, settingsInfo);
    }
}
