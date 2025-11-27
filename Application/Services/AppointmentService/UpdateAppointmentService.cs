using Application.DTOs;
using Application.Interfaces.IAppointment;
using Application.Interfaces.IClinical;
using Domain.Enum;
using Microsoft.Extensions.Logging;

namespace Application.Services.AppointmentService
{
    public class UpdateAppointmentService : IUpdateAppointmentService
    {
        private readonly IAppointmentCommand _command;
        private readonly IAppointmentQuery _query;
        private readonly IAppointmentMapper _mapper;
        private readonly ILogger<UpdateAppointmentService> _logger;

        public UpdateAppointmentService(
            IAppointmentCommand command,
            IAppointmentQuery query,
            IAppointmentMapper mapper,
            ILogger<UpdateAppointmentService> logger)
        {
            _command = command;
            _query = query;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<AppointmentResponse> CancelAsync(long id, AppointmentCancel request)
        {
            var appointment = await _query.GetByIdAsync(id);
            if (appointment == null)
                throw new InvalidOperationException("Appointment not found.");

            if (appointment.Status == AppointmentStatus.CANCELLED)
                throw new InvalidOperationException("Appointment is already cancelled.");

            // Verificar si existe un encounter en ClinicalMS antes de cancelar
            // Si existe, podría requerir una acción adicional o prevenir la cancelación
            try
            {
                
                if (appointment.Status == AppointmentStatus.COMPLETED)
                {
                    _logger.LogWarning(
                        "Intento de cancelar appointment {AppointmentId} que ya tiene un encounter en ClinicalMS",
                        id);
                    // Opcional: podrías lanzar una excepción o solo loguear
                    // throw new InvalidOperationException("No se puede cancelar un appointment que ya tiene un encounter clínico.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar encounter en ClinicalMS para appointment {AppointmentId}", id);
                // Continuar con la cancelación aunque falle la verificación
            }

            appointment.Status = AppointmentStatus.CANCELLED;
            appointment.Reason = request.Reason;
            appointment.UpdatedAt = DateTimeOffset.UtcNow;

            await _command.UpdateAsync(appointment);

            return _mapper.ToResponse(appointment);
        }

        public async Task<AppointmentResponse> RescheduleAsync(long id, AppointmentReschedule request)
        {
            var appointment = await _query.GetByIdAsync(id);
            if (appointment == null)
                throw new InvalidOperationException("Appointment not found.");

            if (request.NewEndTime <= request.NewStartTime)
                throw new ArgumentException("NewEndTime must be after NewStartTime.");

            // Validar conflictos de horario
            var hasConflict = await _query.HasConflictAsync(
                appointment.DoctorId,
                request.NewStartTime,
                request.NewEndTime);

            if (hasConflict)
                throw new InvalidOperationException("Doctor has a conflicting appointment at the new time.");

            appointment.StartTime = request.NewStartTime;
            appointment.EndTime = request.NewEndTime;
            appointment.Reason = request.Reason;
            appointment.UpdatedAt = DateTimeOffset.UtcNow;

            await _command.UpdateAsync(appointment);

            return _mapper.ToResponse(appointment);
        }

        public async Task<AppointmentResponse> UpdateAsync(long id, AppointmentUpdate dto)
        {
            // Obtener la cita existente
            var appointment = await _query.GetByIdAsync(id);
            if (appointment == null)
                throw new InvalidOperationException("Appointment not found.");

            if (dto.EndTime <= dto.StartTime)
                throw new ArgumentException("EndTime must be after StartTime");

            // Validar conflictos (solo si cambió doctor o horario)
            if (appointment.DoctorId != dto.DoctorId ||
                appointment.StartTime != dto.StartTime ||
                appointment.EndTime != dto.EndTime)
            {
                var hasConflict = await _query.HasConflictAsync(dto.DoctorId, dto.StartTime, dto.EndTime);
                if (hasConflict)
                    throw new InvalidOperationException("Doctor has a conflicting appointment.");
            }

            // Actualizar propiedades
            appointment.DoctorId = dto.DoctorId;
            appointment.PatientId = dto.PatientId;
            appointment.StartTime = dto.StartTime;
            appointment.EndTime = dto.EndTime;
            appointment.Status = dto.Status ?? appointment.Status;
            appointment.Reason = dto.Reason ?? appointment.Reason;
            appointment.UpdatedAt = DateTimeOffset.UtcNow;

            await _command.UpdateAsync(appointment);

            return _mapper.ToResponse(appointment);
        }

        public async Task<AppointmentResponse> UpdateAttendanceAsync(long id, AppointmentAttendance request)
        {
            var appointment = await _query.GetByIdAsync(id);
            if (appointment == null)
                throw new InvalidOperationException("Appointment not found.");

            // Validación de estado usando el validador o enum directamente
            if (request.Status != AppointmentStatus.COMPLETED && request.Status != AppointmentStatus.NO_SHOW)
                throw new ArgumentException("Status must be Completed or NoShow.");

            appointment.Status = (AppointmentStatus)request.Status;
            appointment.Reason = request.Reason;
            appointment.UpdatedAt = DateTimeOffset.UtcNow;

            await _command.UpdateAsync(appointment);

            return _mapper.ToResponse(appointment);
        }
    }
}
