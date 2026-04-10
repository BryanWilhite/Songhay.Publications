using Songhay.Publications.Abstractions;

namespace Songhay.Publications.Tests.Extensions;

// ReSharper disable once InconsistentNaming
public class IDocumentExtensionsTests(ITestOutputHelper helper)
{
    [Fact]
    public void GetDocumentByPredicate_Test()
    {
        const string clientId = "my-data";

        Document[] collection =
        [
            new(),
            new(),
            new() { ClientId = clientId },
            new()
        ];

        IDocument first = collection
            .GetDocumentByPredicate(i => i.ClientId == clientId)
            .ToReferenceTypeValueOrThrow();

        Assert.Equal(clientId, first.ClientId);
    }

    public static TheoryData<bool, IDocument?> HasFragmentsTestTheoryData = new()
    {
        { false, null },
        { false, new Document() },
        {
            true,
            new Document
            {
                Fragments = [new()]
            }
        }
    };

    [Theory, MemberData(nameof(HasFragmentsTestTheoryData))]
    public void HasFragments_Test(bool expectedResult, IDocument? data)
    {
        if (data == null)
        {
            Assert.Throws<ArgumentNullException>(() => data.HasFragments());

            return;
        }

        bool actual = data.HasFragments();

        Assert.Equal(expectedResult, actual);
    }

    public static TheoryData<IDocument?, Func<IDocument?, bool>> ToDisplayTextTestTheoryData = new()
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
            new Document
            {
                ClientId = "my-document",
                DocumentShortName = "my-short-name",
                FileName = "my-file.name",
                IsActive = true,
                Path = "./",
                Title = "my-title",
            },
            data =>
            {
                string text = data.ToDisplayText();

                return data switch
                {
                    null => false,
                    _ =>
                        !string.IsNullOrWhiteSpace(data.ClientId) && text.Contains(data.ClientId) &&
                        !string.IsNullOrWhiteSpace(data.DocumentShortName) && text.Contains(data.DocumentShortName) &&
                        !string.IsNullOrWhiteSpace(data.FileName) && text.Contains(data.FileName) &&
                        !string.IsNullOrWhiteSpace(data.Path) && text.Contains(data.Path) &&
                        !string.IsNullOrWhiteSpace(data.Title) && text.Contains(data.Title)
                };
            }
        },
        {
            new Document
            {
                ClientId = "my-document",
                DocumentShortName = "my-short-name",
                FileName = "my-file.name",
                IsActive = true,
                Path = "./",
                Title = "my-title",
            },
            data =>
            {
                string text = $"{data}";

                return data switch
                {
                    null => false,
                    _ =>
                        !string.IsNullOrWhiteSpace(data.ClientId) && text.Contains(data.ClientId) &&
                        !string.IsNullOrWhiteSpace(data.DocumentShortName) && text.Contains(data.DocumentShortName) &&
                        !string.IsNullOrWhiteSpace(data.FileName) && text.Contains(data.FileName) &&
                        text.Contains($"{data.IsActive}") &&
                        !string.IsNullOrWhiteSpace(data.Path) && text.Contains(data.Path) &&
                        !string.IsNullOrWhiteSpace(data.Title) && text.Contains(data.Title)
                };
            }
        },
        {
            new Document
            {
                DocumentId = 999,
                ClientId = "my-document",
                DocumentShortName = "my-short-name",
                FileName = "my-file.name",
                Path = "./",
                Title = "my-title",
            },
            data =>
            {
                string text = data.ToDisplayText(showIdOnly: true);

                return data switch
                {
                    null => false,
                    _ =>
                        text.Contains($"{data.DocumentId}") &&
                        !string.IsNullOrWhiteSpace(data.ClientId) && text.Contains(data.ClientId) &&
                        !string.IsNullOrWhiteSpace(data.DocumentShortName) && !text.Contains(data.DocumentShortName) &&
                        !string.IsNullOrWhiteSpace(data.FileName) && !text.Contains(data.FileName) &&
                        !string.IsNullOrWhiteSpace(data.Path) && !text.Contains(data.Path) &&
                        !string.IsNullOrWhiteSpace(data.Title) && !text.Contains(data.Title)
                };
            }
        }
    };

    [Theory]
    [MemberData(nameof(ToDisplayTextTestTheoryData))]
    public void ToDisplayText_Test(IDocument? data, Func<IDocument?, bool> test)
    {
        bool actual = test(data);

        Assert.True(actual);
    }

    // ReSharper disable once InconsistentNaming
    public static TheoryData<IDocument> ToYamlTestTheoryData =
    [
        new Document { DocumentId = 1, Title = "Hey!", Tag = """{ "extract": "Hello world!" }""" },
        new Document { DocumentId = 1, Title = "Hey!", Tag = """{ "extract": "Hello world!", "keywords": [ "yup" ] }""" }
    ];

    [Theory, MemberData(nameof(ToYamlTestTheoryData))]
    public void ToYaml_Test(IDocument document)
    {
        ILogger logger = _loggerProvider.CreateLogger(nameof(ToYaml_Test));

        string? actual = document.ToYaml(logger);

        logger.LogInformation(actual);
    }

    private readonly XUnitLoggerProvider _loggerProvider = new(helper);
}
