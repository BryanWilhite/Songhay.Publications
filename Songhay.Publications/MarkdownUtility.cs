namespace Songhay.Publications;

/// <summary>
/// Shared routines for Markdown
/// </summary>
public static class MarkdownUtility
{
    /// <summary>
    /// Converts a <c>pre</c> element
    /// into EPUB-friendly HTML.
    /// </summary>
    /// <param name="preElement">The <c>pre</c> <see cref="XElement"/>.</param>
    public static void ConvertPreBlockToHtml(XElement preElement)
    {
        List<string> preList = GetPreLines(preElement);
        preElement.RemoveNodes();

        string fourSpaces = GetFourSpaces();
        string fourSpacesToken = GetFourSpacesToken();
        preList.ForEach(line =>
        {
            string p = Markdown.ToHtml(line.Replace(fourSpaces, fourSpacesToken));
            if (!IsMarkdownParagraph(p)) p = GetPElementWithNewLine();

            p = p.Replace(fourSpacesToken, fourSpaces);
            XElement pElement = XElement.Parse(p);
            pElement.Add(Environment.NewLine);
            preElement.Add(pElement.Nodes());
        });
    }

    /// <summary>
    /// Returns a conventional number of space characters.
    /// </summary>
    public static string GetFourSpaces() => new(Enumerable.Repeat(' ', 4).ToArray());

    /// <summary>
    /// Returns a conventional token representing four space characters.
    /// </summary>
    /// <remarks>
    /// This is a trick used for automating find-change operations.
    /// </remarks>
    public static string GetFourSpacesToken() => "rx:four-spaces";

    /// <summary>
    /// Returns a <c>p</c> element
    /// delimited by <see cref="Environment.NewLine"/>.
    /// </summary>
    public static string GetPElementWithNewLine() => string.Concat("<p>", Environment.NewLine, "</p>");

    /// <summary>
    /// Decomposes the <see cref="XElement.Value"/>
    /// of the specified <c>pre</c> element.
    /// </summary>
    /// <param name="preElement">The <c>pre</c> <see cref="XElement"/>.</param>
    /// <remarks>
    /// Note: the first and last elements of preList
    /// *should* be empty(when no leading spaces before<pre />)
    /// or *should* contain `pre` open and closing, respectively
    /// (when there are leading spaces before <pre />).
    /// </remarks>
    public static List<string> GetPreLines(XElement preElement)
    {
        List<string> preList = preElement.Value
            .Split(Environment.NewLine.ToCharArray())
            .ToList();
        preList.RemoveAt(0);
        preList.RemoveAt(preList.Count - 1);

        return preList;
    }

    /// <summary>
    /// Returns <c>true</c> when a block of text
    /// starts with a <c>p</c> element.
    /// </summary>
    /// <param name="input">The <see cref="string"/> input.</param>
    public static bool IsMarkdownParagraph(string input) => input.StartsWith("<p>");
}
