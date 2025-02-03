using Songhay.Publications.Abstractions;

namespace Songhay.Publications.Tests.Extensions;

// ReSharper disable once InconsistentNaming
public class IDictionaryExtensionsTests
{
    static IDictionaryExtensionsTests()
    {
        var type = typeof(IDictionaryExtensionsTests);
        var projectDirectory = ProgramAssemblyUtility.GetPathFromAssembly(type.Assembly, "../../../");

        ProjectDirInfo = new DirectoryInfo(projectDirectory);
    }

    public IDictionaryExtensionsTests(ITestOutputHelper helper)
    {
        _loggerProvider = new XUnitLoggerProvider(helper);
    }

    [Theory]
    [InlineData(@"yaml/hello-world-01-tagged.yaml", "myCustomProperty", "myOtherCustomProperty")]
    public void ToTaggedDocument_Test(string path, params string[] tagKeys)
    {
        ILogger logger = _loggerProvider.CreateLogger(nameof(ToTaggedDocument_Test));

        var entry = new FileInfo(ProjectDirInfo.ToCombinedPath(path));
        var yaml = entry.ToFrontMatterLinesAndContentLines(logger).FrontMatterLines.ToYamlString(logger);

        logger.LogInformation("YAML:{NL}{Data}", Environment.NewLine, yaml);

        var data = YamlUtility.DeserializeYaml(yaml);
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        IDocument? actual = data.ToTaggedDocument(logger, tagKeys);
        Assert.NotNull(actual);
        logger.LogInformation("{Label}:{NL}{Data}", nameof(IDocument), Environment.NewLine, actual);
        logger.LogInformation("{Label}:{NL}{Data}", nameof(IDocument.Tag), Environment.NewLine, actual.Tag);
    }

    static readonly DirectoryInfo ProjectDirInfo;

    readonly XUnitLoggerProvider _loggerProvider;
}