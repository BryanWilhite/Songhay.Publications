using System.Reflection;
using System.Text.Json.Nodes;
using Songhay.Publications.Abstractions;

namespace Songhay.Publications.Tests.Extensions;

public class JsonObjectExtensionsTests(ITestOutputHelper helper)
{
    [Theory]
    [InlineData(
    """
            {
                "documentId": 101,
                "title": "My Document",
                "endDate": null,
                "inceptDate": "1999-08-24T20:40:23.000",
                "modificationDate": "2011-07-10T19:58:49.403"
            }
            """)]
    public void ToIDocument_Test(string inputJson)
    {
        // arrange:
        ILogger logger = _loggerProvider.CreateLogger(nameof(WithConventionalFrontMatter_Test));
        JsonObject documentData = JsonNodeUtility.ParseJsonObject(inputJson, logger)
            .ToReferenceTypeValueOrThrow();

        // act:
        IDocument document = documentData.ToIDocument(logger);

        // assert:
        foreach (KeyValuePair<string, JsonNode?> pair in documentData)
        {
            string? expected = documentData[pair.Key]?.ToString();

            string propertyName = pair.Key.ToPascalCase().ToReferenceTypeValueOrThrow();
            PropertyInfo? propertyInfo = document
                .GetType()
                .GetProperty(propertyName);

            if (propertyInfo?.PropertyType == typeof(DateTime?))
            {
                DateTime? actual = (DateTime?)propertyInfo.GetValue(document);

                Assert.Equal(expected, actual?.ToLocalTime().ToIso8601String(includeTimeMilliseconds: true));
            }
            else
            {
                string? actual = propertyInfo?
                    .GetValue(document)?
                    .ToString();

                Assert.Equal(expected, actual);
            }
        }

    }

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
    [InlineData(
        """
        {
            "fileName": "my.md",
            "tags": []
        }
        """,
        """
        {
          "documentId": null,
          "title": null,
          "documentShortName": null,
          "fileName": "my.md",
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
          "tags": [],
          "rxExtract": null,
          "rxIndexThumb": null,
          "rxNextLink": null,
          "rxOpenGraphProtocolImageUri": "urn:og:image:default",
          "rxPreviousLink": null,
          "rxWrapperLink": null
        }
        """)]
    [InlineData(
        """
        {
          "rxExtract": null,
          "rxNextLink": null,
          "rxOpenGraphProtocolImageUri": "urn:og:image:default",
          "rxPreviousLink": null,
          "rxWrapperLink": null,
          "documentId": 16767,
          "title": null,
          "documentShortName": null,
          "fileName": "my.md",
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
          "layout": "custom.html"
        }
        """,
        """
        {
          "documentId": 16767,
          "title": null,
          "documentShortName": null,
          "fileName": "my.md",
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
          "layout": "custom.html",
          "tags": [],
          "rxExtract": null,
          "rxIndexThumb": null,
          "rxNextLink": null,
          "rxOpenGraphProtocolImageUri": "urn:og:image:default",
          "rxPreviousLink": null,
          "rxWrapperLink": null
        }
        """)]
    public void WithConventionalFrontMatter_Test(string inputJson, string expectedOutput)
    {
        // arrange:
        ILogger logger = _loggerProvider.CreateLogger(nameof(WithConventionalFrontMatter_Test));
        JsonObject jO = JsonNodeUtility.ParseJsonObject(inputJson, logger)
            .ToReferenceTypeValueOrThrow()
            .WithConventionalFrontMatter(contentLines: null, contentLinesExtractLength: 0, logger);

        // act:
        string actual = jO.ToJsonString(JsonSerializerOptionsCache.OptionsForCamelCaseWithIndentation);

        helper.WriteLine(actual);

        // assert:
        Assert.Equal(expectedOutput, actual);
    }

    private readonly XUnitLoggerProvider _loggerProvider = new(helper);
}