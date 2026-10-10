using FluentValidation;

namespace Songhay.Publications.Validators;

/// <summary>
/// Validator for ‘core’ <see cref="IDocument"/> properties
/// </summary>
// ReSharper disable once InconsistentNaming
public class IDocumentCoreValidator : AbstractValidator<IDocument>
{
    /// <inheritdoc/>
    public IDocumentCoreValidator()
    {
        RuleFor(data => data.DocumentId).NotNull();
        RuleFor(data => data.Title).NotEmpty();

        Include(new ITemporalUtcValidator());
    }
}
