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
