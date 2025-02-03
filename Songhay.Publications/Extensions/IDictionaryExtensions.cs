namespace Songhay.Publications.Extensions;

/// <summary>
/// Extensions of <see cref="IDictionary{TKey,TValue}"/>
/// </summary>
// ReSharper disable once InconsistentNaming
public static class IDictionaryExtensions
{
    /// <summary>
    /// Converts the specified <see cref="IDictionary{TKey,TValue}"/> to a JSON string.
    /// </summary>
    /// <param name="data">the <see cref="IDictionary{TKey,TValue}"/></param>
    public static string? ToJsonString(this IDictionary<string, object>? data)
    {
        ISerializer serializer = new SerializerBuilder()
            .JsonCompatible()
            .Build();

        string json = serializer.Serialize(data);

        return json;
    }

    /// <summary>
    /// Converts the specified <see cref="IDictionary{TKey,TValue}"/> to <see cref="IDocument"/>
    /// </summary>
    /// <param name="data">the <see cref="IDictionary{TKey,TValue}"/></param>
    /// <param name="logger">the <see cref="ILogger"/></param>
    /// <param name="tagKeys">the keys to search for tagging the <see cref="IDocument"/></param>
    /// <remarks>
    /// Tagging the <see cref="IDocument"/> sets <see cref="IDocument.Tag"/>
    /// with serialized JSON key-value pairs from:
    /// - the conventional key, <c>extract</c>
    /// - keys specified in <c>tagKeys</c>
    /// </remarks>
    public static IDocument? ToTaggedDocument(this IDictionary<string, object>? data, ILogger? logger, params string[] tagKeys)
    {
        if (data == null)
        {
            logger?.LogWarning("Warning: the expected data is not here. Returning null...");

            return null;
        }

        IDocument document = new Document();

        #region read Document properties manually:

        string propertyName = nameof(document.DocumentId).ToCamelCase().ToReferenceTypeValueOrThrow();
        logger?.LogInformation("Trying to get `{Name}`...", propertyName);
        int? documentId = ProgramTypeUtility.ParseInt32(data.TryGetValueWithKey(propertyName));
        if (documentId == null)
        {
            logger?.LogError("Error: the expected property, `{Name}`, is not here. This is a key! Continuing...", propertyName);
        }
        else document.DocumentId = documentId;

        propertyName = nameof(document.ClientId).ToCamelCase().ToReferenceTypeValueOrThrow();
        logger?.LogInformation("Trying to get `{Name}`...", propertyName);
        string? clientId = (string?)data.TryGetValueWithKey(propertyName);
        if (clientId == null)
        {
            logger?.LogWarning("Warning: the expected property, `{Name}`, is not here. This is a key! Continuing...", propertyName);
        }
        else document.ClientId = clientId;

        propertyName = nameof(document.DocumentShortName).ToCamelCase().ToReferenceTypeValueOrThrow();
        logger?.LogInformation("Trying to get `{Name}`...", propertyName);
        document.DocumentShortName = (string?)data.TryGetValueWithKey(propertyName);

        propertyName = nameof(document.FileName).ToCamelCase().ToReferenceTypeValueOrThrow();
        logger?.LogInformation("Trying to get `{Name}`...", propertyName);
        document.FileName = (string?)data.TryGetValueWithKey(propertyName);

        propertyName = nameof(document.EndDate).ToCamelCase().ToReferenceTypeValueOrThrow();
        logger?.LogInformation("Trying to get `{Name}`...", propertyName);
        document.EndDate = ProgramTypeUtility.ParseDateTime(data.TryGetValueWithKey(propertyName));

        propertyName = nameof(document.InceptDate).ToCamelCase().ToReferenceTypeValueOrThrow();
        logger?.LogInformation("Trying to get `{Name}`...", propertyName);
        document.InceptDate = ProgramTypeUtility.ParseDateTime(data.TryGetValueWithKey(propertyName));

        propertyName = nameof(document.IsActive).ToCamelCase().ToReferenceTypeValueOrThrow();
        logger?.LogInformation("Trying to get `{Name}`...", propertyName);
        document.IsActive = ProgramTypeUtility.ParseBoolean(data.TryGetValueWithKey(propertyName));

        propertyName = nameof(document.IsRoot).ToCamelCase().ToReferenceTypeValueOrThrow();
        logger?.LogInformation("Trying to get `{Name}`...", propertyName);
        document.IsRoot = ProgramTypeUtility.ParseBoolean(data.TryGetValueWithKey(propertyName));

        propertyName = nameof(document.ModificationDate).ToCamelCase().ToReferenceTypeValueOrThrow();
        logger?.LogInformation("Trying to get `{Name}`...", propertyName);
        document.ModificationDate = ProgramTypeUtility.ParseDateTime(data.TryGetValueWithKey(propertyName));

        propertyName = nameof(document.Path).ToCamelCase().ToReferenceTypeValueOrThrow();
        logger?.LogInformation("Trying to get `{Name}`...", propertyName);
        document.Path = (string?)data.TryGetValueWithKey(propertyName);

        propertyName = nameof(document.SegmentId).ToCamelCase().ToReferenceTypeValueOrThrow();
        logger?.LogInformation("Trying to get `{Name}`...", propertyName);
        document.SegmentId = ProgramTypeUtility.ParseInt32(data.TryGetValueWithKey(propertyName));

        propertyName = nameof(document.TemplateId).ToCamelCase().ToReferenceTypeValueOrThrow();
        logger?.LogInformation("Trying to get `{Name}`...", propertyName);
        document.TemplateId = ProgramTypeUtility.ParseInt32(data.TryGetValueWithKey(propertyName));

        propertyName = nameof(document.Title).ToCamelCase().ToReferenceTypeValueOrThrow();
        logger?.LogInformation("Trying to get `{Name}`...", propertyName);
        document.Title = (string?)data.TryGetValueWithKey(propertyName);

        #endregion

        logger?.LogInformation("Preparing to set IDocument.Tag...");

        var node = JsonNode.Parse("{}");
        if (node == null) return document;

        JsonObject jO = node.AsObject();

        propertyName = "extract";
        logger?.LogInformation("Trying to get `{Name}` for IDocument.Tag...", propertyName);
        jO[propertyName] = (string?)data.TryGetValueWithKey(propertyName);
        foreach (string key in tagKeys.Distinct())
        {
            logger?.LogInformation("Trying to get `{Name}` for IDocument.Tag...", key);
            jO[key] = (string?)data.TryGetValueWithKey(key);
        }

        document.Tag = jO.ToJsonString();

        return document;
    }
}
