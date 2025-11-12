using Application.DTOs;

namespace Application.Interfaces.IAppointment
{
    public interface ICreateAppointmentService
    {
        Task<AppointmentResponse> CreateAsync(AppointmentCreate appointment);
    }
}
