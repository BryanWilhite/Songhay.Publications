namespace Songhay.Publications.Extensions;

/// <summary>
/// Extensions of <see cref="JsonObject"/>
/// </summary>
public static class JsonObjectExtensions
{
    /// <summary>
    /// Returns a serialized JSON string,
    /// according to the conventions
    /// for <see cref="IDocument.Tag"/>.
    /// </summary>
    /// <param name="documentData">the <see cref="JsonObject"/></param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    /// <remarks>
    /// <para>
    /// This member expects to see a <see cref="JsonObject"/>
    /// with several properties with names prefixed with <c>rx</c>
    /// and a <c>tags</c> property.
    /// </para>
    ///
    /// <para>
    /// This implies that the <see cref="JsonObject"/> is generated
    /// from a Markdown file and this member is being used to save
    /// into a Publications database.
    /// </para>
    /// </remarks>
    public static string? GetSerializedTagsAndCustomProperties(this JsonObject? documentData, ILogger logger)
    {
        if (documentData == null) return null;

        JsonNode? tags = documentData.GetPropertyJsonNodeOrNull("tags", logger);
        if (tags == null)
        {
            logger.LogError("The expected front-matter tags are not here.");

            return null;
        }

        const string prefix = "rx";
        JsonObject properties = new();

        foreach (var (propertyName, node) in documentData.AsEnumerable())
        {
            if(!propertyName.StartsWith(prefix)) continue;

            properties[propertyName] = node?.DeepClone();
        }

        var anon = new { tags, properties };

        string json = JsonSerializer.Serialize(anon);

        return json;
    }

    /// <summary>
    /// Converts the <see cref="JsonObject"/>
    /// into a newly allocated instance of <see cref="IDocument"/>.
    /// </summary>
    /// <param name="documentData">the <see cref="JsonObject"/></param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    /// <seealso cref="DocumentUtility.UpdateDocument"/>
    public static IDocument ToIDocument(this JsonObject? documentData, ILogger logger)
    {
        Document document = new();

        if (documentData == null) return document;

        DocumentUtility.UpdateDocument(document, documentData, logger);

        return document;
    }

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
            {
                using JsonDocument jDoc = JsonDocument.Parse(documentData.ToJsonString());

                return jDoc.RootElement.ToYaml();
            }
        }
    }

    /// <summary>
    /// Updates the <see cref="JsonObject"/>
    /// with the specified instance of <see cref="IDocument"/>.
    /// </summary>
    /// <param name="documentData">the <see cref="JsonObject"/></param>
    /// <param name="document">the <see cref="IDocument"/></param>
    /// <seealso cref="DocumentUtility.UpdateFrontMatter"/>
    public static void Update(this JsonObject? documentData, IDocument? document) =>
        DocumentUtility.UpdateFrontMatter(documentData, document as Document);

    /// <summary>
    /// Returns the specified <see cref="JsonObject"/>
    /// with a <see cref="JsonArray"/> property.
    /// </summary>
    /// <param name="jsonObject">the <see cref="JsonObject"/></param>
    /// <param name="arrayPropertyName">the property name of the array</param>
    public static JsonObject? WithArrayProperty(this JsonObject? jsonObject, string? arrayPropertyName)
    {
        if (jsonObject == null) return null;
        if (string.IsNullOrWhiteSpace(arrayPropertyName)) return jsonObject;

        if (!jsonObject.HasProperty(arrayPropertyName))
            jsonObject[arrayPropertyName] = JsonArray.Create(JsonElement.Parse("[]"));

        return jsonObject;
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
    /// with the conventional Index-document properties
    /// for Studio Publications.
    /// </summary>
    /// <param name="documentData">the <see cref="JsonObject"/></param>
    /// <param name="tag">the value of <see cref="IDocument.Tag"/></param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    /// <seealso cref="DocumentUtility.GetConventionalTagsAndPropertiesElements"/>
    public static JsonObject WithIndexDocumentProperties(this JsonObject? documentData, string? tag, ILogger logger)
    {
        if (documentData == null) return new JsonObject();
        if(string.IsNullOrWhiteSpace(tag)) return documentData;

        var (tagsE, propE) = DocumentUtility.GetConventionalTagsAndPropertiesElements(documentData, tag, logger);

        if (propE.ValueKind == JsonValueKind.Object) documentData["properties"] = propE.ToJsonNode();
        if(tagsE.ValueKind == JsonValueKind.Array) documentData["keywords"] = JsonArray.Create(tagsE);

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
