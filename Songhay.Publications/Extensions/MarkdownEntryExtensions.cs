using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Markdig;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Songhay.Diagnostics;
using Songhay.Extensions;
using Songhay.Publications.Models;

namespace Songhay.Publications.Extensions
{
    /// <summary>
    /// Extensions of <see cref="MarkdownEntry" />
    /// </summary>
    public static class MarkdownEntryExtensions
    {
        static MarkdownEntryExtensions() => traceSource = TraceSources
           .Instance
           .GetTraceSourceFromConfiguredName()
           .WithSourceLevels();

        static readonly TraceSource traceSource;

        /// <summary>
        /// Effectively validates <see cref="MarkdownEntry" />
        /// </summary>
        /// <param name="entry">the <see cref="MarkdownEntry" /> entry</param>
        public static MarkdownEntry DoNullCheck(this MarkdownEntry entry)
        {
            entry.DoNullCheckForFrontMatter();

            entry.DoNullCheckForContent();

            return entry;
        }

        /// <summary>
        /// Effectively validates <see cref="MarkdownEntry.Content" />
        /// </summary>
        /// <param name="entry">the <see cref="MarkdownEntry" /> entry</param>
        public static MarkdownEntry DoNullCheckForContent(this MarkdownEntry entry)
        {
            if (entry == null)
            {
                throw new NullReferenceException($"The expected {nameof(MarkdownEntry)} is not here.");
            }

            if (string.IsNullOrWhiteSpace(entry.Content))
            {
                throw new NullReferenceException($"The expected {nameof(MarkdownEntry.Content)} is not here.");
            }

            return entry;
        }

        /// <summary>
        /// Effectively validates <see cref="MarkdownEntry.FrontMatter" />
        /// </summary>
        /// <param name="entry">the <see cref="MarkdownEntry" /> entry</param>
        public static MarkdownEntry DoNullCheckForFrontMatter(this MarkdownEntry entry)
        {
            if (entry == null)
            {
                throw new NullReferenceException($"The expected {nameof(MarkdownEntry)} is not here.");
            }

            if (entry.FrontMatter == null)
            {
                throw new NullReferenceException($"The expected {nameof(MarkdownEntry.FrontMatter)} is not here.");
            }

            return entry;
        }

        /// <summary>
        /// Converts the <see cref="MarkdownEntry"/>
        /// to an extract of the specified length
        /// </summary>
        /// <param name="entry">the <see cref="MarkdownEntry" /></param>
        /// <param name="length">the string-length of the extract</param>
        /// <returns></returns>
        public static string ToExtract(this MarkdownEntry entry, int length)
        {
            var paragraphs = entry.ToParagraphs();
            var skip = paragraphs.Count() > 1 ? 1 : 0;
            var content = paragraphs.Skip(1).Aggregate(string.Empty, (a, i) => $"{a} {i}");
            content = Regex.Replace(content, @"<[^>]+>", string.Empty);
            content = Markdown.ToPlainText(content);

            return (content.Length > length) ?
                string.Concat(content.Substring(0, length), "�") :
                content;
        }

        /// <summary>
        /// Converts the specified <see cref="MarkdownEntry" />
        /// into the final edit <see cref="String" />
        /// </summary>
        /// <param name="entry">the <see cref="MarkdownEntry" /> entry</param>
        public static string ToFinalEdit(this MarkdownEntry entry)
        {
            entry.DoNullCheck();

            var finalEdit = string.Concat(
                "---json",
                MarkdownEntry.NewLine,
                entry.FrontMatter.ToString().Trim(),
                MarkdownEntry.NewLine,
                "---",
                MarkdownEntry.NewLine,
                MarkdownEntry.NewLine,
                entry.Content.Trim(),
                MarkdownEntry.NewLine
            );

            return finalEdit;
        }

        /// <summary>
        /// Converts the specified <see cref="FileInfo" />
        /// into <see cref="MarkdownEntry" />
        /// </summary>
        /// <param name="entry">the <see cref="FileInfo" /> entry</param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">
        /// The expected {nameof(FileInfo)} is not here.
        /// or
        /// The expected {nameof(FileInfo)} path is not here.
        /// </exception>
        /// <exception cref="FormatException">
        /// File {entry.Name} is empty.
        /// or
        /// The expected entry format is not here [front matter top].
        /// or
        /// The expected entry format is not here [front matter bottom].
        /// </exception>
        public static MarkdownEntry ToMarkdownEntry(this FileInfo entry)
        {
            if (entry == null) throw new NullReferenceException($"The expected {nameof(FileInfo)} is not here.");
            traceSource?.TraceVerbose($"converting `{entry.FullName}`...");
            if (!File.Exists(entry.FullName)) throw new NullReferenceException($"The expected {nameof(FileInfo)} path is not here.");

            var frontTop = "---json";
            var frontBottom = "---";
            var lines = File.ReadAllLines(entry.FullName);

            if (!lines.Any()) throw new FormatException($"File {entry.Name} is empty.");
            if (lines.First().Trim() != frontTop) throw new FormatException("The expected entry format is not here [front matter top].");
            if (!lines.Contains(frontBottom)) throw new FormatException("The expected entry format is not here [front matter bottom].");

            var json = lines
                .Skip(1)
                .TakeWhile(i => !i.Contains(frontBottom))
                .Aggregate((a, i) => $"{a}{MarkdownEntry.NewLine}{i}");

            var content = lines
                .SkipWhile(i => !i.Equals(frontBottom))
                .Skip(1)
                .Aggregate((a, i) => $"{a}{MarkdownEntry.NewLine}{i}");

            JObject frontMatter = JObject.FromObject(new { error = "front matter was not found", file = entry.FullName });
            try
            {
                frontMatter = JObject.Parse(json);
            }
            catch (JsonReaderException ex)
            {
                traceSource?.TraceError(ex);
            }

            var mdEntry = new MarkdownEntry
            {
                EntryFileInfo = entry,
                FrontMatter = frontMatter,
                Content = content
            };

            return mdEntry;
        }

        /// <summary>
        /// Converts <see cref="MarkdownEntry.Content" /> to paragraphs
        /// </summary>
        /// <param name="entry">the <see cref="MarkdownEntry" /> entry</param>
        public static string[] ToParagraphs(this MarkdownEntry entry)
        {
            entry.DoNullCheck();

            var delimiter = new string[] { $"{MarkdownEntry.NewLine}{MarkdownEntry.NewLine}" };

            var paragraphs = entry.Content
                .Trim()
                .Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            return paragraphs;
        }

        /// <summary>
        /// Sets the modification date of the <see cref="MarkdownEntry" />.
        /// </summary>
        /// <param name="entry">the <see cref="MarkdownEntry" /> entry</param>
        /// <param name="date">the touch <see cref="DateTime" /></param>
        /// <returns></returns>
        public static MarkdownEntry Touch(this MarkdownEntry entry, DateTime date)
        {
            return entry.WithEdit(i =>
            {
                i.DoNullCheckForFrontMatter();

                var propertyName = "modificationDate";

                if (!i.FrontMatter.HasProperty(propertyName))
                    throw new FormatException($"The expected date property, `{propertyName ?? "[null]"}`, is not here.");

                i.FrontMatter[propertyName] = ProgramTypeUtility.ConvertDateTimeToUtc(date);
            });
        }

        /// <summary>
        /// Returns the <see cref="MarkdownEntry"/>
        /// with the conventional eleventy extract
        /// of the specified length.
        /// </summary>
        /// <param name="entry">the <see cref="MarkdownEntry" /></param>
        /// <param name="length">the string-length of the extract</param>
        /// <returns></returns>
        public static MarkdownEntry With11tyExtract(this MarkdownEntry entry, int length)
        {
            entry.WithEdit(i =>
            {
                string UpdateExtractAndReturnTag(string tag, string e)
                {
                    var extractPropertyName = "extract";

                    var jO = tag.TrimStart().StartsWith("{") ?
                        JObject.Parse(tag) :
                        JObject.FromObject(new { legacy = tag });

                    if (!jO.HasProperty(extractPropertyName))
                    {
                        jO.Add(extractPropertyName, null);
                    }

                    jO[extractPropertyName] = e;
                    return jO.ToString();
                }

                var tagPropertyName = "tag";
                var tagString = i.FrontMatter
                    .GetValue<string>(tagPropertyName, throwException: false);

                var extract = i.ToExtract(length);

                i.FrontMatter[tagPropertyName] = string.IsNullOrWhiteSpace(tagString) ?
                    JObject.FromObject(new { extract }).ToString()
                    :
                    UpdateExtractAndReturnTag(tagString, extract)
                    ;
            });

            return entry;
        }

        /// <summary>
        /// Returns the <see cref="MarkdownEntry"/>
        /// based on <see cref="MarkdownEntry.FrontMatter"/>
        /// with a <c>title</c> property.
        /// </summary>
        /// <param name="entry">the <see cref="MarkdownEntry" /> entry</param>
        /// <returns></returns>
        public static MarkdownEntry WithContentHeader(this MarkdownEntry entry)
        {
            return entry.WithContentHeader(headerLevel: 1);
        }

        /// <summary>
        /// Returns the <see cref="MarkdownEntry"/>
        /// based on <see cref="MarkdownEntry.FrontMatter"/>
        /// with a <c>title</c> property.
        /// </summary>
        /// <param name="entry">the <see cref="MarkdownEntry" /> entry</param>
        /// <param name="headerLevel"></param>
        /// <returns></returns>
        public static MarkdownEntry WithContentHeader(this MarkdownEntry entry, int headerLevel)
        {
            entry.DoNullCheckForFrontMatter();

            var propertyName = "title";

            if (!entry.FrontMatter.HasProperty(propertyName))
                throw new FormatException($"The expected date property, `{propertyName ?? "[null]"}`, is not here.");

            headerLevel = (headerLevel == 0) ? 1 : Math.Abs(headerLevel);
            var markdownHeader = new string(Enumerable.Repeat('#', (headerLevel > 6) ? 6 : headerLevel).ToArray());
            return entry.WithEdit(i => i.Content = $"{markdownHeader} {i.FrontMatter[propertyName]}{MarkdownEntry.NewLine}{MarkdownEntry.NewLine}");
        }

        /// <summary>
        /// Edits the <see cref="MarkdownEntry" /> with the specified edit action.
        /// </summary>
        /// <param name="entry">the <see cref="MarkdownEntry" /></param>
        /// <param name="editAction">the edit <see cref="Action{MarkdownEntry}" /></param>
        public static MarkdownEntry WithEdit(this MarkdownEntry entry, Action<MarkdownEntry> editAction)
        {
            editAction?.Invoke(entry);
            return entry;
        }

        /// <summary>
        /// Returns the <see cref="MarkdownEntry" /> with conventional 11ty frontmatter.
        /// </summary>
        /// <param name="entry">the <see cref="MarkdownEntry" /> entry</param>
        /// <param name="title">the title of th entry</param>
        /// <param name="inceptDate">the incept date of the entry</param>
        /// <param name="path">the path to the entry</param>
        /// <param name="tag">the tag of the entry</param>
        /// <returns></returns>
        public static MarkdownEntry WithNew11tyFrontMatter(this MarkdownEntry entry, string title, DateTime inceptDate, string path, string tag)
        {
            return entry.WithNewFrontMatter(title, inceptDate,
                documentId: 0, fileName: "index.html", path: path, segmentId: 0, tag: tag)
                .WithEdit(i => i.FrontMatter["clientId"] = $"{inceptDate.ToString("yyyy-MM-dd")}-{i.FrontMatter["clientId"]}")
                .WithEdit(i => i.FrontMatter["documentShortName"] = i.FrontMatter["clientId"])
                .WithEdit(i => i.FrontMatter["path"] = $"{i.FrontMatter["path"]}{i.FrontMatter["clientId"]}");
        }

        /// <summary>
        /// Returns the <see cref="MarkdownEntry" /> with conventional frontmatter.
        /// </summary>
        /// <param name="entry">the <see cref="MarkdownEntry" /> entry</param>
        /// <param name="title">the title of th entry</param>
        /// <param name="inceptDate">the incept date of the entry</param>
        /// <param name="documentId">the DBMS ID of the entry</param>
        /// <param name="fileName">the file name (with extension) of the entry</param>
        /// <param name="path">the path to the entry</param>
        /// <param name="segmentId">the DBMS ID of the Publications Segment</param>
        /// <param name="tag">the tag of the entry</param>
        /// <returns></returns>
        public static MarkdownEntry WithNewFrontMatter(this MarkdownEntry entry, string title, DateTime inceptDate, int documentId, string fileName, string path, int segmentId, string tag)
        {
            if (entry == null)
            {
                throw new NullReferenceException($"The expected {nameof(MarkdownEntry)} is not here.");
            }

            var slug = title.ToBlogSlug();

            var fm = new
            {
                documentId,
                title,
                documentShortName = slug,
                fileName,
                path,
                date = ProgramTypeUtility.ConvertDateTimeToUtc(inceptDate),
                modificationDate = ProgramTypeUtility.ConvertDateTimeToUtc(inceptDate),
                templateId = 0,
                segmentId,
                isRoot = false,
                isActive = true,
                sortOrdinal = 0,
                clientId = slug,
                tag
            };

            entry.FrontMatter = JObject.FromObject(fm);

            return entry;
        }
    }
}