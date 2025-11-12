using Application.DTOs;
using FluentValidation;

namespace Application.Validators.AvailabilityBlock
{
    public class AvailabilityBlockUpdateValidator : AbstractValidator<AvailabilityBlockUpdate>
    {
        public AvailabilityBlockUpdateValidator()
        {
            RuleFor(x => x.EndTime)
                .GreaterThan(x => x.StartTime)
                .When(x => x.StartTime.HasValue && x.EndTime.HasValue)
                .WithMessage("La fecha de fin debe ser posterior a la de inicio.");

            RuleFor(x => x.Reason)
                .MaximumLength(255).WithMessage("El motivo no puede superar los 255 caracteres.");

            RuleFor(x => x.Note)
                .MaximumLength(500).WithMessage("La nota no puede superar los 500 caracteres.");
        }
    }
}
