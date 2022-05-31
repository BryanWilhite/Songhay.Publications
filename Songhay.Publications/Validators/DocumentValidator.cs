using FluentValidation;
using Songhay.Publications.Models;

namespace Songhay.Publications.Validators
{
    /// <summary>
    /// Validator for <see cref="Document"/>.
    /// </summary>
    public class DocumentValidator : AbstractValidator<Document>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentValidator"/> class.
        /// </summary>
        public DocumentValidator()
        {
            RuleFor(i => i.IsActive)
                .NotNull()
                .WithMessage(Scalars.ValidationMessageRequired);
            RuleFor(i => i.SegmentId)
                .NotNull()
                .WithMessage(Scalars.ValidationMessageRequired);
            RuleFor(i => i.DocumentId)
                .NotNull()
                .WithMessage(Scalars.ValidationMessageRequired);
            RuleFor(i => i.Title)
                .NotNull()
                .WithMessage(Scalars.ValidationMessageRequired);

            RuleFor(i => i).SetValidator(new ITemporalValidator());
        }
    }
}
