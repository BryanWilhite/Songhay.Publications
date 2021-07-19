using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Songhay.Extensions;
using Songhay.Publications.Extensions;
using Songhay.Publications.Models;
using Songhay.Tests;
using Xunit;
using Xunit.Abstractions;

namespace Songhay.Publications.Tests.Extensions
{
    public class ISegmentExtensionsTests
    {
        public ISegmentExtensionsTests(ITestOutputHelper helper)
        {
            this._testOutputHelper = helper;
        }

        [Fact]
        public void GetSegmentByPredicate_Test()
        {
            var clientId = "my-data";

            var collection = new[]
            {
                new Segment(),
                new Segment(),
                new Segment { ClientId = clientId },
                new Segment(),
            };

            var first = collection
                .GetSegmentByPredicate(i => i.ClientId == clientId);

            Assert.NotNull(first);
            Assert.Equal(clientId, first.ClientId);
        }

        [Fact]
        public void HasDocuments_Test()
        {
            var testCollection = new (bool expectedResult, ISegment data)[]
            {
                ( expectedResult: false, data: null ),
                ( expectedResult: false, data: new Segment() ),
                (
                    expectedResult: true,
                    data: new Segment
                    {
                        Documents = new [] { new Document() }
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
            var testCollection = new (ISegment data, Func<ISegment, bool> test)[]
            {
                (
                    null,
                    data =>
                    {
                        var text = data.ToDisplayText();
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
                        return
                            text.Contains(data.ClientId) &&
                            text.Contains(data.SegmentName) &&
                            text.Contains(data.IsActive.ToString()) &&
                            text.Contains(DateTime.Now.Day.ToString())
                            ;
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
                        return
                            text.Contains(data.ClientId) &&
                            text.Contains(data.SegmentName) &&
                            text.Contains(data.IsActive.ToString()) &&
                            text.Contains(DateTime.Now.Day.ToString())
                            ;
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
                        return
                            text.Contains(data.SegmentId.ToString()) &&
                            text.Contains(data.ClientId) &&
                            !text.Contains(data.SegmentName) &&
                            !text.Contains(data.IsActive.ToString()) &&
                            !text.Contains(DateTime.Now.Day.ToString())
                            ;
                    }
                ),
            };

            foreach (var item in testCollection)
            {
                Assert.True(item.test(item.data));
            }
        }

        [Theory]
        [ProjectFileData(typeof(ISegmentExtensionsTests), "../../../gen-web-data/responsive-layouts/index.json")]
        public void ToPublicationIndexEntries_Test(FileInfo indexInfo)
        {
            var json = File.ReadAllText(indexInfo.FullName);

            var segments = JsonConvert.DeserializeObject<IEnumerable<Segment>>(json);

            Assert.NotNull(segments);
            Assert.True(segments.Any());

            this._testOutputHelper.WriteLine($"converting enumeration of {nameof(Segment)}...");

            var entries = segments.ToPublicationIndexEntries();
            Assert.NotEmpty(entries);
        }

        [Theory]
        [ProjectFileData(typeof(ISegmentExtensionsTests), "../../../gen-web-data/responsive-layouts/index.json")]
        public void ToPublicationIndexEntry_Test(FileInfo indexInfo)
        {
            var json = File.ReadAllText(indexInfo.FullName);

            var segments = JsonConvert.DeserializeObject<IEnumerable<Segment>>(json);

            Assert.NotNull(segments);
            Assert.True(segments.Any());

            var segment = segments.First();

            this._testOutputHelper.WriteLine($"converting {nameof(Segment)} `{segment.SegmentName}`...");

            var entry = segment.ToPublicationIndexEntry();
            Assert.NotNull(entry);
        }

        [Theory]
        [ProjectFileData(typeof(ISegmentExtensionsTests), "../../../gen-web-data/responsive-layouts/index.json")]
        public void ToPublicationIndexEntryJObject_Test(FileInfo indexInfo)
        {
            var json = File.ReadAllText(indexInfo.FullName);

            var segments = JsonConvert.DeserializeObject<IEnumerable<Segment>>(json);

            Assert.NotNull(segments);
            Assert.True(segments.Any());

            var segment = segments.First();

            this._testOutputHelper.WriteLine($"converting {nameof(Segment)} `{segment.SegmentName}`...");

            var jIndex = segment.ToPublicationIndexEntryJObject(useJavaScriptCase: true);
            Assert.NotNull(jIndex);
            this._testOutputHelper.WriteLine($"{nameof(Segment)} `{segment.SegmentName}` parsed:");
            this._testOutputHelper.WriteLine(jIndex.ToString());

            var jDocuments = jIndex.GetJArray(nameof(Segment.Documents).ToLowerInvariant(), throwException: false);
            Assert.Null(jDocuments);

            var jSegments = jIndex.GetJArray(nameof(Segment.Segments).ToLowerInvariant(), throwException: false);
            Assert.NotNull(jSegments);
            Assert.True(jSegments.Any());
        }

        readonly ITestOutputHelper _testOutputHelper;
    }
}
