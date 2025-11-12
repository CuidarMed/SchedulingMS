using Application.DTOs;
using FluentValidation;

namespace Application.Validators.AppointmentValidators
{
    public class AppointmentSearchValidator : AbstractValidator<AppointmentSearch>
    {
        public AppointmentSearchValidator()
        {
            
            When(x => x.StartTime.HasValue && x.EndTime.HasValue, () =>
            {
                RuleFor(x => x.EndTime)
                    .GreaterThan(x => x.StartTime)
                    .WithMessage("La fecha de fin debe ser mayor a la fecha de inicio.");
            });
        }
    }
}

