using System;
using Songhay.Publications.Extensions;
using Songhay.Publications.Models;
using Xunit;
using Xunit.Abstractions;

namespace Songhay.Publications.Tests.Extensions
{
    public class IDocumentExtensionsTests
    {
        public IDocumentExtensionsTests(ITestOutputHelper helper)
        {
            this._testOutputHelper = helper;
        }

        [Fact]
        public void HasFragments_Test()
        {
            var testCollection = new (bool expectedResult, IDocument data)[]
            {
                ( expectedResult: false, data: null ),
                ( expectedResult: false, data: new Document() ),
                (
                    expectedResult: true,
                    data: new Document
                    {
                        Fragments = new [] { new Fragment() }
                    }
                ),
            };

            foreach (var test in testCollection)
            {
                if (test.data == null)
                {
                    Assert.Throws<ArgumentNullException>(() => test.data.HasFragments());
                    continue;
                }

                var actual = test.data.HasFragments();
                Assert.Equal(test.expectedResult, actual);
            }
        }

        readonly ITestOutputHelper _testOutputHelper;
    }
}
