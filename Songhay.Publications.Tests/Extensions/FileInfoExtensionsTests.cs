using System.Text.Json.Nodes;

namespace Songhay.Publications.Tests.Extensions;

public class FileInfoExtensionsTests
{
    static FileInfoExtensionsTests()
    {
        var type = typeof(FileInfoExtensionsTests);
        var projectDirectory = ProgramAssemblyUtility.GetPathFromAssembly(type.Assembly, "../../../");

        ProjectDirInfo = new DirectoryInfo(projectDirectory);
    }

    public FileInfoExtensionsTests(ITestOutputHelper helper)
    {
        _loggerProvider = new XUnitLoggerProvider(helper);
    }

    [Theory]
    [InlineData(@"yaml/hello-world-01.yaml", false)]
    [InlineData(@"yaml/hello-world-01-with-json-front-matter.yaml", true)]
    public void LookLikeJsonFrontMatter_Test(string path, bool expected)
    {
        ILogger logger = _loggerProvider.CreateLogger(nameof(LookLikeJsonFrontMatter_Test));

        var entry = new FileInfo(ProjectDirInfo.ToCombinedPath(path));

        var (frontMatterLines, _) = entry.ToFrontMatterLinesAndContentLines(logger);

        var actual = frontMatterLines.LookLikeJsonFrontMatter(logger);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(@"yaml/hello-world-01.yaml", true)]
    [InlineData(@"yaml/hello-world-01-with-json-front-matter.yaml", false)]
    public void LookLikeYamlFrontMatter_Test(string path, bool expected)
    {
        ILogger logger = _loggerProvider.CreateLogger(nameof(LookLikeYamlFrontMatter_Test));

        var entry = new FileInfo(ProjectDirInfo.ToCombinedPath(path));

        var (frontMatterLines, _) = entry.ToFrontMatterLinesAndContentLines(logger);

        var actual = frontMatterLines.LookLikeYamlFrontMatter(logger);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(@"yaml/hello-world-01.yaml", 3, 3)]
    [InlineData(@"yaml/hello-world-02.yaml", 3, 0)]
    [InlineData(@"yaml/hello-world-03.yaml", 3, 0)]
    public void ToFrontMatterLinesAndContentLines_Test(string path, int expectedFrontMatterLineCount, int expectedContentLineCount)
    {
        ILogger logger = _loggerProvider.CreateLogger(nameof(LookLikeYamlFrontMatter_Test));

        var entry = new FileInfo(ProjectDirInfo.ToCombinedPath(path));

        var actual = entry.ToFrontMatterLinesAndContentLines(logger);

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
    [InlineData("entry/hello-world-json.md")]
    public void ToIDocumentAndAnyContent_Test(string entryPath)
    {
        ILogger logger = _loggerProvider.CreateLogger(nameof(WriteNewPublicationEntryWithJsonFrontMatter_Test));

        var actual = new FileInfo(ProjectDirInfo.ToCombinedPath(entryPath));

        var (frontMatter, content) = actual.ToIDocumentAndAnyContent(logger);
        logger.LogInformation("{Label}: {Value}", nameof(frontMatter), frontMatter?.ToString());
        logger.LogInformation("{Label}: {Value}", nameof(content), content);
    }

    [Theory]
    [InlineData(@"yaml/hello-world-01-with-json-front-matter.yaml")]
    public void ToJsonString_Test(string path)
    {
        ILogger logger = _loggerProvider.CreateLogger(nameof(ToJsonString_Test));

        var entry = new FileInfo(ProjectDirInfo.ToCombinedPath(path));
        string? actual = entry.ToFrontMatterLinesAndContentLines(logger).FrontMatterLines.ToJsonString(logger);

        var node = JsonNode.Parse(actual!);
        logger.LogInformation(node!.ToJsonString());
    }

    [Theory]
    [InlineData(@"yaml/hello-world-01.yaml")]
    [InlineData(@"yaml/hello-world-01-not-inline.yaml")]
    public void ToYamlString_Test(string path)
    {
        ILogger logger = _loggerProvider.CreateLogger(nameof(ToYamlString_Test));

        var entry = new FileInfo(ProjectDirInfo.ToCombinedPath(path));
        string? actual = entry.ToFrontMatterLinesAndContentLines(logger).FrontMatterLines.ToYamlString(logger);

        logger.LogInformation("Attempting to parse:{NL}{Yaml}", Environment.NewLine, actual);
        IDictionary<string, object>? yO = YamlUtility.DeserializeYaml(actual);
        logger.LogInformation(yO.ToJsonString());
    }

    [Theory]
    [InlineData("Hello World!", "entry/hello-world-json.md", null)]
    public void WriteNewPublicationEntryWithJsonFrontMatter_Test(string title, string entryPath, string? content)
    {
        ILogger logger = _loggerProvider.CreateLogger(nameof(WriteNewPublicationEntryWithJsonFrontMatter_Test));

        var actual = new FileInfo(ProjectDirInfo.ToCombinedPath(entryPath));

        actual.WriteNewPublicationEntryWithJsonFrontMatter(title,"./entries/", content, logger);

        Assert.True(actual.Exists);
    }

    [Theory]
    [InlineData("Hello World!", "entry/hello-world.md", null)]
    public void WriteNewPublicationEntryWithYamlFrontMatter_Test(string title, string entryPath, string? content)
    {
        ILogger logger = _loggerProvider.CreateLogger(nameof(WriteNewPublicationEntryWithJsonFrontMatter_Test));

        var actual = new FileInfo(ProjectDirInfo.ToCombinedPath(entryPath));

        actual.WriteNewPublicationEntryWithYamlFrontMatter(title,"./", content, logger);

        Assert.True(actual.Exists);
    }

    static readonly DirectoryInfo ProjectDirInfo;

    readonly XUnitLoggerProvider _loggerProvider;
}