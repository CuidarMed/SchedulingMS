using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
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
        public async Task<List<AvailabilityBlock>> GetByIdAsyncList(long DoctorId)
        {
            var list = new List<AvailabilityBlock>();

            return list
                .Where(d => d.DoctorId == DoctorId && d.IsBlock == false)
                .OrderBy(d => d.StartTime)
                .Select(d => new AvailabilityBlock
                {
                    BlockId = d.BlockId,
                    DoctorId = d.DoctorId,
                    StartTime = d.StartTime,
                    EndTime = d.EndTime,
                    IsBlock = d.IsBlock,
                    AllDay = d.AllDay
                }).ToList();
        }
    }
}
