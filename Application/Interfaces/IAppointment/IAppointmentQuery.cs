using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces.IAppointment
{
    public interface IAppointmentQuery
    {
        Task<IReadOnlyList<Appointment>> GetAll();
        Task<Appointment?> GetByIdAsync(long id);
        Task<IReadOnlyList<Appointment>> SearchAsync(AppointmentSearch search);
        Task<bool> HasConflictAsync(long doctorId, DateTimeOffset start, DateTimeOffset end);
    }
}
