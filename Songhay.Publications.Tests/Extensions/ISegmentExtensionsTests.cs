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
