using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public class DoctorAvailabilityUpdateValidator : AbstractValidator<DoctorAvailabilityUpdate>
    {
        public DoctorAvailabilityUpdateValidator()
        {
            RuleFor(x => x.StartTime)
                .LessThan(x => x.EndTime)
                .WithMessage("La hora de inicio debe ser anterior a la hora de fin.");

            RuleFor(x => x.DurationMinutes)
                .GreaterThan(0)
                .LessThanOrEqualTo(480)
                .WithMessage("La duración debe ser mayor a 0 y menor o igual a 480 minutos.");

            RuleFor(x => x.IsActive)
                .Must(x => x == null || x == true || x == false)
                .WithMessage("El campo IsActive debe ser un valor booleano válido.");
        }
    }
}
