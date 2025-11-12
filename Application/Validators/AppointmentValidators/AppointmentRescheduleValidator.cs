using Application.DTOs;
using FluentValidation;

namespace Application.Validators.AppointmentValidators
{
    public class AppointmentRescheduleValidator : AbstractValidator<AppointmentReschedule>
    {
        public AppointmentRescheduleValidator()
        {
            RuleFor(x => x.NewStartTime)
                .NotEmpty()
                .WithMessage("La nueva hora de inicio es requerida.")
                .GreaterThan(DateTimeOffset.UtcNow)
                .WithMessage("No se pueden crear turnos en el pasado.");

            RuleFor(x => x.NewEndTime)
                .NotEmpty()
                .WithMessage("La nueva hora de fin es requerida.")
                .GreaterThan(x => x.NewStartTime)
                .WithMessage("La hora de fin debe ser mayor a la hora de inicio.");
        }
    }
}
