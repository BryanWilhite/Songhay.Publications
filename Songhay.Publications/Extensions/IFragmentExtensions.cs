﻿using CloneExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Songhay.Extensions;
using Songhay.Publications.Models;
using Songhay.Models;
using System;
using System.Text;

namespace Songhay.Publications.Extensions
{

    /// <summary>
    /// Extensions of <see cref="IFragment"/>
    /// </summary>
    public static class IFragmentExtensions
    {
        /// <summary>
        /// Clones the instance of <see cref="IFragment"/>.
        /// </summary>
        /// <param name="data">The document.</param>
        /// <returns><see cref="Fragment"/></returns>
        public static Fragment Clone(this IFragment data)
        {
            return data?.GetClone(CloneInitializers.Publications) as Fragment;
        }

        /// <summary>
        /// Sets the defaults.
        /// </summary>
        /// <param name="data">The fragment.</param>
        public static void SetDefaults(this IFragment data)
        {
            if (data == null) return;

            data.InceptDate = DateTime.Now;
            data.IsActive = true;
            data.ModificationDate = DateTime.Now;
        }

        /// <summary>
        /// Converts the <see cref="IFragment"/> into a display text.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static string ToDisplayText(this IFragment data)
        {
            if (data == null) return null;

            var builder = new StringBuilder($"{nameof(data.FragmentId)}: {data?.FragmentId}");
            builder.Append($", {nameof(data.FragmentName)}: {data?.FragmentName}");
            builder.Append($", {nameof(data.IsActive)}: {data?.IsActive}");
            builder.Append($", {nameof(data.DocumentId)}: {data?.DocumentId}");

            if (!string.IsNullOrEmpty(data.FragmentDisplayName)) builder.Append($", {nameof(data.FragmentDisplayName)}: {data?.FragmentDisplayName}");
            if (!string.IsNullOrEmpty(data.Content)) builder.Append($", {nameof(data.Content)}: {data?.Content.Truncate(32)}");

            builder.Append($", {nameof(data.PrevFragmentId)}: {data?.PrevFragmentId}");
            builder.Append($", {nameof(data.NextFragmentId)}: {data?.NextFragmentId}");
            builder.Append($", {nameof(data.IsNext)}: {data?.IsNext}");
            builder.Append($", {nameof(data.IsPrevious)}: {data?.IsPrevious}");
            builder.Append($", {nameof(data.IsWrapper)}: {data?.IsWrapper}");

            return builder.ToString();
        }

        /// <summary>
        /// Converts the <see cref="IFragment" /> to <see cref="JObject" />.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="useJavaScriptCase">when <c>true</c> use “camel” casing.</param>
        /// <returns></returns>
        public static JObject ToJObject(this IFragment data, bool useJavaScriptCase)
        {
            if (data == null) return null;

            var settings = JsonSerializationUtility
                .GetConventionalResolver<IFragment>(useJavaScriptCase)
                .ToJsonSerializerSettings();

            var jO = JObject.FromObject(data, JsonSerializer.Create(settings));

            return jO;
        }

        /// <summary>
        /// Converts the <see cref="IFragment"/> into a menu display item model.
        /// </summary>
        /// <param name="data">The fragment.</param>
        public static MenuDisplayItemModel ToMenuDisplayItemModel(this IFragment data)
        {
            return data.ToMenuDisplayItemModel(group: null, copyFragmentContent: false);
        }

        /// <summary>
        /// Converts the <see cref="IFragment" /> into a menu display item model.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="copyFragmentContent">if set to <c>true</c> include <see cref="IFragment" /> content.</param>
        /// <returns></returns>
        public static MenuDisplayItemModel ToMenuDisplayItemModel(this IFragment data, bool copyFragmentContent)
        {
            return data.ToMenuDisplayItemModel(group: null, copyFragmentContent: copyFragmentContent);
        }

        /// <summary>
        /// Converts the <see cref="IFragment"/> into a menu display item model.
        /// </summary>
        /// <param name="data">The fragment.</param>
        /// <param name="group">The group.</param>
        public static MenuDisplayItemModel ToMenuDisplayItemModel(this IFragment data, IGroupable group)
        {
            return data.ToMenuDisplayItemModel(group, copyFragmentContent: false);
        }

        /// <summary>
        /// Converts the <see cref="IFragment"/> into a menu display item model.
        /// </summary>
        /// <param name="data">The document.</param>
        /// <param name="group">The group.</param>
        /// <param name="copyFragmentContent">if set to <c>true</c> include <see cref="IFragment"/> content.</param>
        public static MenuDisplayItemModel ToMenuDisplayItemModel(this IFragment data, IGroupable group, bool copyFragmentContent)
        {
            if (data == null) return null;

            var @namespace = typeof(PublicationContext).Namespace;

            var dataOut = new MenuDisplayItemModel()
            {
                DisplayText = data.FragmentDisplayName,
                GroupDisplayText = (group == null) ? $"{@namespace}.{nameof(Fragment)}" : group.GroupDisplayText,
                GroupId = (group == null) ? $"{@namespace}.{nameof(Fragment)}".ToLowerInvariant() : group.GroupId,
                Id = data.FragmentId,
                ItemName = data.FragmentName
            };
            if (copyFragmentContent) dataOut.Description = data.Content;
            return dataOut;
        }

        /// <summary>
        /// Returns <see cref="IFragment"/> with default values.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static IFragment WithDefaults(this IFragment data)
        {
            data.SetDefaults();
            return data;
        }

        /// <summary>
        /// Returns <see cref="IFragment" />
        /// after the specified edit <see cref="Action{IFragment}" />.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="editAction">The edit action.</param>
        /// <returns></returns>
        public static IFragment WithEdit(this IFragment data, Action<IFragment> editAction)
        {
            editAction?.Invoke(data);
            return data;
        }

    }
}
