using Application.DTOs;
using FluentValidation;
using System;           
using System.Linq;      

namespace Application.Command.DoctorAvailability;

public sealed class CreateDoctorAvailabilityValidator : AbstractValidator<DoctorAvailabilityCreate>
{
    public CreateDoctorAvailabilityValidator()
    {
        RuleFor(x => x.StartTime)
            .Must(t => t >= TimeSpan.Zero && t < TimeSpan.FromHours(24))
            .WithMessage("StartTime debe estar entre 00:00 y 23:59.");

        RuleFor(x => x.EndTime)
            .Must(t => t > TimeSpan.Zero && t <= TimeSpan.FromHours(24))
            .WithMessage("EndTime debe estar entre 00:01 y 24:00.");

        RuleFor(x => x)
            .Must(x => x.EndTime > x.StartTime)
            .WithMessage("EndTime debe ser mayor a StartTime.");

        RuleFor(x => x.DurationMinutes)
            .Must(x => new[] { 10, 15, 20, 30 }.Contains(x))
            .WithMessage("DurationMinutes debe ser 10/15/20/30.");
    }
}
