using System.Text.Json.Nodes;

namespace Songhay.Publications.Tests.Extensions;

public class MarkdownEntryExtensionsTests(ITestOutputHelper helper)
{
    [Fact]
    public void DoNullCheck_ArgumentNullException_Test()
    {
        // arrange
        MarkdownEntry? data = null;

        // assert
        Assert.Throws<ArgumentNullException>(data.DoNullCheck);
    }

    [Fact]
    public void DoNullCheck_NullReferenceException_Test()
    {
        // arrange
        MarkdownEntry data = new();

        // assert
        Assert.Throws<NullReferenceException>(data.DoNullCheck);
    }

    [Theory]
    [ProjectFileData([255], "../../../test-files/markdown/presentation-drafts/to-extract-test.md")]
    [ProjectFileData([255], "../../../test-files/markdown/presentation-drafts/to-extract-test2.md")]
    public void ToExtract_Test(int expectedLength, FileInfo entryInfo)
    {
        MarkdownEntry entry = entryInfo.ToMarkdownEntry();

        string extract = entry.ToExtract(expectedLength);

        Assert.False(string.IsNullOrWhiteSpace(extract));
        Assert.Equal(expectedLength + 1, extract.Length);
    }

    [Theory]
    [ProjectFileData("../../../test-files/markdown/to-markdown-entry-test.md")]
    public void ToFinalEdit_Test(FileInfo entryInfo)
    {
        //arrange
        string expected = File.ReadAllText(entryInfo.FullName);

        // act
        string actual = entryInfo.ToMarkdownEntry().ToFinalEdit();

        //assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [ProjectFileData(
        ["2005-01-18-msn-video-replaces-feedroomcom"],
        "../../../test-files/markdown/to-markdown-entry-test.md")]
    public void ToMarkdownEntry_Test(string clientId, FileInfo entryInfo)
    {
        // arrange
        MarkdownEntry entry = entryInfo.ToMarkdownEntry();

        // act
        JsonObject jFrontMatter = entry.FrontMatter;

        // assert
        Assert.NotNull(jFrontMatter);
        Assert.Equal(clientId, jFrontMatter["clientId"]?.GetValue<string>());
    }

    [Theory]
    [ProjectFileData([5], "../../../test-files/markdown/to-markdown-entry-test.md")]
    public void ToParagraphs_Test(int expectedNumberOfParagraphs, FileInfo entryInfo)
    {
        //arrange
        MarkdownEntry entry = entryInfo.ToMarkdownEntry();

        // act
        string[] paragraphs = entry.ToParagraphs();

        //assert
        Assert.Equal(expectedNumberOfParagraphs, paragraphs.Length);
    }

    [Theory]
    [ProjectFileData("../../../test-files/markdown/to-markdown-entry-test.md")]
    public void ToParagraphsAndToFinalEdit_Test(FileInfo entryInfo)
    {
        //arrange
        string expected = File.ReadAllText(entryInfo.FullName);
        MarkdownEntry entry = entryInfo.ToMarkdownEntry();
        string[] paragraphs = entry.ToParagraphs();

        // act
        entry.Content = paragraphs
            .Aggregate(
                string.Empty,
                (a, p) => string.Concat(a, $"{MarkdownEntry.NewLine}{MarkdownEntry.NewLine}", p)
            );
        string actual = entryInfo.ToMarkdownEntry().ToFinalEdit();

        //assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Hello World!", null, "./path", null, 8)]
    [InlineData("Hello World!", "It was the best of times.", "./path", null, 8)]
    [InlineData("Hello World!", "It was the best of times.", "./path", "thing", 8)]
    [InlineData("Hello World!", "It was the best of times.", "./path", "{\"thing\": true}", 8)]
    public void With11tyExtract_Test(string? title, string? content, string? path, string? tag, int length)
    {
        MarkdownEntry entry = (new MarkdownEntry())
            .WithNew11TyFrontMatter(title, DateTime.Now, path, tag)
            .WithContentHeader()
            .WithEdit(i => i.Content = string.Concat(i.Content, content)).With11TyExtract(length);

        JsonNode? jO = JsonNode.Parse(entry.FrontMatter["tag"]?.GetValue<string>() ?? "null");
        Assert.NotEqual(JsonValueKind.Null, jO?.GetValueKind());

        string? extract = jO?["extract"]?.GetValue<string>();

        helper.WriteLine($"front matter (input-tag: `{tag ?? "[null]"}`):");
        helper.WriteLine($"{entry.FrontMatter}");

        Assert.False(string.IsNullOrWhiteSpace(extract));
    }

    [Theory]
    [ProjectFileData([255], "../../../test-files/markdown/presentation-drafts/to-extract-test.md")]
    [ProjectFileData([255], "../../../test-files/markdown/presentation-drafts/to-extract-test2.md")]
    public void With11tyExtract_FromFile_Test(int expectedLength, FileInfo entryInfo)
    {
        MarkdownEntry entry = entryInfo.ToMarkdownEntry()
            .With11TyExtract(expectedLength);

        JsonNode? jO = JsonNode.Parse(entry.FrontMatter["tag"]?.GetValue<string>() ?? "null");
        Assert.NotEqual(JsonValueKind.Null, jO?.GetValueKind());

        string? extract = jO?["extract"]?.GetValue<string>();

        Assert.False(string.IsNullOrWhiteSpace(extract));
        Assert.Equal(expectedLength + 1, extract.Length);
    }

    [Fact]
    public void WithNew11tyFrontMatterAndContentHeaderAndTouch_Test()
    {
        //arrange
        const string title = "Hello World!";
        DateTime inceptDate = DateTime.Now.AddSeconds(-3);
        MarkdownEntry entry = new MarkdownEntry()
            .WithNew11TyFrontMatter(title, inceptDate, "/path/to/entry/", "entry_tag")
            .WithContentHeader();

        //act
        helper.WriteLine(entry.ToFinalEdit());
        entry.Touch(DateTime.Now);

        //assert
        JsonNode? otherDoc = JsonNode.Parse("{ \"entry\": {\"modificationDate\": \"2019-11-22T05:58:34.573Z\" } }");
        JsonNode? node = entry.FrontMatter["modificationDate"];
        JsonNode? otherNode = otherDoc?["entry"]?["modificationDate"];
        DateTime? v = otherNode?.GetValue<DateTime>();

        Assert.True(node?.GetValue<DateTime>() > inceptDate);

        helper.WriteLine(node.ToJsonString());

        Assert.NotNull(v);
    }
}
