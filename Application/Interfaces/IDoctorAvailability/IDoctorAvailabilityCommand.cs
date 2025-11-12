using Domain.Entities;

namespace Application.Interfaces.IDoctorAvailability
{
    public interface IDoctorAvailabilityCommand
    {
        Task<DoctorAvailability> CreateAsync(DoctorAvailability availability);
        Task<DoctorAvailability> UpdateAsync(DoctorAvailability availability);
        Task DeleteAsync(long id);
    }
}
