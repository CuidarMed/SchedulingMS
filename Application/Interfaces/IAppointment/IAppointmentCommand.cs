using Domain.Entities;

namespace Application.Interfaces.IAppointment
{
    public interface IAppointmentCommand
    {
        Task CreateAsync(Appointment appointment);
        Task UpdateAsync(Appointment appointment);
        Task DeleteAsync(long appointmentId);
    }

}
