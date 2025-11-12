using Application.DTOs;
using FluentValidation;

namespace Application.Validators.AvailabilityBlock
{
    public class AvailabilityBlockCreateValidator : AbstractValidator<AvailabilityBlockCreate>
    {
        public AvailabilityBlockCreateValidator()
        {
            RuleFor(x => x.StartTime)
                .NotEmpty().WithMessage("La fecha de inicio es obligatoria.");

            RuleFor(x => x.EndTime)
                .NotEmpty().WithMessage("La fecha de fin es obligatoria.")
                .GreaterThan(x => x.StartTime)
                .WithMessage("La fecha de fin debe ser posterior a la de inicio.");

            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Debe indicar un motivo del bloqueo.")
                .MaximumLength(255).WithMessage("El motivo no puede superar los 255 caracteres.");

            RuleFor(x => x.Note)
                .MaximumLength(500).WithMessage("La nota no puede superar los 500 caracteres.");
        }
    }
}
