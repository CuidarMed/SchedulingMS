using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Queries
{
    public class AvailabilityBlockQuery:IAvailabilityBlockQuery
    {
        private readonly AppDbContext _context;
        public AvailabilityBlockQuery(AppDbContext context)
        {
            _context = context;
        }
        public async Task<AvailabilityBlock> GetByIdAsync(long DoctorId)
        {
            return await _context.AvailabilityBlocks.FirstOrDefaultAsync(d => d.DoctorId == DoctorId);
        }
    }
}
