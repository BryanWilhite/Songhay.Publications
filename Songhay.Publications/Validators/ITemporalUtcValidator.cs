using FluentValidation;

namespace Songhay.Publications.Validators;

/// <summary>
/// Validator for <see cref="ITemporal"/> under UTC time
/// </summary>
// ReSharper disable once InconsistentNaming
public class ITemporalUtcValidator :AbstractValidator<ITemporal>
{
    /// <inheritdoc/>
    public ITemporalUtcValidator()
    {
        RuleFor(data => data.EndDate)
            .Must(HaveDateTimeKindUtc)
            .When(data => data.EndDate.HasValue)
            .WithMessage(HaveDateTimeKindUtcMessage);

        RuleFor(data => data.InceptDate)
            .NotNull()
            .Must(HaveDateTimeKindUtc)
            .WithMessage(HaveDateTimeKindUtcMessage);

        RuleFor(data => data.ModificationDate)
            .NotNull()
            .Must(HaveDateTimeKindUtc)
            .WithMessage(HaveDateTimeKindUtcMessage);
    }

    private const string HaveDateTimeKindUtcMessage = "The date-time kind UTC was not found.";

    private static bool HaveDateTimeKindUtc(DateTime? dateTime) => dateTime?.Kind == DateTimeKind.Utc;
}
