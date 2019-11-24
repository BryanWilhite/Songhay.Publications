using Markdig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Songhay.Publications
{
    /// <summary>
    /// Shared routines for Markdown
    /// </summary>
    public static class MarkdownUtility
    {
        /// <summary>
        /// Converts a <c>pre</c> element
        /// into EPUB-friendly HTML.
        /// </summary>
        /// <param name="preElement"></param>
        public static void ConvertPreBlockToHtml(XElement preElement)
        {
            var preList = GetPreLines(preElement);
            preElement.RemoveNodes();

            var fourSpaces = GetFourSpaces();
            var fourSpacesToken = GetFourSpacesToken();
            preList.ForEach(line =>
            {
                var p = Markdown.ToHtml(line.Replace(fourSpaces, fourSpacesToken));
                if (!IsMarkdownParagraph(p)) p = GetPElementWithNewLine();

                p = p.Replace(fourSpacesToken, fourSpaces);
                var pElement = XElement.Parse(p);
                pElement.Add(Environment.NewLine);
                preElement.Add(pElement.Nodes());
            });
        }

        /// <summary>
        /// Returns a conventional number of space characters.
        /// </summary>
        /// <returns></returns>
        public static string GetFourSpaces()
        {
            return new string(Enumerable.Repeat(' ', 4).ToArray());
        }

        /// <summary>
        /// Returns a conventional token representing four space characters.
        /// </summary>
        /// <remarks>
        /// This is a trick used for automating find-change operations.
        /// </remarks>
        /// <returns></returns>
        public static string GetFourSpacesToken()
        {
            return "rx:four-spaces";
        }

        /// <summary>
        /// Returns a <c>p</c> element
        /// delimited by <see cref="Environment.NewLine"/>.
        /// </summary>
        /// <returns></returns>
        public static string GetPElementWithNewLine()
        {
            return string.Concat("<p>", Environment.NewLine, "</p>");
        }

        /// <summary>
        /// Decomposes the <see cref="XElement.Value"/>
        /// of the specified <c>pre</c> element.
        /// </summary>
        /// <param name="preElement"><c>pre</c> element</param>
        /// <remarks>
        /// Note: the first and last elements of preList
        /// *should* be empty(when no leading spaces before<pre />)
        /// or *should* contain `pre` open and closing, respectively
        /// (when there are leading spaces before <pre />).
        /// </remarks>
        /// <returns></returns>
        public static List<string> GetPreLines(XElement preElement)
        {
            var preList = preElement.Value
                .Split(Environment.NewLine.ToCharArray())
                .ToList();
            preList.RemoveAt(0);
            preList.RemoveAt(preList.Count() - 1);

            return preList;
        }

        /// <summary>
        /// Returns <c>true</c> when a block of text
        /// starts with a <c>p</c> element.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool IsMarkdownParagraph(string p)
        {
            return p.StartsWith("<p>");
        }
    }
}
