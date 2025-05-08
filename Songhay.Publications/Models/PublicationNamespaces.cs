namespace Songhay.Publications.Models;

/// <summary>
/// Centralizes XML namespaces
/// primarily for EPUB. pipelines.
/// </summary>
public static class PublicationNamespaces
{
    /// <summary>
    /// Conventional namespace for EPUB.
    /// </summary>
    public static readonly XNamespace DaisyNcx = "http://www.daisy.org/z3986/2005/ncx/";

    /// <summary>
    /// Conventional namespace for EPUB.
    /// </summary>
    public static readonly XNamespace DublinCore = "http://purl.org/dc/elements/1.1/";

    /// <summary>
    /// Conventional namespace for EPUB.
    /// </summary>
    public static readonly XNamespace IdpfOpenPackagingFormat = "http://www.idpf.org/2007/opf";

    /// <summary>
    /// Conventional namespace for EPUB.
    /// </summary>
    public static readonly XNamespace IdpfOpenPackagingStructure = "http://www.idpf.org/2007/ops";

    /// <summary>
    /// Conventional namespace for EPUB.
    /// </summary>
    public static readonly XNamespace Xhtml = "http://www.w3.org/1999/xhtml";

    /// <summary>
    /// Conventional namespace for EPUB.
    /// </summary>
    public static XNamespace Xml = "http://www.w3.org/XML/1998/namespace";
}