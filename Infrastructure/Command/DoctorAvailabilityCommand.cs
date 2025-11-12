using Application.Interfaces.IDoctorAvailability;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Commands
{
    public class DoctorAvailabilityCommand : IDoctorAvailabilityCommand
    {
        private readonly AppDbContext _context;

        public DoctorAvailabilityCommand(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DoctorAvailability> CreateAsync(DoctorAvailability availability)
        {
            _context.DoctorAvailabilities.Add(availability);
            await _context.SaveChangesAsync();
            return availability;
        }

        public async Task<DoctorAvailability> UpdateAsync(DoctorAvailability availability)
        {
            var existing = await _context.DoctorAvailabilities
                .FirstOrDefaultAsync(x => x.AvailabilityId == availability.AvailabilityId);

            if (existing == null)
                throw new KeyNotFoundException("No se encontró la disponibilidad del doctor.");

            existing.StartTime = availability.StartTime;
            existing.EndTime = availability.EndTime;
            existing.DurationMinutes = availability.DurationMinutes;
            existing.IsActive = availability.IsActive;
            existing.UpdatedAt = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task DeleteAsync(long availabilityId)
        {
            var availability = await _context.DoctorAvailabilities
                .FirstOrDefaultAsync(a => a.AvailabilityId == availabilityId);

            if (availability == null)
                throw new KeyNotFoundException("No se encontró la disponibilidad para eliminar.");

            _context.DoctorAvailabilities.Remove(availability);
            await _context.SaveChangesAsync();
        }
    }
}
