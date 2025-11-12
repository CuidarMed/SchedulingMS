using Application.DTOs;
using Application.Interfaces.IAppointment;
using Domain.Entities;

namespace Application.Mappers
{
    public class AppointmentMapper : IAppointmentMapper
    {
        public AppointmentResponse ToResponse(Appointment entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "La entidad no puede ser null");

            return new AppointmentResponse
            {
                AppointmentId = entity.AppointmentId,
                DoctorId = entity.DoctorId,
                PatientId = entity.PatientId,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                Status = entity.Status,
                Reason = entity.Reason,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                OriginalAppointmentId = entity.OriginalAppointmentId
            };
        }


        public List<AppointmentResponse> ToResponseList(List<Appointment> entities)
        {
            return entities?.Select(ToResponse).ToList() ?? new List<AppointmentResponse>();
        }
    }
}