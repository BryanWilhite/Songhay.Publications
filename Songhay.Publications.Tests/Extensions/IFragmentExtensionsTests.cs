using System.Diagnostics;
using Songhay.Diagnostics;
using Songhay.Extensions;
using Songhay.Publications.Extensions;
using Songhay.Publications.Models;
using Xunit;
using Xunit.Abstractions;

namespace Songhay.Publications.Tests.Extensions
{
    public class IFragmentExtensionsTests
    {
        static IFragmentExtensionsTests() => traceSource = TraceSources
            .Instance
            .GetTraceSourceFromConfiguredName()
            .WithSourceLevels();

        static readonly TraceSource traceSource;

        public IFragmentExtensionsTests(ITestOutputHelper helper)
        {
            this._testOutputHelper = helper;
        }

        [Fact]
        public void GetFragmentByPredicate_Test()
        {
            var clientId = "my-data";

            var collection = new[]
            {
                new Fragment(),
                new Fragment(),
                new Fragment { ClientId = clientId },
                new Fragment(),
            };

            var first = collection
                .GetFragmentByPredicate(i => i.ClientId == clientId);

            Assert.NotNull(first);
            Assert.Equal(clientId, first.ClientId);
        }

        readonly ITestOutputHelper _testOutputHelper;
    }
}
