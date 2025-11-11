using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infrastructure.Queries
{
    public class AvailabilityBlockQuery:IAvailabilityBlockQuery
    {
        private readonly AppDbContext _context;
        private readonly IAvailabilityBlockQuery _query;

        public AvailabilityBlockQuery(AppDbContext context, IAvailabilityBlockQuery query)
        {
            _context = context;
            _query = query;
        }

        public async Task<AvailabilityBlock> GetByIdAsync(long DoctorId)
        {
            return await _context.AvailabilityBlocks.FirstOrDefaultAsync(d => d.DoctorId == DoctorId);
        }

        public async Task<List<AvailabilityBlock>> GetByIdAsyncListBlocks(long DoctorId)
        {
            List<AvailabilityBlock> result = await _context.AvailabilityBlocks.ToListAsync();
            
            return result;
        }
    }
}
