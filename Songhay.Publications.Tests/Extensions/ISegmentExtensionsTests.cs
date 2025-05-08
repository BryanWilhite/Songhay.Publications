using Songhay.Publications.Abstractions;

namespace Songhay.Publications.Tests.Extensions;

// ReSharper disable once InconsistentNaming
public class ISegmentExtensionsTests(ITestOutputHelper helper)
{
    [Fact]
    public void GetSegmentByPredicate_Test()
    {
        const string clientId = "my-data";

        var collection = new[]
        {
            new Segment(),
            new Segment(),
            new Segment { ClientId = clientId },
            new Segment(),
        };

        var first = collection
            .GetSegmentByPredicate(i => i.ClientId == clientId)
            .ToReferenceTypeValueOrThrow();

        Assert.Equal(clientId, first.ClientId);
    }

    [Fact]
    public void HasDocuments_Test()
    {
        var testCollection = new (bool expectedResult, ISegment? data)[]
        {
            ( expectedResult: false, data: null ),
            ( expectedResult: false, data: new Segment() ),
            (
                expectedResult: true,
                data: new Segment
                {
                    Documents = [new Document()]
                }
            ),
        };

        foreach (var test in testCollection)
        {
            if (test.data == null)
            {
                Assert.Throws<ArgumentNullException>(() => test.data.HasDocuments());
                continue;
            }

            var actual = test.data.HasDocuments();
            Assert.Equal(test.expectedResult, actual);
        }
    }

    [Fact]
    public void ToDisplayText_Test()
    {
        var testCollection = new (ISegment? data, Func<ISegment?, bool> test)[]
        {
            (
                null,
                data =>
                {
                    var text = data.ToDisplayText();
                    helper.WriteLine(text);

                    return text.Contains("the specified ") && text.Contains("is null.");
                }
            ),
            (
                new Segment
                {
                    ClientId = "my-segment",
                    SegmentName = "my-segment-name",
                    IsActive = true,
                    InceptDate = DateTime.Now
                },
                data =>
                {
                    var text = data.ToDisplayText();
                    helper.WriteLine(text);

                    return data switch
                    {
                        null => false,
                        _ =>
                            !string.IsNullOrWhiteSpace(data.ClientId) && text.Contains(data.ClientId) &&
                            !string.IsNullOrWhiteSpace(data.SegmentName) && text.Contains(data.SegmentName) &&
                            text.Contains($"{data.IsActive}") &&
                            text.Contains(DateTime.Now.Day.ToString())
                    };
                }
            ),
            (
                new Segment
                {
                    ClientId = "my-segment",
                    SegmentName = "my-segment-name",
                    IsActive = true,
                    InceptDate = DateTime.Now
                },
                data =>
                {
                    var text = $"{data}";
                    helper.WriteLine(text);

                    return data switch
                    {
                        null => false,
                        _ =>
                            !string.IsNullOrWhiteSpace(data.ClientId) && text.Contains(data.ClientId) &&
                            !string.IsNullOrWhiteSpace(data.SegmentName) && text.Contains(data.SegmentName) &&
                            text.Contains($"{data.IsActive}") &&
                            text.Contains(DateTime.Now.Day.ToString())
                    };
                }
            ),
            (
                new Segment
                {
                    SegmentId = 999,
                    ClientId = "my-segment",
                    SegmentName = "my-segment-name",
                    IsActive = true,
                    InceptDate = DateTime.Now
                },
                data =>
                {
                    var text = data.ToDisplayText(showIdOnly: true);
                    helper.WriteLine(text);

                    return data switch
                    {
                        null => false,
                        _ =>
                            text.Contains($"{data.SegmentId}") &&
                            !string.IsNullOrWhiteSpace(data.ClientId) && text.Contains(data.ClientId) &&
                            !string.IsNullOrWhiteSpace(data.SegmentName) && !text.Contains(data.SegmentName) &&
                            !text.Contains($"{data.IsActive}") &&
                            !text.Contains(DateTime.Now.Day.ToString())
                    };
                }
            )
        };

        foreach (var item in testCollection)
        {
            Assert.True(item.test(item.data));
        }
    }

    [Theory]
    [ProjectFileData("../../../gen-web-data/responsive-layouts/index.json",
        "../../../json/ToPublicationIndexEntries_Test_output.json")]
    public void ToPublicationIndexEntries_Test(FileInfo indexInfo, FileInfo outputInfo)
    {
        string json = File.ReadAllText(indexInfo.FullName);

        Segment[] segments = JsonSerializer
            .Deserialize<IEnumerable<Segment>>(json, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
            .ToReferenceTypeValueOrThrow()
            .ToArray();

        Assert.True(segments.Any());

        helper.WriteLine($"converting enumeration of {nameof(Segment)}...");

        var entries = segments.ToPublicationIndexEntries().ToArray();
        Assert.NotEmpty(entries);

        var jsonOutput = entries.ToJson();

        File.WriteAllText(outputInfo.FullName, jsonOutput);
    }

    [Theory]
    [ProjectFileData("../../../gen-web-data/responsive-layouts/index.json")]
    public void ToPublicationIndexEntry_Test(FileInfo indexInfo)
    {
        string json = File.ReadAllText(indexInfo.FullName);

        Segment[] segments = JsonSerializer
            .Deserialize<IEnumerable<Segment>>(json, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
            .ToReferenceTypeValueOrThrow()
            .ToArray();

        Assert.True(segments.Any());

        var segment = segments.First();
        Assert.False(string.IsNullOrWhiteSpace(segment.SegmentName));

        helper.WriteLine($"converting {nameof(Segment)} `{segment.SegmentName}`...");

        var entry = segment.ToPublicationIndexEntry();
        Assert.NotNull(entry);
    }
}