using CloneExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Songhay.Extensions;
using Songhay.Models;
using Songhay.Publications.Models;
using System;
using System.Text;
using System.Xml.Linq;

namespace Songhay.Publications.Extensions
{
    /// <summary>
    /// Extensions of <see cref="IDocument"/>
    /// </summary>
    public static class IDocumentExtensions
    {
        /// <summary>
        /// Clones the instance of <see cref="IDocument"/>.
        /// </summary>
        /// <param name="data">The document.</param>
        /// <returns><see cref="Document"/></returns>
        public static Document Clone(this IDocument data)
        {
            return data?.GetClone(CloneInitializers.Publications) as Document;
        }

        /// <summary>
        /// Determines whether the specified document is template-able.
        /// </summary>
        /// <param name="data">The document.</param>
        /// <returns>
        ///   <c>true</c> if the specified document is template-able; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsTemplatable(this IDocument data)
        {
            if (data == null) return false;
            if (string.IsNullOrEmpty(data.FileName)) return false;
            return data.FileName.EndsWith(".html");
        }

        /// <summary>
        /// Sets the defaults.
        /// </summary>
        /// <param name="data">The document.</param>
        public static void SetDefaults(this IDocument data)
        {
            if (data == null) return;

            data.InceptDate = DateTime.Now;
            data.IsActive = true;
            data.IsRoot = false;
            data.ModificationDate = DateTime.Now;
        }

        /// <summary>
        /// Converts <see cref="Document"/> to the conventional publication item.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="templateFileName">Name of the template file.</param>
        /// <returns></returns>
        public static XElement ToConventionalPublicationItem(this IDocument data, string templateFileName)
        {
            if (data == null) return null;

            return new XElement("item",
                new XAttribute(nameof(Document.SegmentId), data.SegmentId),
                new XAttribute(nameof(Document.DocumentId), data.DocumentId),
                new XAttribute(nameof(Document.Title), data.Title),
                new XAttribute("Template", templateFileName),
                new XAttribute("PathAndFileName", string.Concat(data.Path, data.FileName)),
                new XAttribute(nameof(Document.IsRoot), data.IsRoot));
        }

        /// <summary>
        /// Converts the <see cref="IDocument"/> into a display text.
        /// </summary>
        /// <param name="data">The data.</param>
        public static string ToDisplayText(this IDocument data)
        {
            if (data == null) return null;
            var sb = new StringBuilder($"{nameof(data.DocumentId)}: {data.DocumentId}");
            sb.Append($", {nameof(data.Title)}: {data.Title}");
            sb.Append($", {nameof(data.IsActive)}: {data.IsActive}");
            sb.Append($", {nameof(data.IsRoot)}: { data.IsRoot}");
            if (!string.IsNullOrEmpty(data.FileName)) sb.Append($", {nameof(data.Path)}:{data.Path}, {nameof(data.FileName)}: {data.FileName}");
            if (!string.IsNullOrEmpty(data.DocumentShortName)) sb.Append($", {nameof(data.DocumentShortName)}: {data.DocumentShortName}");
            return sb.ToString();
        }

        /// <summary>
        /// Converts the <see cref="IDocument" /> to <see cref="JObject" />.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="useJavaScriptCase">when <c>true</c> use “camel” casing.</param>
        /// <returns></returns>
        public static JObject ToJObject(this IDocument data, bool useJavaScriptCase)
        {
            if (data == null) return null;

            var settings = JsonSerializationUtility
                .GetConventionalResolver<IDocument>(useJavaScriptCase)
                .ToJsonSerializerSettings();

            var jO = JObject.FromObject(data, JsonSerializer.Create(settings));
            return jO;
        }

        /// <summary>
        /// Converts the <see cref="IDocument"/> into a menu display item model.
        /// </summary>
        /// <param name="data">The document.</param>
        public static MenuDisplayItemModel ToMenuDisplayItemModel(this IDocument data)
        {
            return data.ToMenuDisplayItemModel(group: null);
        }

        /// <summary>
        /// Converts the <see cref="IDocument"/> into a menu display item model.
        /// </summary>
        /// <param name="data">The document.</param>
        /// <param name="group">The group.</param>
        public static MenuDisplayItemModel ToMenuDisplayItemModel(this IDocument data, IGroupable group)
        {
            if (data == null) return null;

            var dataOut = new MenuDisplayItemModel()
            {
                DisplayText = data.Title,
                GroupDisplayText = (group == null) ? MenuDisplayItemModelGroups.GenericWebDocument : group.GroupDisplayText,
                GroupId = (group == null) ? MenuDisplayItemModelGroups.GenericWebDocument.ToLowerInvariant() : group.GroupId,
                Id = data.DocumentId,
                ItemName = data.DocumentShortName
            };

            return dataOut;
        }

        /// <summary>
        /// Returns <see cref="IDocument"/> with default values.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static IDocument WithDefaults(this IDocument data)
        {
            data.SetDefaults();
            return data;
        }

        /// <summary>
        /// Returns <see cref="IDocument" />
        /// after the specified edit <see cref="Action{IDocument}" />.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="editAction">The edit action.</param>
        /// <returns></returns>
        public static IDocument WithEdit(this IDocument data, Action<IDocument> editAction)
        {
            editAction?.Invoke(data);
            return data;
        }
    }
}
