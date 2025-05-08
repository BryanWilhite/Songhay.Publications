namespace Songhay.Publications.Extensions;

/// <summary>
/// Extensions of <see cref="JsonObject"/>
/// </summary>
public static class JsonObjectExtensions
{
    /// <summary>
    /// Converts the specified <see cref="JsonElement"/>
    /// to a YAML <see cref="string"/>.
    /// </summary>
    /// <param name="documentData">the <see cref="JsonObject"/></param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    public static string? ToYaml(this JsonObject? documentData, ILogger logger)
    {
        switch (documentData)
        {
            case null:
                logger.LogWarning("Warning: the expected {Name} is not here.", nameof(JsonObject));

                return null;
            default:
                return JsonDocument.Parse(documentData.ToJsonString()).RootElement.ToYaml();
        }
    }

    /// <summary>
    /// Returns the specified <see cref="JsonObject"/>
    /// with a Publications extract.
    /// </summary>
    /// <param name="documentData">the <see cref="JsonObject"/></param>
    /// <param name="contentLines">the collection of content lines</param>
    /// <param name="extractLength">the length of the extract</param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    public static JsonObject? WithExtract(this JsonObject? documentData, IReadOnlyCollection<string>? contentLines, int extractLength, ILogger logger)
    {
        if (documentData == null || contentLines == null) return null;

        const string extract = "extract";
        string? extractData = PublicationLinesUtility.ConvertToExtract(contentLines, extractLength, logger);

        if (documentData.HasProperty(extract))
        {
            logger.LogInformation("Updating extract from content...");
            documentData[extract] = extractData;
        }
        else
        {
            logger.LogInformation("Adding extract from content...");
            documentData.Add(extract, extractData);
        }

        return documentData;
    }

    /// <summary>
    /// Returns the specified <see cref="JsonObject"/>
    /// without the properties that should display in front matter.
    /// </summary>
    /// <param name="documentData">the <see cref="JsonObject"/></param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    public static JsonObject? WithoutConventionalDocumentProperties(this JsonObject? documentData, ILogger logger)
    {
        if (documentData == null)
        {
            logger.LogWarning("Warning: the expected {Name} is not here.", nameof(JsonObject));

            return null;
        }

        documentData.Remove(nameof(Document.TemplateId));
        documentData.Remove(nameof(Document.Segment));
        documentData.Remove(nameof(Document.Fragments));
        documentData.Remove(nameof(Document.IndexKeywords));
        documentData.Remove(nameof(Document.ResponsiveImages));

        return documentData;
    }

    /// <summary>
    /// Returns the specified <see cref="JsonObject"/>
    /// without the properties that are <c>null</c>
    /// and marked <c>DisallowNull</c>.
    /// </summary>
    /// <param name="documentData">the <see cref="JsonObject"/></param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    public static JsonObject? WithoutNonNullableDocumentProperties(this JsonObject? documentData, ILogger logger)
    {
        if (documentData == null)
        {
            logger.LogWarning("Warning: the expected {Name} is not here.", nameof(JsonObject));

            return null;
        }

        if(documentData[nameof(Document.DocumentId)]?.GetValue<int?>() == null)
            documentData.Remove(nameof(Document.DocumentId));

        if(documentData[nameof(Document.ClientId)]?.GetValue<int?>() == null)
            documentData.Remove(nameof(Document.ClientId));

        return documentData;
    }
}
