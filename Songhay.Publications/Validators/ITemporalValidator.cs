namespace Songhay.Publications.Validators;

/// <summary>
/// Validator for <see cref="ITemporal"/> implementors.
/// </summary>
// ReSharper disable once InconsistentNaming
public class ITemporalValidator : AbstractValidator<ITemporal>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ITemporalValidator"/> class.
    /// </summary>
    public ITemporalValidator()
    {
        RuleFor(i => i.InceptDate)
            .NotNull()
            .WithMessage(Scalars.ValidationMessageRequired);
        RuleFor(i => i.ModificationDate)
            .NotNull()
            .WithMessage(Scalars.ValidationMessageRequired);
    }
}