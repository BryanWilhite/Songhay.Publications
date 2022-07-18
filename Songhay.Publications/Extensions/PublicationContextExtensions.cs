namespace Songhay.Publications.Extensions;

/// <summary>
/// Extensions of <see cref="PublicationContext"/>.
/// </summary>
public static class PublicationContextExtensions
{
    /// <summary>
    /// Generates an EPUB publication
    /// in the specified <see cref="PublicationContext"/>.
    /// </summary>
    /// <param name="context">The <see cref="PublicationContext"/>.</param>
    public static void GenerateEpub(this PublicationContext context)
    {
        if (context == null)
            throw new NullReferenceException($"The expected {nameof(PublicationContext)} is not here.");

        context.GenerateMeta();
        context.GenerateChapters();
        context.WriteTitle();
        context.WriteToc();
        context.WriteCopyright();
        context.WriteBiography();
        context.WriteDedication();
    }
}
