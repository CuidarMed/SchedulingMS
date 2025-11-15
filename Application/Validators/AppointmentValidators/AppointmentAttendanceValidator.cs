using FluentValidation;
using Application.DTOs;
using Domain.Enum;

public class AppointmentAttendanceDtoValidator : AbstractValidator<AppointmentAttendance>
{
    public AppointmentAttendanceDtoValidator()
    {
        RuleFor(x => x.Status)
            .NotNull()
            .WithMessage("El estado es requerido.")
            .Must(status => status == AppointmentStatus.COMPLETED || status == AppointmentStatus.NO_SHOW)
            .WithMessage("El estado debe ser Completed o NoShow.");
    }
}
