using Application.Interfaces;
using Application.Interfaces.IAvailabilityBlock;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Queries
{
    public class AvailabilityBlockQuery : IAvailabilityBlockQuery
    {
        private readonly AppDbContext _context;

        public AvailabilityBlockQuery(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AvailabilityBlock>> GetAllAsync()
        {
            return await _context.AvailabilityBlocks.AsNoTracking().ToListAsync();
        }

        public async Task<AvailabilityBlock?> GetByIdAsync(long doctorId, long blockId)
        {
            return await _context.AvailabilityBlocks.AsNoTracking()
                .FirstOrDefaultAsync(x => x.BlockId == blockId && x.DoctorId== doctorId);
        }

        public async Task<IEnumerable<AvailabilityBlock>> GetByDoctorAsync(long doctorId)
        {
            return await _context.AvailabilityBlocks.AsNoTracking()
                .Where(x => x.DoctorId == doctorId)
                .OrderBy(b => b.StartTime)
                .ToListAsync();
        }

        public async Task<bool> HasOverlapAsync(long doctorId, DateTimeOffset startTime, DateTimeOffset endTime, long? excludeBlockId = null)
        {
            var query = _context.AvailabilityBlocks
                .Where(b =>
                    b.DoctorId == doctorId &&
                    b.IsBlock &&
                    ((b.StartTime < endTime && b.EndTime > startTime)));

            if (excludeBlockId.HasValue)
            {
                query = query.Where(b => b.BlockId != excludeBlockId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
