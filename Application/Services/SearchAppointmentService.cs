using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services
{
    public class SearchAppointmentService : ISearchAppointmentService
    {
        private readonly IAppointmentCommand _command;
        private readonly IAppointmentQuery _query;

        public SearchAppointmentService(IAppointmentCommand command, IAppointmentQuery query)
        {
            _command = command;
            _query = query;
        }

        public async Task<AppointmentResponse?> GetByIdAsync(long id)
        {
            var appointment = await _query.GetByIdAsync(id);
            return appointment != null ? MapToResponse(appointment) : null;
        }

        public async Task<IEnumerable<AppointmentResponse>> SearchAsync(AppointmentSearch search)
        {
            var appointments = await _query.SearchAsync(
                search.DoctorId,
                search.PatientId,
                search.StartTime,
                search.EndTime,
                search.Status,
                search.PageNumber,
                search.PageSize);

            return appointments.Select(MapToResponse);
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
