using Application.DTOs;

namespace Application.Interfaces.IAppointment
{
    public interface ISearchAppointmentService
    {
        Task<AppointmentResponse> GetByIdAsync(long id);
        Task<List<AppointmentResponse>> GetAllAsync();
        Task<List<AppointmentResponse>> SearchAsync(AppointmentSearch search);
        Task<List<AppointmentResponse>> GetPatientsByDoctorIdAsync(long doctorId);

    }
}
