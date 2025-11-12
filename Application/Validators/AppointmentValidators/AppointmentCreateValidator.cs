using Application.DTOs;
using FluentValidation;

namespace Application.Validators.AppointmentValidators
{
    public class AppointmentCreateValidator : AbstractValidator<AppointmentCreate>
    {
        public AppointmentCreateValidator()
        {
            RuleFor(x => x.DoctorId)
                .GreaterThan(0)
                .WithMessage("El ID del doctor es requerido.");

            RuleFor(x => x.PatientId)
                .GreaterThan(0)
                .WithMessage("El ID del paciente es requerido.");

            RuleFor(x => x.StartTime)
                .NotEmpty()
                .WithMessage("La hora de inicio es requerida.")
                .GreaterThan(DateTimeOffset.UtcNow)
                .WithMessage("No se pueden crear turnos en el pasado.");

            RuleFor(x => x.EndTime)
                .NotEmpty()
                .WithMessage("La hora de fin es requerida.")
                .GreaterThan(x => x.StartTime)
                .WithMessage("La hora de fin debe ser mayor a la hora de inicio.");
        }
    }
}
