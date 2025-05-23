using FluentValidation;

namespace Songhay.Publications.Validators;

/// <summary>
/// Validator for <see cref="Segment"/>.
/// </summary>
public class SegmentValidator : AbstractValidator<Segment>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SegmentValidator"/> class.
    /// </summary>
    public SegmentValidator()
    {
        RuleFor(i => i.IsActive)
            .NotNull()
            .WithMessage(PublicationAppScalars.ValidationMessageRequired);
        RuleFor(i => i.SegmentId)
            .NotNull()
            .WithMessage(PublicationAppScalars.ValidationMessageRequired);
        RuleFor(i => i.SegmentName)
            .NotNull()
            .WithMessage(PublicationAppScalars.ValidationMessageRequired);

        RuleFor(i => i).SetValidator(new ITemporalValidator());
    }
}
