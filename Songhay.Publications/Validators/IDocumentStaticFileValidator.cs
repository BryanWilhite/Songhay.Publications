using FluentValidation;

namespace Songhay.Publications.Validators;

/// <summary>
/// Validator for ‘static’ <see cref="IDocument"/>
/// </summary>
// ReSharper disable once InconsistentNaming
public class IDocumentStaticFileValidator :AbstractValidator<IDocument>
{
    /// <inheritdoc/>
    public IDocumentStaticFileValidator()
    {
        Include(new IDocumentCoreValidator());

        RuleFor(data => data.SegmentId).NotNull();
        RuleFor(data => data.Path).NotEmpty();
        RuleFor(data => data.FileName).NotEmpty();
    }
}
