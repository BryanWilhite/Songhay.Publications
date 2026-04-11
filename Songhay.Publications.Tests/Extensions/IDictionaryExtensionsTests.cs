using Songhay.Publications.Abstractions;

namespace Songhay.Publications.Tests.Extensions;

// ReSharper disable once InconsistentNaming
public class IDictionaryExtensionsTests(ITestOutputHelper helper)
{
    [Theory]
    [ProjectDirectoryData("test-files/yaml/hello-world-01-tagged.yaml", "myCustomProperty", "myOtherCustomProperty")]
    public void ToTaggedDocument_Test(DirectoryInfo projectDirInfo, string path, params string[] tagKeys)
    {
        ILogger logger = _loggerProvider.CreateLogger(nameof(ToTaggedDocument_Test));

        FileInfo entry = new(projectDirInfo.ToCombinedPath(path));
        string? yaml = entry.ToFrontMatterLinesAndContentLines(logger).FrontMatterLines.ToYamlString(logger);

        logger.LogInformation("YAML:{NL}{Data}", Environment.NewLine, yaml);

        IDictionary<string, object>? data = YamlUtility.DeserializeYaml(yaml);
        Assert.NotNull(data);
        Assert.NotEmpty(data);

        IDocument? actual = data.ToTaggedDocument(logger, tagKeys);
        Assert.NotNull(actual);
        logger.LogInformation("{Label}:{NL}{Data}", nameof(IDocument), Environment.NewLine, actual);
        logger.LogInformation("{Label}:{NL}{Data}", nameof(IDocument.Tag), Environment.NewLine, actual.Tag);
    }

    private readonly XUnitLoggerProvider _loggerProvider = new(helper);
}
