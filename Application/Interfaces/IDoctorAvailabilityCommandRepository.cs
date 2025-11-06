using Domain.Entities;

namespace Application.Interfaces.Command;

public interface IDoctorAvailabilityCommandRepository
{
    Task<DoctorAvailability> AddAsync(DoctorAvailability entity, CancellationToken ct);
    Task UpdateAsync(DoctorAvailability entity, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}