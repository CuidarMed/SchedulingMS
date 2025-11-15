using Application.Interfaces;
using Application.Interfaces.IAvailabilityBlock;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Commands
{
    public class AvailabilityBlockCommand : IAvailabilityBlockCommand
    {
        private readonly AppDbContext _context;

        public AvailabilityBlockCommand(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AvailabilityBlock> CreateAsync(AvailabilityBlock availabilityBlock)
        {
            _context.AvailabilityBlocks.Add(availabilityBlock);
            await _context.SaveChangesAsync();

            return availabilityBlock;
        }

        public async Task<AvailabilityBlock> UpdateAsync(AvailabilityBlock availabilityBlock)
        {
            _context.AvailabilityBlocks.Update(availabilityBlock);
            await _context.SaveChangesAsync();

            return availabilityBlock;
        }

        public async Task<bool> DeleteAsync(long doctorId, long blockId)
        {
            var block = await _context.AvailabilityBlocks.FindAsync(doctorId, blockId);
            if (block == null)
                return false;

            _context.AvailabilityBlocks.Remove(block);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
