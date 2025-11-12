using Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.AppointmentValidators
{
    public class AppointmentCancelValidator : AbstractValidator<AppointmentCancel>
    {
        public AppointmentCancelValidator()
        {
            RuleFor(x => x.Reason)
                .NotEmpty()
                .WithMessage("La razón de cancelación es requerida.")
                .MaximumLength(500)
                .WithMessage("La razón no puede exceder 500 caracteres.");
        }
    }
}
