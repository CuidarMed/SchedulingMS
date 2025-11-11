using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class CreateAppointmentService : ICreateAppointmentService
    {
        private readonly IAppointmentCommand _command;
        private readonly IAppointmentQuery _query;

        public CreateAppointmentService(IAppointmentCommand command, IAppointmentQuery query)
        {
            _command = command;
            _query = query;
        }

        public async Task<AppointmentResponse> CreateAsync(AppointmentCreate request)
        {
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
                Status = "CONFIRMED",
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            var created = await _command.CreateAsync(appointment);
            return MapToResponse(created);
        }

        private AppointmentResponse MapToResponse(Appointment appointment)
        {
            return new AppointmentResponse
            {
                AppointmentId = appointment.AppointmentId,
                DoctorId = appointment.DoctorId,
                PatientId = appointment.PatientId,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Status = appointment.Status,
                Reason = appointment.Reason,
                MeetingURL = appointment.MeetingURL,
                CreatedAt = appointment.CreatedAt,
                UpdatedAt = appointment.UpdatedAt,
                OriginalAppointmentId = appointment.OriginalAppointmentId
            };
        }
    }
}
