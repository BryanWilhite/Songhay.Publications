using FluentValidation;

namespace Songhay.Publications.Validators;

/// <summary>
/// Validator for <see cref="Fragment"/>.
/// </summary>
public class FragmentValidator : AbstractValidator<Fragment>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FragmentValidator"/> class.
    /// </summary>
    public FragmentValidator()
    {
        RuleFor(i => i.IsActive)
            .NotNull()
            .WithMessage(PublicationAppScalars.ValidationMessageRequired);
        RuleFor(i => i.DocumentId)
            .NotNull()
            .WithMessage(PublicationAppScalars.ValidationMessageRequired);
        RuleFor(i => i.FragmentName)
            .NotNull()
            .WithMessage(PublicationAppScalars.ValidationMessageRequired);

        RuleFor(i => i).SetValidator(new ITemporalValidator());
    }
}
