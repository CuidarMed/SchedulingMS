using Domain.Entities;
using Domain.Enum;

namespace Application.Interfaces.IDoctorAvailability;

public interface IDoctorAvailabilityQuery
{
    Task<IEnumerable<DoctorAvailability>> GetAllAsync();
    Task<DoctorAvailability?> GetByIdAsync(long id);
    Task<IEnumerable<DoctorAvailability>> SearchAsync(long? doctorId, WeekDay? dayOfWeek);
}