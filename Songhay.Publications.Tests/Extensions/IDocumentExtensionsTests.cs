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
        public void GetDocumentByPredicate_Test()
        {
            var clientId = "my-data";

            var collection = new[]
            {
                new Document(),
                new Document(),
                new Document { ClientId = clientId },
                new Document(),
            };

            var first = collection
                .GetDocumentByPredicate(i => i.ClientId == clientId);

            Assert.NotNull(first);
            Assert.Equal(clientId, first.ClientId);
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

        [Fact]
        public void ToDisplayText_Test()
        {
            var testCollection = new (IDocument data, Func<IDocument, bool> test)[]
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
                    new Document
                    {
                        ClientId = "my-document",
                        DocumentShortName = "my-short-name",
                        FileName = "my-file.name",
                        IsActive = true,
                        Path = "./",
                        Title = "my-title",
                        InceptDate = DateTime.Now
                    },
                    data =>
                    {
                        var text = data.ToDisplayText();
                        return
                            text.Contains(data.ClientId) &&
                            text.Contains(data.DocumentShortName) &&
                            text.Contains(data.FileName) &&
                            text.Contains(data.Path) &&
                            text.Contains(data.Title) &&
                            !text.Contains(DateTime.Now.Day.ToString())
                            ;
                    }
                ),
                (
                    new Document
                    {
                        ClientId = "my-document",
                        DocumentShortName = "my-short-name",
                        FileName = "my-file.name",
                        IsActive = true,
                        Path = "./",
                        Title = "my-title",
                        InceptDate = DateTime.Now
                    },
                    data =>
                    {
                        var text = $"{data}";
                        return
                            text.Contains(data.ClientId) &&
                            text.Contains(data.DocumentShortName) &&
                            text.Contains(data.FileName) &&
                            text.Contains(data.IsActive.ToString()) &&
                            text.Contains(data.Path) &&
                            text.Contains(data.Title) &&
                            !text.Contains(DateTime.Now.Day.ToString())
                            ;
                    }
                ),
                (
                    new Document
                    {
                        DocumentId = 999,
                        ClientId = "my-document",
                        DocumentShortName = "my-short-name",
                        FileName = "my-file.name",
                        Path = "./",
                        Title = "my-title",
                        InceptDate = DateTime.Now
                    },
                    data =>
                    {
                        var text = data.ToDisplayText(showIdOnly: true);
                        return
                            text.Contains(data.DocumentId.ToString()) &&
                            text.Contains(data.ClientId) &&
                            !text.Contains(data.DocumentShortName) &&
                            !text.Contains(data.FileName) &&
                            !text.Contains(data.Path) &&
                            !text.Contains(data.Title) &&
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

        readonly ITestOutputHelper _testOutputHelper;
    }
}
