namespace Songhay.Publications;

/// <summary>
/// Shared routines for <seealso cref="Document"/> or <seealso cref="IDocument"/>.
/// </summary>
public static class DocumentUtility
{
    /// <summary>
    /// Returns a collection of property names
    /// based on <seealso cref="IDocument"/>
    /// </summary>
    /// <param name="includeTagProperty">When <c>true</c> return <see cref="IDocument.Tag"/> in the collection of property names</param>
    public static IReadOnlyCollection<string> GetConventionalFrontMatterProperties(bool includeTagProperty = true)
    {
        string[] properties =
        [
            nameof(IDocument.DocumentId).ToCamelCase().ToReferenceTypeValueOrThrow(),
            nameof(IDocument.Title).ToCamelCase().ToReferenceTypeValueOrThrow(),
            nameof(IDocument.DocumentShortName).ToCamelCase().ToReferenceTypeValueOrThrow(),
            nameof(IDocument.FileName).ToCamelCase().ToReferenceTypeValueOrThrow(),
            nameof(IDocument.Path).ToCamelCase().ToReferenceTypeValueOrThrow(),
            nameof(IDocument.TemplateId).ToCamelCase().ToReferenceTypeValueOrThrow(),
            nameof(IDocument.SegmentId).ToCamelCase().ToReferenceTypeValueOrThrow(),
            nameof(IDocument.IsRoot).ToCamelCase().ToReferenceTypeValueOrThrow(),
            nameof(IDocument.IsActive).ToCamelCase().ToReferenceTypeValueOrThrow(),
            nameof(IDocument.SortOrdinal).ToCamelCase().ToReferenceTypeValueOrThrow(),
            nameof(IDocument.ClientId).ToCamelCase().ToReferenceTypeValueOrThrow(),
            nameof(IDocument.EndDate).ToCamelCase().ToReferenceTypeValueOrThrow(),
            nameof(IDocument.InceptDate).ToCamelCase().ToReferenceTypeValueOrThrow(),
            nameof(IDocument.ModificationDate).ToCamelCase().ToReferenceTypeValueOrThrow(),
        ];

        if (includeTagProperty)
        {
            return
            [..
                properties,
                nameof(IDocument.Tag).ToCamelCase().ToReferenceTypeValueOrThrow()
            ];
        }

        return properties;
    }

    /// <summary>
    /// Returns a tuple of two <see cref="JsonElement"/> instances
    /// representing the conventional tags and properties elements.
    /// </summary>
    /// <param name="documentData">the <see cref="JsonObject"/></param>
    /// <param name="tag">the value of <see cref="IDocument.Tag"/></param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    /// <remarks>
    /// The <see cref="JsonObject"/> passed into this method
    /// should be or will be shaped like <see cref="IDocument"/>
    /// where <see cref="IDocument.Tag"/> can be serialized JSON
    /// of the form:
    ///
    /// <code>
    /// {
    ///     \"tags\": [...],
    ///     \"properties\": { ... }
    /// }
    /// </code>
    ///
    /// ...where the <c>tags</c> property is an array of strings
    /// and the <c>properties</c> property is an object
    /// of conventional properties, named with a prefix of <c>rx</c>.
    ///
    /// This use of <see cref="IDocument.Tag"/> is a temporary workaround
    /// until a formal concept of Document properties can be defined.
    /// </remarks>
    public static (JsonElement tags, JsonElement propeties) GetConventionalTagsAndPropertiesElements(JsonObject? documentData, string? tag, ILogger logger)
    {
        if (documentData == null) return (JsonElementUtility.GetNullJsonElement(), JsonElementUtility.GetNullJsonElement());

        documentData.RemoveProperty("tag", logger);

        JsonElement tagE = JsonElementUtility.ParseJson(tag, logger);
        if (tagE.ValueKind == JsonValueKind.Null)
        {
            logger.LogError("\nERROR - the document tag does not appear to be valid JSON:\n\n{Json}\n\n", tag);

            return (JsonElementUtility.GetNullJsonElement(), JsonElementUtility.GetNullJsonElement());
        }

        JsonElement tagsE = tagE.GetJsonChildElementOrNull(Tags) ?? JsonElementUtility.GetNullJsonElement();

        JsonElement propE = tagE.GetJsonChildElementOrNull("properties") ?? JsonElementUtility.GetNullJsonElement();

        return (tagsE, propE);
    }

    /// <summary>
    /// Updates the instance of <see cref="IDocument"/>
    /// with the specified <see cref="JsonObject"/>.
    /// </summary>
    /// <param name="document">the <see cref="IDocument"/></param>
    /// <param name="documentData">the <see cref="JsonObject"/></param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    /// <remarks>
    /// This member operates in the opposite ‘direction’
    /// of <see cref="UpdateFrontMatter"/>.
    /// </remarks>
    public static void UpdateDocument(IDocument? document, JsonObject? documentData, ILogger logger)
    {
        if(document == null || documentData == null) return;

        foreach (string propertyName in GetConventionalFrontMatterProperties(includeTagProperty: false))
        {
            JsonValue? jsonValue = documentData.GetPropertyJsonValueOrNull(propertyName);

            switch (propertyName.ToPascalCase())
            {
                case nameof(IDocument.DocumentId):
                    document.DocumentId = jsonValue?.GetValue<int?>() ?? 0;
                    break;
                case nameof(IDocument.Title):
                    document.Title = jsonValue?.GetValue<string?>();
                    break;
                case nameof(IDocument.DocumentShortName):
                    document.DocumentShortName = jsonValue?.GetValue<string?>();
                    break;
                case nameof(IDocument.FileName):
                    document.FileName = jsonValue?.GetValue<string?>();
                    break;
                case nameof(IDocument.Path):
                    document.Path = jsonValue?.GetValue<string?>();
                    break;
                case nameof(IDocument.TemplateId):
                    document.TemplateId = jsonValue?.GetValue<int?>();
                    break;
                case nameof(IDocument.SegmentId):
                    document.SegmentId = jsonValue?.GetValue<int?>();
                    break;
                case nameof(IDocument.IsRoot):
                    document.IsRoot = jsonValue?.GetValue<bool?>();
                    break;
                case nameof(IDocument.IsActive):
                    document.IsActive = jsonValue?.GetValue<bool?>();
                    break;
                case nameof(IDocument.SortOrdinal):
                    document.SortOrdinal = jsonValue?.GetValue<byte?>();
                    break;
                case nameof(IDocument.ClientId):
                    string? clientId = jsonValue?.GetValue<string?>();
                    if (!string.IsNullOrWhiteSpace(clientId)) document.ClientId = clientId;
                    break;
                case nameof(IDocument.EndDate):
                {
                    string? dateString = jsonValue?.GetValue<string?>();
                    document.EndDate = ProgramTypeUtility.ParseToUtcDateTimeFromLocalTime(dateString);

                    break;
                }
                case nameof(IDocument.InceptDate):
                {
                    string? dateString = jsonValue?.GetValue<string?>();
                    document.InceptDate = ProgramTypeUtility.ParseToUtcDateTimeFromLocalTime(dateString);

                    break;
                }
                case nameof(IDocument.ModificationDate):
                {
                    string? dateString = jsonValue?.GetValue<string?>();
                    document.ModificationDate = ProgramTypeUtility.ParseToUtcDateTimeFromLocalTime(dateString);

                    break;
                }
            }
        }

        document.Tag = documentData.GetSerializedTagsAndCustomProperties(logger);
    }

    /// <summary>
    /// Updates the <see cref="JsonObject"/>
    /// with the specified instance of <see cref="IDocument"/>.
    /// </summary>
    /// <param name="documentData">the <see cref="JsonObject"/></param>
    /// <param name="document">the <see cref="IDocument"/></param>
    /// <remarks>
    /// This member operates in the opposite ‘direction’
    /// of <see cref="UpdateDocument"/>.
    /// </remarks>
    public static void UpdateFrontMatter(JsonObject? documentData, Document? document)
    {
        if(document == null || documentData == null) return;

        foreach (string propertyName in GetConventionalFrontMatterProperties(includeTagProperty: false))
        {
            switch (propertyName.ToPascalCase())
            {
                case nameof(IDocument.DocumentId):
                    documentData[nameof(IDocument.DocumentId).ToCamelCase().ToReferenceTypeValueOrThrow()] = document.DocumentId;
                    break;
                case nameof(IDocument.Title):
                    documentData[nameof(IDocument.Title).ToLowerInvariant()] = document.Title;
                    break;
                case nameof(IDocument.DocumentShortName):
                    documentData[nameof(IDocument.DocumentShortName).ToCamelCase().ToReferenceTypeValueOrThrow()] = document.DocumentShortName;
                    break;
                case nameof(IDocument.FileName):
                    documentData[nameof(IDocument.FileName).ToCamelCase().ToReferenceTypeValueOrThrow()] = document.FileName;
                    break;
                case nameof(IDocument.Path):
                    documentData[nameof(IDocument.Path).ToLowerInvariant()] = document.Path;
                    break;
                case nameof(IDocument.TemplateId):
                    documentData[nameof(IDocument.TemplateId).ToCamelCase().ToReferenceTypeValueOrThrow()] = document.TemplateId;
                    break;
                case nameof(IDocument.SegmentId):
                    documentData[nameof(IDocument.SegmentId).ToCamelCase().ToReferenceTypeValueOrThrow()] = document.SegmentId;
                    break;
                case nameof(IDocument.IsRoot):
                    documentData[nameof(IDocument.IsRoot).ToCamelCase().ToReferenceTypeValueOrThrow()] = document.IsRoot;
                    break;
                case nameof(IDocument.IsActive):
                    documentData[nameof(IDocument.IsActive).ToCamelCase().ToReferenceTypeValueOrThrow()] = document.IsActive;
                    break;
                case nameof(IDocument.SortOrdinal):
                    documentData[nameof(IDocument.SortOrdinal).ToCamelCase().ToReferenceTypeValueOrThrow()] = document.SortOrdinal;
                    break;
                case nameof(IDocument.ClientId):
                    documentData[nameof(IDocument.ClientId).ToCamelCase().ToReferenceTypeValueOrThrow()] = document.ClientId;
                    break;
                case nameof(IDocument.EndDate):
                    documentData[nameof(IDocument.EndDate).ToCamelCase().ToReferenceTypeValueOrThrow()] =
                        document.EndDate?.ToLocalTime().ToIso8601String(includeTimeMilliseconds: false);
                    break;
                case nameof(IDocument.InceptDate):
                    documentData[nameof(IDocument.InceptDate).ToCamelCase().ToReferenceTypeValueOrThrow()] =
                        document.InceptDate?.ToLocalTime().ToIso8601String(includeTimeMilliseconds: false);
                    break;
                case nameof(IDocument.ModificationDate):
                    documentData[nameof(IDocument.ModificationDate).ToCamelCase().ToReferenceTypeValueOrThrow()] =
                        document.ModificationDate?.ToLocalTime().ToIso8601String(includeTimeMilliseconds: false);
                    break;
            }
        }
    }

    /// <summary>
    /// Updates the <see cref="JsonObject"/>
    /// with the conventional front-matter properties
    /// based on <see cref="IDocument.Tag"/>
    /// for Studio Publications.
    /// </summary>
    /// <param name="documentData">the <see cref="JsonObject"/></param>
    /// <param name="tag">the value of <see cref="IDocument.Tag"/></param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    /// <seealso cref="Publications.Extensions.IDocumentExtensions.WithConventionalFrontMatterForDocumentTag"/>
    /// <seealso cref="GetConventionalTagsAndPropertiesElements"/>
    public static void UpdateFrontMatterWithDocumentTag(JsonObject? documentData, string? tag, ILogger logger)
    {
        if (documentData == null) return;
        if(string.IsNullOrWhiteSpace(tag)) return;

        var (tagsE, propE) = GetConventionalTagsAndPropertiesElements(documentData, tag, logger);

        if(tagsE.ValueKind == JsonValueKind.Array)
        {
            documentData.WithArrayProperty(Tags);

            foreach (JsonElement jsonElement in tagsE.EnumerateArray())
            {
                documentData.AddItemToArray(Tags, jsonElement, logger);
            }
        }

        if (propE.ValueKind == JsonValueKind.Null) return;

        foreach (JsonProperty jsonProperty in propE.EnumerateObject())
        {
            documentData[jsonProperty.Name] = JsonValue.Create(jsonProperty.Value);
        }
    }

    private const string Tags = "tags";
}
