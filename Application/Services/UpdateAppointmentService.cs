using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class UpdateAppointmentService : IUpdateAppointmentService
    {
        private readonly IAppointmentCommand _command;
        private readonly IAppointmentQuery _query;

        public UpdateAppointmentService(IAppointmentCommand command, IAppointmentQuery query)
        {
            _command = command;
            _query = query;
        }

        public async Task<AppointmentResponse> RescheduleAsync(long id, AppointmentReschedule request)
        {
            var appointment = await _query.GetByIdAsync(id);
            if (appointment == null)
                throw new Exception("Turno no encontrado");

            if (appointment.Status == "CANCELLED")
                throw new Exception("No se puede reprogramar un turno cancelado");

            // Validar conflictos con el nuevo horario
            var hasConflict = await _query.HasConflictAsync(
                appointment.DoctorId,
                request.NewStartTime,
                request.NewEndTime,
                id);

            if (hasConflict)
                throw new Exception("El doctor ya tiene un turno en ese horario");

            // Crear nuevo appointment
            var newAppointment = new Appointment
            {
                DoctorId = appointment.DoctorId,
                PatientId = appointment.PatientId,
                StartTime = request.NewStartTime,
                EndTime = request.NewEndTime,
                Status = "RESCHEDULED",
                OriginalAppointmentId = id,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            var created = await _command.CreateAsync(newAppointment);

            // Marcar el original como cancelado
            appointment.Status = "CANCELLED";
            appointment.Reason = "Reprogramado";
            appointment.UpdatedAt = DateTimeOffset.UtcNow;
            await _command.UpdateAsync(appointment);

            return MapToResponse(created);
        }

        public async Task<AppointmentResponse> CancelAsync(long id, AppointmentCancel request)
        {
            var appointment = await _query.GetByIdAsync(id);
            if (appointment == null)
                throw new Exception("Turno no encontrado");

            if (appointment.Status == "CANCELLED")
                throw new Exception("El turno ya está cancelado");

            appointment.Status = "CANCELLED";
            appointment.Reason = request.Reason;
            appointment.UpdatedAt = DateTimeOffset.UtcNow;

            var updated = await _command.UpdateAsync(appointment);
            return MapToResponse(updated);
        }

        public async Task<AppointmentResponse> UpdateAttendanceAsync(long id, AppointmentAttendance request)
        {
            var appointment = await _query.GetByIdAsync(id);
            if (appointment == null)
                throw new Exception("Turno no encontrado");

            if (appointment.Status == "CANCELLED")
                throw new Exception("No se puede marcar asistencia en un turno cancelado");

            appointment.Status = request.Status; // COMPLETED o NO_SHOW
            appointment.UpdatedAt = DateTimeOffset.UtcNow;

            var updated = await _command.UpdateAsync(appointment);
            return MapToResponse(updated);
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
