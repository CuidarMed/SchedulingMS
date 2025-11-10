using Application.DTOs;
using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class SearchAvailabilityBlock:ISearchAvailabilityBlock
    {
        private readonly IAvailabilityBlockQuery _query;
        public SearchAvailabilityBlock(IAvailabilityBlockQuery query)
        {
            _query = query;
        }
        public async Task<AvailabilityBlockResponse> GetByIdAsync(long DoctorId)
        {
            var doctor =  await _query.GetByIdAsync(DoctorId);
            
            return new AvailabilityBlockResponse
            {
                BlockId = doctor.BlockId,
                DoctorId = doctor.DoctorId,
                StartTime = doctor.StartTime,
                EndTime = doctor.EndTime,
                Reason = doctor.Reason,
                IsActive = doctor.IsBlock,
                CreatedAt = doctor.CreatedAt,
                UpdatedAt = doctor.UpdatedAt
            };
        }
    }
}
