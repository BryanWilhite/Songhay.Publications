using System.Text.Json.Nodes;
using FluentValidation.Results;
using Songhay.Publications.Abstractions;

namespace Songhay.Publications.Tests.Extensions;

// ReSharper disable once InconsistentNaming
public class IDocumentExtensionsTests(ITestOutputHelper helper)
{
    [Fact]
    public void GetDocumentByPredicate_Test()
    {
        const string clientId = "my-data";
        ILogger logger = _loggerProvider.CreateLogger(nameof(GetDocumentByPredicate_Test));

        Document[] collection =
        [
            new(),
            new(),
            new() { ClientId = clientId },
            new()
        ];

        IDocument first = collection
            .GetDocumentByPredicate(i => i.ClientId == clientId, logger)
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
        ILogger logger = _loggerProvider.CreateLogger(nameof(HasFragments_Test));

        if (data == null)
        {
            Assert.Throws<ArgumentNullException>(() => data.HasFragments(logger));

            return;
        }

        bool actual = data.HasFragments(logger);

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

    [Theory]
    [InlineData(
        """
        {
            "documentId": 101,
            "title": "My Document",
            "tag": "{\"tags\": [\"one\",\"two\",\"three\"], \"properties\": { \"rxOne\": 1, \"rxTwo\": \"dos\", \"rxThree\": true }}"
        }
        """,
        """
        {
          "documentId": 101,
          "title": "My Document",
          "documentShortName": null,
          "fileName": null,
          "path": null,
          "templateId": null,
          "segmentId": null,
          "isRoot": null,
          "isActive": null,
          "sortOrdinal": null,
          "clientId": null,
          "endDate": null,
          "inceptDate": null,
          "modificationDate": null,
          "tags": [
            "one",
            "two",
            "three"
          ],
          "rxExtract": null,
          "rxIndexThumb": null,
          "rxNextLink": null,
          "rxOpenGraphProtocolImageUri": "urn:og:image:default",
          "rxPreviousLink": null,
          "rxWrapperLink": null,
          "rxOne": 1,
          "rxTwo": "dos",
          "rxThree": true
        }
        """
    )]
    public void ToConventionalFrontMatter_Test(string inputJson, string expectedOutput)
    {
        // arrange:
        ILogger logger = _loggerProvider.CreateLogger(nameof(ToConventionalFrontMatter_Test));
        IDocument document = JsonSerializer
            .Deserialize<Document>(inputJson, JsonSerializerOptionsCache.OptionsForCamelCaseWithIndentation)
            .ToReferenceTypeValueOrThrow();
        JsonObject jO = document.ToConventionalFrontMatter(logger);

        // act:
        string actual = jO.ToJsonString(JsonSerializerOptionsCache.OptionsForCamelCaseWithIndentation);

        helper.WriteLine(actual);

        // assert:
        Assert.Equal(expectedOutput, actual);
    }

    [Theory]
    [InlineData(
        """
        {
            "documentId": 101,
            "title": "My Document"
        }
        """, false)]
    [InlineData(
        """
        {
            "documentId": 102,
            "title": "My Document",
            "inceptDate": "1999-08-24T20:40:23.000Z",
            "modificationDate": "2011-07-10T19:58:49.403Z",
            "fileName": "hello.html",
            "path": "./",
            "segmentId": 12345 
        }
        """, true)]
    [InlineData(
        """
        {
            "documentId": 103,
            "title": "My Document",
            "inceptDate": "1999-08-24T20:40:23.000Z",
            "modificationDate": "2011-07-10T19:58:49.403",
            "fileName": "hello.html",
            "path": "./",
            "segmentId": 12345 
        }
        """, false)] // modificationDate is not UTC
    public void ToStaticFileValidationResult_Test(string inputJson, bool shouldValidate)
    {
        // arrange:
        ILogger logger = _loggerProvider.CreateLogger(nameof(ToConventionalFrontMatter_Test));
        IDocument document = JsonSerializer
            .Deserialize<Document>(inputJson, JsonSerializerOptionsCache.OptionsForCamelCaseWithIndentation)
            .ToReferenceTypeValueOrThrow();

        // act:
        ValidationResult? actual = document.ToStaticFileValidationResult(logger);

        // assert:
        Assert.Equal(shouldValidate, actual?.IsValid);
    }

    [Theory, MemberData(nameof(ToYamlTestTheoryData))]
    public void ToYaml_Test(IDocument document)
    {
        ILogger logger = _loggerProvider.CreateLogger(nameof(ToYaml_Test));

        string? actual = document.ToYaml(logger);

        logger.LogInformation(actual);
    }

    private readonly XUnitLoggerProvider _loggerProvider = new(helper);
}
