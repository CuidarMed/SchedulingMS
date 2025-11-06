using Application.Interfaces.Queries;
using Domain.Entities;
using Domain.Enum;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Queries;

public sealed class DoctorAvailabilityQueryRepository : IDoctorAvailabilityQueryRepository
{
    private readonly AppDbContext _db;
    public DoctorAvailabilityQueryRepository(AppDbContext db) => _db = db;

    public Task<List<DoctorAvailability>> GetByDoctorAsync(long doctorId, CancellationToken ct) =>
        _db.DoctorAvailabilities
           .AsNoTracking()
           .Where(x => x.DoctorId == doctorId && x.IsActive)
           .ToListAsync(ct);

    public Task<DoctorAvailability?> GetByIdAsync(long doctorId, long availabilityId, CancellationToken ct) =>
        _db.DoctorAvailabilities
           .FirstOrDefaultAsync(x => x.DoctorId == doctorId && x.AvailabilityId == availabilityId, ct);

    public Task<bool> ExistsOverlapAsync(long doctorId, WeekDay day, TimeSpan start, TimeSpan end, long? excludeId, CancellationToken ct)
    {
        return _db.DoctorAvailabilities
            .AnyAsync(x =>
                x.DoctorId == doctorId &&
                x.DayOfWeek == day &&
                x.IsActive &&
                (excludeId == null || x.AvailabilityId != excludeId) &&
                !(x.EndTime <= start || x.StartTime >= end), ct);
    }
}
