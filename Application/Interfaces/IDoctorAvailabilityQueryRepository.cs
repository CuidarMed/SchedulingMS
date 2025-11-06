using Domain.Entities;
using Domain.Enum;

namespace Application.Interfaces.Queries;

public interface IDoctorAvailabilityQueryRepository
{
    Task<List<DoctorAvailability>> GetByDoctorAsync(long doctorId, CancellationToken ct);
    Task<DoctorAvailability?> GetByIdAsync(long doctorId, long availabilityId, CancellationToken ct);
    Task<bool> ExistsOverlapAsync(long doctorId, WeekDay day, TimeSpan start, TimeSpan end, long? excludeId, CancellationToken ct);
}