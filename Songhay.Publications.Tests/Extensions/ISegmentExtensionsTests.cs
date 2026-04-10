using Songhay.Publications.Abstractions;

namespace Songhay.Publications.Tests.Extensions;

// ReSharper disable once InconsistentNaming
public class ISegmentExtensionsTests(ITestOutputHelper helper)
{
    [Fact]
    public void GetSegmentByPredicate_Test()
    {
        const string clientId = "my-data";

        Segment[] collection =
        [
            new Segment(),
            new Segment(),
            new Segment { ClientId = clientId },
            new Segment()
        ];

        ISegment first = collection
            .GetSegmentByPredicate(i => i.ClientId == clientId)
            .ToReferenceTypeValueOrThrow();

        Assert.Equal(clientId, first.ClientId);
    }

    [Fact]
    public void HasDocuments_Test()
    {
        (bool expectedResult, ISegment? data)[] testCollection =
        [
            ( expectedResult: false, data: null ),
            ( expectedResult: false, data: new Segment() ),
            (
                expectedResult: true,
                data: new Segment
                {
                    Documents = [new Document()]
                }
            )
        ];

        foreach ((bool expectedResult, ISegment? data) test in testCollection)
        {
            if (test.data == null)
            {
                Assert.Throws<ArgumentNullException>(() => test.data.HasDocuments());
                continue;
            }

            bool actual = test.data.HasDocuments();
            Assert.Equal(test.expectedResult, actual);
        }
    }

    public static TheoryData<ISegment?, Func<ISegment?, bool>> ToDisplayTextTestTheoryData = new ()
    {
        {
            null,
            data =>
            {
                string text = data.ToDisplayText();

                return text.Contains("the specified ") && text.Contains("is null.");
            }
        },
        {
            new Segment
            {
                ClientId = "my-segment",
                SegmentName = "my-segment-name",
                IsActive = true,
                InceptDate = DateTime.Now
            },
            data =>
            {
                string text = data.ToDisplayText();

                return data switch
                {
                    null => false,
                    _ =>
                        !string.IsNullOrWhiteSpace(data.ClientId) && text.Contains(data.ClientId) &&
                        !string.IsNullOrWhiteSpace(data.SegmentName) && text.Contains(data.SegmentName) &&
                        text.Contains($"{data.IsActive}")
                };
            }
        },
        {
            new Segment
            {
                ClientId = "my-segment",
                SegmentName = "my-segment-name",
                IsActive = true,
                InceptDate = DateTime.Now
            },
            data =>
            {
                string text = $"{data}";

                return data switch
                {
                    null => false,
                    _ =>
                        !string.IsNullOrWhiteSpace(data.ClientId) && text.Contains(data.ClientId) &&
                        !string.IsNullOrWhiteSpace(data.SegmentName) && text.Contains(data.SegmentName) &&
                        text.Contains($"{data.IsActive}")
                };
            }
        },
        {
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
                string text = data.ToDisplayText(showIdOnly: true);

                return data switch
                {
                    null => false,
                    _ =>
                        text.Contains($"{data.SegmentId}") &&
                        !string.IsNullOrWhiteSpace(data.ClientId) && text.Contains(data.ClientId) &&
                        !string.IsNullOrWhiteSpace(data.SegmentName) && !text.Contains(data.SegmentName) &&
                        !text.Contains($"{data.IsActive}")
                };
            }
        }
    };

    [Theory, MemberData(nameof(ToDisplayTextTestTheoryData))]
    public void ToDisplayText_Test(ISegment? data, Func<ISegment?, bool> test)
    {
        Assert.True(test(data));
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

        Assert.NotEmpty(segments);

        helper.WriteLine($"converting enumeration of {nameof(Segment)}...");

        IIndexEntry[] entries = segments.ToPublicationIndexEntries().ToArray();
        Assert.NotEmpty(entries);

        string jsonOutput = entries.ToJson();

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

        Assert.NotEmpty(segments);

        Segment segment = segments.First();
        Assert.False(string.IsNullOrWhiteSpace(segment.SegmentName));

        helper.WriteLine($"converting {nameof(Segment)} `{segment.SegmentName}`...");

        IIndexEntry entry = segment.ToPublicationIndexEntry();
        Assert.NotNull(entry);
    }
}