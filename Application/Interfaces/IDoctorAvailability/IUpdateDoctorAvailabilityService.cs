using Application.DTOs;

namespace Application.Interfaces.IDoctorAvailability
{
    public interface IUpdateDoctorAvailabilityService
    {

        Task<DoctorAvailabilityResponse> UpdateAsync(long id, DoctorAvailabilityUpdate dto);
        Task<bool> DeleteAsync(long id);
    }
}
