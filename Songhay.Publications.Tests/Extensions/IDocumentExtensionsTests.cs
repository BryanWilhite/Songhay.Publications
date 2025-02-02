using Songhay.Publications.Abstractions;

namespace Songhay.Publications.Tests.Extensions;

// ReSharper disable once InconsistentNaming
public class IDocumentExtensionsTests
{
    public IDocumentExtensionsTests(ITestOutputHelper helper)
    {
        _testOutputHelper = helper;
        _loggerProvider = new XUnitLoggerProvider(helper);
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
            .GetDocumentByPredicate(i => i.ClientId == clientId)
            .ToReferenceTypeValueOrThrow();

        Assert.Equal(clientId, first.ClientId);
    }

    [Fact]
    public void HasFragments_Test()
    {
        var testCollection = new (bool expectedResult, IDocument? data)[]
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
        var testCollection = new (IDocument? data, Func<IDocument?, bool> test)[]
        {
            (
                null,
                data =>
                {
                    var text = data.ToDisplayText();
                    _testOutputHelper.WriteLine(text);

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
                    _testOutputHelper.WriteLine(text);

                    return data switch
                    {
                        null => false,
                        _ =>
                            !string.IsNullOrWhiteSpace(data.ClientId) && text.Contains(data.ClientId) &&
                            !string.IsNullOrWhiteSpace(data.DocumentShortName) && text.Contains(data.DocumentShortName) &&
                            !string.IsNullOrWhiteSpace(data.FileName) && text.Contains(data.FileName) &&
                            !string.IsNullOrWhiteSpace(data.Path) && text.Contains(data.Path) &&
                            !string.IsNullOrWhiteSpace(data.Title) && text.Contains(data.Title) &&
                            !text.Contains(DateTime.Now.Day.ToString())
                    };
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
                    _testOutputHelper.WriteLine(text);

                    return data switch
                    {
                        null => false,
                        _ =>
                            !string.IsNullOrWhiteSpace(data.ClientId) && text.Contains(data.ClientId) &&
                            !string.IsNullOrWhiteSpace(data.DocumentShortName) && text.Contains(data.DocumentShortName) &&
                            !string.IsNullOrWhiteSpace(data.FileName) && text.Contains(data.FileName) &&
                            text.Contains($"{data.IsActive}") &&
                            !string.IsNullOrWhiteSpace(data.Path) && text.Contains(data.Path) &&
                            !string.IsNullOrWhiteSpace(data.Title) && text.Contains(data.Title) &&
                            !text.Contains(DateTime.Now.Day.ToString())
                    };
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
                    _testOutputHelper.WriteLine(text);

                    return data switch
                        {
                            null => false,
                            _ =>
                                text.Contains($"{data.DocumentId}") &&
                                !string.IsNullOrWhiteSpace(data.ClientId) && text.Contains(data.ClientId) &&
                                !string.IsNullOrWhiteSpace(data.DocumentShortName) && !text.Contains(data.DocumentShortName) &&
                                !string.IsNullOrWhiteSpace(data.FileName) && !text.Contains(data.FileName) &&
                                !string.IsNullOrWhiteSpace(data.Path) && !text.Contains(data.Path) &&
                                !string.IsNullOrWhiteSpace(data.Title) && !text.Contains(data.Title) &&
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

    // ReSharper disable once InconsistentNaming
    public static IEnumerable<object[]> TestData = new[]
    {
        new object[]
        {
            new Document { DocumentId = 1, Title = "Hey!", Tag = @"{ ""extract"": ""Hello world!"" }"},
        },
        new object[]
        {
            new Document { DocumentId = 1, Title = "Hey!", Tag = @"{ ""extract"": ""Hello world!"", ""keywords"": [ ""yup"" ] }"},
        },
    };

    [Theory]
    [MemberData(nameof(TestData))]
    public void ToYaml_Test(IDocument document)
    {
        ILogger logger = _loggerProvider.CreateLogger(nameof(ToYaml_Test));

        string? actual = document.ToYaml(logger);

        logger.LogInformation(actual);
    }

    readonly XUnitLoggerProvider _loggerProvider;
    readonly ITestOutputHelper _testOutputHelper;
}
