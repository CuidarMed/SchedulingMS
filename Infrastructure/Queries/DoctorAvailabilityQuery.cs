using Application.Interfaces.IDoctorAvailability;
using Domain.Entities;
using Domain.Enum;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Queries
{
    public class DoctorAvailabilityQuery : IDoctorAvailabilityQuery
    {
        private readonly AppDbContext _context;

        public DoctorAvailabilityQuery(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DoctorAvailability>> GetAllAsync()
        {
            return await _context.DoctorAvailabilities
                .OrderBy(a => a.DoctorId)
                .ThenBy(a => a.DayOfWeek)
                .ToListAsync();
        }

        public async Task<DoctorAvailability?> GetByIdAsync(long id)
        {
            return await _context.DoctorAvailabilities
                .FirstOrDefaultAsync(a => a.AvailabilityId == id);
        }

        public async Task<IEnumerable<DoctorAvailability>> SearchAsync(long? doctorId, WeekDay? dayOfWeek)
        {
            var query = _context.DoctorAvailabilities.AsQueryable();

            if (doctorId.HasValue)
                query = query.Where(a => a.DoctorId == doctorId.Value);

            if (dayOfWeek.HasValue)
                query = query.Where(a => a.DayOfWeek == dayOfWeek.Value);

            return await query
                .OrderBy(a => a.DoctorId)
                .ThenBy(a => a.DayOfWeek)
                .ToListAsync();
        }
    }
}
