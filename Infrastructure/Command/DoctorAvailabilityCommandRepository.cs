using Application.Interfaces.Command;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Command;

public sealed class DoctorAvailabilityCommandRepository : IDoctorAvailabilityCommandRepository
{
    private readonly AppDbContext _db;
    public DoctorAvailabilityCommandRepository(AppDbContext db) => _db = db;

    public async Task<DoctorAvailability> AddAsync(DoctorAvailability entity, CancellationToken ct)
    {
        var entry = await _db.DoctorAvailabilities.AddAsync(entity, ct);
        return entry.Entity;
    }

    public Task UpdateAsync(DoctorAvailability entity, CancellationToken ct)
    {
        _db.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}
