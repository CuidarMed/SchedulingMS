using Application.DTOs;

namespace Application.Interfaces.IDoctorAvailability
{
    public interface ICreateDoctorAvailabilityService
    {
        Task<DoctorAvailabilityResponse> CreateAsync(long doctorId,  DoctorAvailabilityCreate request);
    }
}
