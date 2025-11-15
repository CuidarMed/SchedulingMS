using Application.DTOs;
using Application.Interfaces.IAppointment;
using Domain.Entities;
using Domain.Enum;
using FluentValidation;

namespace Application.Services.AppointmentService
{
    public class CreateAppointmentService : ICreateAppointmentService
    {
        private readonly IAppointmentCommand _command;
        private readonly IAppointmentQuery _query;
        private readonly IAppointmentMapper _mapper;
        private readonly IValidator<AppointmentCreate> _validator;

        public CreateAppointmentService(IAppointmentCommand command, IAppointmentQuery query, IAppointmentMapper mapper, IValidator<AppointmentCreate> validator)
        {
            _command = command;
            _query = query;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<AppointmentResponse> CreateAsync(AppointmentCreate request)
        {
            await _validator.ValidateAndThrowAsync(request);

            // Validar horarios
            if (request.StartTime >= request.EndTime)
                throw new Exception("StartTime debe ser menor que EndTime");

            // Validar conflictos
            var hasConflict = await _query.HasConflictAsync(
                request.DoctorId,
                request.StartTime,
                request.EndTime);

            if (hasConflict)
                throw new Exception("El doctor ya tiene un turno en ese horario");

            // Crear appointment
            var appointment = new Appointment
            {
                DoctorId = request.DoctorId,
                PatientId = request.PatientId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Reason = request.Reason,
                Status = AppointmentStatus.SCHEDULED,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };
            
            await _command.CreateAsync(appointment);
            return _mapper.ToResponse(appointment);
        }
    }
}
