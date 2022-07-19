namespace Songhay.Publications.Models;

/// <summary>
/// Centralizes the EPUB files.
/// </summary>
public static class PublicationFiles
{
    /// <summary>
    /// EPUB DAISY Consortium Navigation Control file.
    /// </summary>
    /// <remarks>
    /// “The NCX file (Navigation Control file for XML),
    /// traditionally named toc.ncx, contains the hierarchical table
    /// of contents for the EPUB file.”
    /// 
    /// https://en.wikipedia.org/wiki/EPUB
    /// https://web.archive.org/web/20090831133730/http://www.niso.org/workrooms/daisy/Z39-86-2005.html#NCX
    /// </remarks>
    public const string DaisyConsortiumNcxToc = "toc.ncx";

    /// <summary>
    /// EPUB XHTML template output file.
    /// </summary>
    public const string EpubFileBiography = "biography.xhtml";

    /// <summary>
    /// EPUB XHTML template output file.
    /// </summary>
    public const string EpubFileCopyright = "copyright.xhtml";

    /// <summary>
    /// EPUB XHTML template output file.
    /// </summary>
    public const string EpubFileDedication = "dedication.xhtml";

    /// <summary>
    /// EPUB XHTML template output file.
    /// </summary>
    public const string EpubFileTitle = "title.xhtml";

    /// <summary>
    /// EPUB XHTML template output file.
    /// </summary>
    public const string EpubFileToc = "toc.xhtml";

    /// <summary>
    /// EPUB XHTML template input file.
    /// </summary>
    public const string EpubMarkdownBiography = "author-biography.md";

    /// <summary>
    /// EPUB XHTML template input file.
    /// </summary>
    public const string EpubMarkdownDedication = "author-dedication.md";

    /// <summary>
    /// Conventional Songhay System metadata for EPUB publications.
    /// </summary>
    public const string EpubMetadata = "publication-meta.json";

    /// <summary>
    /// EPUB XHTML template file.
    /// </summary>
    public const string EpubTemplateBiography = "biography-template.xhtml";

    /// <summary>
    /// EPUB XHTML template file.
    /// </summary>
    public const string EpubTemplateChapter = "chapter-template.xhtml";

    /// <summary>
    /// EPUB XHTML template file.
    /// </summary>
    public const string EpubTemplateDedication = "dedication-template.xhtml";

    /// <summary>
    /// International Digital Publishing Forum
    /// Open Packaging Format manifest.
    /// </summary>
    /// <remarks>
    /// “Open Packaging Format (OPF) 2.0.1, describes the structure
    /// of the .epub file in XML…
    /// The OPF file, traditionally named content.opf,
    /// houses the EPUB book’s metadata, file manifest,
    /// and linear reading order.”
    /// 
    /// https://en.wikipedia.org/wiki/EPUB
    /// </remarks>
    public const string IdpfcOpfManifest = "content.opf";
}
