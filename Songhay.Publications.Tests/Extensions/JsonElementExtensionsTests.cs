using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Songhay.Models;
using Songhay.Publications.Activities;

namespace Songhay.Publications.Tests.Extensions;

public class JsonElementExtensionsTests(ITestOutputHelper helper)
{
    [Theory]
    [InlineData("md-add-entry-extract-settings.json", "../../../markdown/shell")]
    public void GetAddEntryExtractArg_Test(string settingsFile, string presentationRoot)
    {
        ILogger logger = _loggerProvider.CreateLogger(nameof(GetAddEntryExtractArg_Test));

        presentationRoot = ProgramAssemblyUtility.GetPathFromAssembly(GetType().Assembly, presentationRoot);

        string[] args =
        [
            ConsoleArgsScalars.BaseDirectoryRequired, ConsoleArgsScalars.FlagSpacer,
            ConsoleArgsScalars.SettingsFile, settingsFile,
            ConsoleArgsScalars.BaseDirectory, presentationRoot
        ];

        IConfiguration? configuration = null;
        IHostBuilder builder = Host.CreateDefaultBuilder(args);

        builder.ConfigureHostConfiguration(b => b.AddCommandLine(["--ENVIRONMENT", Environments.Development]));
        builder.ConfigureAppConfiguration((hostingContext, _) => configuration = hostingContext.Configuration);
        using IHost _ = builder.Build();

        Assert.NotNull(configuration);

        (DirectoryInfo presentationInfo, FileInfo settingsInfo) = configuration.ToPresentationAndSettingsInfo(logger);

        Assert.True(presentationInfo.Exists);
        Assert.True(settingsInfo.Exists);

        using var jDoc = JsonDocument.Parse(File.ReadAllText(settingsInfo.FullName)).ToReferenceTypeValueOrThrow();
        JsonElement jO = jDoc.RootElement;

        string entryPath = jO.GetAddEntryExtractArg(presentationInfo);
        Assert.True(File.Exists(entryPath));

        string? commandName = jO.GetPublicationCommand();
        Assert.Equal(nameof(MarkdownEntryActivity.AddEntryExtract), commandName);
    }

    [Theory]
    [InlineData("index-activity-settings.json", "../../../markdown/shell")]
    public void GetCompressed11tyIndexArgs_Test(string settingsFile, string presentationRoot)
    {
        ILogger logger = _loggerProvider.CreateLogger(nameof(GetAddEntryExtractArg_Test));

        presentationRoot = ProgramAssemblyUtility.GetPathFromAssembly(GetType().Assembly, presentationRoot);

        string[] args =
        [
            ConsoleArgsScalars.SettingsFile, settingsFile,
            ConsoleArgsScalars.BaseDirectory, presentationRoot
        ];

        IConfiguration? configuration = null;
        IHostBuilder builder = Host.CreateDefaultBuilder(args);

        builder.ConfigureHostConfiguration(b => b.AddCommandLine(["--ENVIRONMENT", Environments.Development]));
        builder.ConfigureAppConfiguration((hostingContext, _) => configuration = hostingContext.Configuration);
        using IHost _ = builder.Build();

        Assert.NotNull(configuration);

        (DirectoryInfo presentationInfo, FileInfo settingsInfo) = configuration.ToPresentationAndSettingsInfo(logger);

        Assert.True(presentationInfo.Exists);
        Assert.True(settingsInfo.Exists);

        using var jDoc = JsonDocument.Parse(File.ReadAllText(settingsInfo.FullName)).ToReferenceTypeValueOrThrow();
        JsonElement jO = jDoc.RootElement;
        (DirectoryInfo entryRootInfo, DirectoryInfo indexRootInfo, string indexFileName) = jO.GetCompressed11TyIndexArgs(presentationInfo);

        Assert.True(entryRootInfo.Exists);
        Assert.True(indexRootInfo.Exists);
        Assert.False(string.IsNullOrWhiteSpace(indexFileName));

        indexRootInfo.FindFile(indexFileName);

        string? commandName = jO.GetPublicationCommand();
        Assert.Equal(nameof(SearchIndexActivity.GenerateCompressed11TySearchIndex), commandName);
    }

    [Theory]
    [InlineData("md-expand-uris-settings.json", "../../../markdown/shell")]
    public void GetExpandUrisArgs_Test(string settingsFile, string presentationRoot)
    {
        ILogger logger = _loggerProvider.CreateLogger(nameof(GetAddEntryExtractArg_Test));

        presentationRoot = ProgramAssemblyUtility.GetPathFromAssembly(GetType().Assembly, presentationRoot);

        string[] args =
        [
            ConsoleArgsScalars.SettingsFile, settingsFile,
            ConsoleArgsScalars.BaseDirectory, presentationRoot
        ];

        IConfiguration? configuration = null;
        IHostBuilder builder = Host.CreateDefaultBuilder(args);

        builder.ConfigureHostConfiguration(b => b.AddCommandLine(["--ENVIRONMENT", Environments.Development]));
        builder.ConfigureAppConfiguration((hostingContext, _) => configuration = hostingContext.Configuration);
        using IHost _ = builder.Build();

        Assert.NotNull(configuration);

        (DirectoryInfo presentationInfo, FileInfo settingsInfo) = configuration.ToPresentationAndSettingsInfo(logger);

        Assert.True(presentationInfo.Exists);
        Assert.True(settingsInfo.Exists);

        using var jDoc = JsonDocument.Parse(File.ReadAllText(settingsInfo.FullName)).ToReferenceTypeValueOrThrow();
        JsonElement jO = jDoc.RootElement;

        (string entryPath, string collapsedHost) = jO.GetExpandUrisArgs(presentationInfo);
        Assert.True(File.Exists(entryPath));
        Assert.False(string.IsNullOrEmpty(collapsedHost));

        string? commandName = jO.GetPublicationCommand();
        Assert.Equal(nameof(MarkdownEntryActivity.ExpandUris), commandName);
    }

    [Theory]
    [InlineData("md-find-change-settings.json", "../../../markdown/shell")]
    public void GetFindChangeArgs_Test(string settingsFile, string presentationRoot)
    {
        ILogger logger = _loggerProvider.CreateLogger(nameof(GetAddEntryExtractArg_Test));

        presentationRoot = ProgramAssemblyUtility.GetPathFromAssembly(GetType().Assembly, presentationRoot);

        string[] args =
        [
            ConsoleArgsScalars.SettingsFile, settingsFile,
            ConsoleArgsScalars.BaseDirectory, presentationRoot
        ];

        IConfiguration? configuration = null;
        IHostBuilder builder = Host.CreateDefaultBuilder(args);

        builder.ConfigureHostConfiguration(b => b.AddCommandLine(["--ENVIRONMENT", Environments.Development]));
        builder.ConfigureAppConfiguration((hostingContext, _) => configuration = hostingContext.Configuration);
        using IHost _ = builder.Build();

        Assert.NotNull(configuration);

        (DirectoryInfo presentationInfo, FileInfo settingsInfo) = configuration.ToPresentationAndSettingsInfo(logger);

        Assert.True(presentationInfo.Exists);
        Assert.True(settingsInfo.Exists);

        using var jDoc = JsonDocument.Parse(File.ReadAllText(settingsInfo.FullName)).ToReferenceTypeValueOrThrow();
        JsonElement jO = jDoc.RootElement;
        (string input, string pattern, string replacement, bool useRegex, string outputPath) = jO.GetFindChangeArgs(presentationInfo);

        Assert.False(string.IsNullOrEmpty(input));
        Assert.False(string.IsNullOrEmpty(pattern));
        Assert.False(string.IsNullOrEmpty(replacement));
        Assert.True(useRegex);
        Assert.False(string.IsNullOrEmpty(outputPath));

        string? inputPath = jO.GetProperty("inputPath").GetString();

        helper.WriteLine($"{nameof(inputPath)}: {inputPath}");
        helper.WriteLine($"{nameof(input)}: {input.Substring(0, 16)}...");
        helper.WriteLine($"{nameof(pattern)}: {pattern}");
        helper.WriteLine($"{nameof(replacement)}: {replacement}");
        helper.WriteLine($"{nameof(useRegex)}: {useRegex}");
        helper.WriteLine($"{nameof(outputPath)}: {outputPath}");
    }

    [Theory]
    [InlineData("md-generate-entry-settings.json", "../../../markdown/shell")]
    public void GetGenerateEntryArgs_Test(string settingsFile, string presentationRoot)
    {
        ILogger logger = _loggerProvider.CreateLogger(nameof(GetAddEntryExtractArg_Test));

        presentationRoot = ProgramAssemblyUtility.GetPathFromAssembly(GetType().Assembly, presentationRoot);

        string[] args =
        [
            ConsoleArgsScalars.SettingsFile, settingsFile,
            ConsoleArgsScalars.BaseDirectory, presentationRoot
        ];

        IConfiguration? configuration = null;
        IHostBuilder builder = Host.CreateDefaultBuilder(args);

        builder.ConfigureHostConfiguration(b => b.AddCommandLine(["--ENVIRONMENT", Environments.Development]));
        builder.ConfigureAppConfiguration((hostingContext, _) => configuration = hostingContext.Configuration);
        using IHost _ = builder.Build();

        Assert.NotNull(configuration);

        (DirectoryInfo presentationInfo, FileInfo settingsInfo) = configuration.ToPresentationAndSettingsInfo(logger);

        Assert.True(presentationInfo.Exists);
        Assert.True(settingsInfo.Exists);

        using var jDoc = JsonDocument.Parse(File.ReadAllText(settingsInfo.FullName)).ToReferenceTypeValueOrThrow();
        JsonElement jO = jDoc.RootElement;

        (DirectoryInfo entryDraftsRootInfo, string title) = jO.GetGenerateEntryArgs(presentationInfo);
        Assert.True(entryDraftsRootInfo.Exists);
        Assert.False(string.IsNullOrEmpty(title));

        string? commandName = jO.GetPublicationCommand();
        Assert.Equal(nameof(MarkdownEntryActivity.GenerateEntry), commandName);
    }

    [Theory]
    [InlineData("md-publish-entry-settings.json", "../../../markdown/shell")]
    public void GetPublishEntryArgs_Test(string settingsFile, string presentationRoot)
    {
        ILogger logger = _loggerProvider.CreateLogger(nameof(GetAddEntryExtractArg_Test));

        presentationRoot = ProgramAssemblyUtility.GetPathFromAssembly(GetType().Assembly, presentationRoot);

        string[] args =
        [
            ConsoleArgsScalars.SettingsFile, settingsFile,
            ConsoleArgsScalars.BaseDirectory, presentationRoot
        ];

        IConfiguration? configuration = null;
        IHostBuilder builder = Host.CreateDefaultBuilder(args);

        builder.ConfigureHostConfiguration(b => b.AddCommandLine(["--ENVIRONMENT", Environments.Development]));
        builder.ConfigureAppConfiguration((hostingContext, _) => configuration = hostingContext.Configuration);
        using IHost _ = builder.Build();

        Assert.NotNull(configuration);

        (DirectoryInfo presentationInfo, FileInfo settingsInfo) = configuration.ToPresentationAndSettingsInfo(logger);

        Assert.True(presentationInfo.Exists);
        Assert.True(settingsInfo.Exists);

        using var jDoc = JsonDocument.Parse(File.ReadAllText(settingsInfo.FullName)).ToReferenceTypeValueOrThrow();
        JsonElement jO = jDoc.RootElement;

        (DirectoryInfo entryDraftsRootInfo, DirectoryInfo entryRootInfo, string entryFileName) =
            jO.GetPublishEntryArgs(presentationInfo);
        Assert.True(entryDraftsRootInfo.Exists);
        Assert.True(entryRootInfo.Exists);
        Assert.False(string.IsNullOrEmpty(entryFileName));

        string? commandName = jO.GetPublicationCommand();
        Assert.Equal(nameof(MarkdownEntryActivity.PublishEntry), commandName);
    }

    [Theory]
    [InlineData("""

                {
                    "myString": "a scalar",
                    "myBoolean": true,
                    "myDate": "2005-12-30T23:16:54.0000000",
                    "sequence": [ "one", "two" ],
                    "myObject": { "myNumber": 42, "isUp": true, "nextNumbers": [ 43, 44 ] }
                }

                """)]
    public void ToYaml_Test(string json)
    {
        using var jDoc = JsonDocument.Parse(json);
        JsonElement jE = jDoc.RootElement;

        string yaml = jE.ToYaml();

        helper.WriteLine(yaml);
    }

    private readonly XUnitLoggerProvider _loggerProvider = new(helper);
}