using Application.DTOs;
using Domain.Enum;

namespace Application.Interfaces.IDoctorAvailability
{
    public interface ISearchDoctorAvailabilityService
    {
        Task<List<DoctorAvailabilityResponse>> GetAllAsync();
        Task<DoctorAvailabilityResponse?> GetByIdAsync(long id);
        Task<List<DoctorAvailabilityResponse>> SearchAsync(long? doctorId, WeekDay? dayOfWeek);
    }
}
