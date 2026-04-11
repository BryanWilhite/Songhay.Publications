using System.Text.Json.Nodes;
using Songhay.Publications.Abstractions;

namespace Songhay.Publications.Tests.Extensions;

public class FileInfoExtensionsTests(ITestOutputHelper helper)
{
    [Theory]
    [ProjectDirectoryData(@"test-files/yaml/hello-world-01.yaml", false)]
    [ProjectDirectoryData(@"test-files/yaml/hello-world-01-with-json-front-matter.yaml", true)]
    public void LookLikeJsonFrontMatter_Test(DirectoryInfo projectDirInfo, string path, bool expected)
    {
        ILogger logger = _loggerProvider.CreateLogger(nameof(LookLikeJsonFrontMatter_Test));

        FileInfo entry = new(projectDirInfo.ToCombinedPath(path));

        (IReadOnlyCollection<string> frontMatterLines, _) = entry.ToFrontMatterLinesAndContentLines(logger);

        bool actual = frontMatterLines.LookLikeJsonFrontMatter(logger);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [ProjectDirectoryData(@"test-files/yaml/hello-world-01.yaml", true)]
    [ProjectDirectoryData(@"test-files/yaml/hello-world-01-with-json-front-matter.yaml", false)]
    public void LookLikeYamlFrontMatter_Test(DirectoryInfo projectDirInfo, string path, bool expected)
    {
        ILogger logger = _loggerProvider.CreateLogger(nameof(LookLikeYamlFrontMatter_Test));

        FileInfo entry = new(projectDirInfo.ToCombinedPath(path));

        (IReadOnlyCollection<string> frontMatterLines, _) = entry.ToFrontMatterLinesAndContentLines(logger);

        bool actual = frontMatterLines.LookLikeYamlFrontMatter(logger);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [ProjectDirectoryData(@"test-files/yaml/hello-world-01.yaml", 3, 3)]
    [ProjectDirectoryData(@"test-files/yaml/hello-world-02.yaml", 3, 0)]
    [ProjectDirectoryData(@"test-files/yaml/hello-world-03.yaml", 3, 0)]
    public void ToFrontMatterLinesAndContentLines_Test(DirectoryInfo projectDirInfo, string path, int expectedFrontMatterLineCount, int expectedContentLineCount)
    {
        ILogger logger = _loggerProvider.CreateLogger(nameof(LookLikeYamlFrontMatter_Test));

        FileInfo entry = new(projectDirInfo.ToCombinedPath(path));

        (IReadOnlyCollection<string> FrontMatterLines, IReadOnlyCollection<string> ContentLines) actual = entry.ToFrontMatterLinesAndContentLines(logger);

        if(expectedFrontMatterLineCount > 0)
        {
            Assert.NotEmpty(actual.FrontMatterLines);
            Assert.Equal(expectedFrontMatterLineCount, actual.FrontMatterLines.Count);
        }
        else Assert.Empty(actual.FrontMatterLines);

        if(expectedContentLineCount > 0)
        {
            Assert.NotEmpty(actual.ContentLines);
            Assert.Equal(expectedContentLineCount, actual.ContentLines.Count);
        }
        else Assert.Empty(actual.ContentLines);
    }

    [Theory]
    [ProjectDirectoryData("test-files/entry/hello-world-json.md")]
    public void ToIDocumentAndAnyContent_Test(DirectoryInfo projectDirInfo, string entryPath)
    {
        ILogger logger = _loggerProvider.CreateLogger(nameof(WriteNewPublicationEntryWithJsonFrontMatter_Test));

        FileInfo actual = new(projectDirInfo.ToCombinedPath(entryPath));

        (IDocument? frontMatter, string? content) = actual.ToIDocumentAndAnyContent(logger);
        logger.LogInformation("{Label}: {Value}", nameof(frontMatter), frontMatter?.ToString());
        logger.LogInformation("{Label}: {Value}", nameof(content), content);
    }

    [Theory]
    [ProjectDirectoryData(@"test-files/yaml/hello-world-01-with-json-front-matter.yaml")]
    public void ToJsonString_Test(DirectoryInfo projectDirInfo, string path)
    {
        ILogger logger = _loggerProvider.CreateLogger(nameof(ToJsonString_Test));

        FileInfo entry = new(projectDirInfo.ToCombinedPath(path));
        string? actual = entry.ToFrontMatterLinesAndContentLines(logger).FrontMatterLines.ToJsonString(logger);

        JsonNode? node = JsonNode.Parse(actual!);
        logger.LogInformation(node!.ToJsonString());
    }

    [Theory]
    [ProjectDirectoryData(@"test-files/yaml/hello-world-01.yaml")]
    [ProjectDirectoryData(@"test-files/yaml/hello-world-01-not-inline.yaml")]
    public void ToYamlString_Test(DirectoryInfo projectDirInfo, string path)
    {
        ILogger logger = _loggerProvider.CreateLogger(nameof(ToYamlString_Test));

        FileInfo entry = new(projectDirInfo.ToCombinedPath(path));
        string? actual = entry.ToFrontMatterLinesAndContentLines(logger).FrontMatterLines.ToYamlString(logger);

        logger.LogInformation("Attempting to parse:{NL}{Yaml}", Environment.NewLine, actual);
        IDictionary<string, object>? yO = YamlUtility.DeserializeYaml(actual);
        logger.LogInformation(yO.ToJsonString());
    }

    [Theory]
    [ProjectDirectoryData("Hello World!", "test-files/entry/hello-world-json.md", null!)]
    public void WriteNewPublicationEntryWithJsonFrontMatter_Test(DirectoryInfo projectDirInfo, string title, string entryPath, string? content)
    {
        ILogger logger = _loggerProvider.CreateLogger(nameof(WriteNewPublicationEntryWithJsonFrontMatter_Test));

        FileInfo actual = new(projectDirInfo.ToCombinedPath(entryPath));

        actual.WriteNewPublicationEntryWithJsonFrontMatter(title,"./entries/", content, logger);

        Assert.True(actual.Exists);
    }

    [Theory]
    [ProjectDirectoryData("Hello World!", "test-files/entry/hello-world.md", null!)]
    public void WriteNewPublicationEntryWithYamlFrontMatter_Test(DirectoryInfo projectDirInfo, string title, string entryPath, string? content)
    {
        ILogger logger = _loggerProvider.CreateLogger(nameof(WriteNewPublicationEntryWithJsonFrontMatter_Test));

        FileInfo actual = new(projectDirInfo.ToCombinedPath(entryPath));

        actual.WriteNewPublicationEntryWithYamlFrontMatter(title,"./", content, logger);

        Assert.True(actual.Exists);
    }

    private readonly XUnitLoggerProvider _loggerProvider = new(helper);
}