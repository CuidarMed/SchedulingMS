using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public class DoctorAvailabilityCreateValidator : AbstractValidator<DoctorAvailabilityCreate>
    {
        public DoctorAvailabilityCreateValidator()
        {
            RuleFor(x => x.DayOfWeek)
                .IsInEnum()
                .WithMessage("El día de la semana es inválido.");

            RuleFor(x => x.StartTime)
                .LessThan(x => x.EndTime)
                .WithMessage("La hora de inicio debe ser anterior a la hora de fin.");

            RuleFor(x => x.DurationMinutes)
                .GreaterThan(0)
                .LessThanOrEqualTo(480) // máximo 8 horas
                .WithMessage("La duración debe ser mayor a 0 y menor o igual a 480 minutos.");

            RuleFor(x => x.EndTime - x.StartTime)
                .Must(d => d.TotalMinutes >= 15)
                .WithMessage("El rango de tiempo debe ser al menos de 15 minutos.");
        }
    }
}
