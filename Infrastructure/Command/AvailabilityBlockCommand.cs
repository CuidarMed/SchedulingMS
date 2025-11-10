using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Command
{
    public class AvailabilityBlockCommand: IAvailabilityBlockCommand
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
    }
}
