using Application.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ISearchAvailabilityBlock
    {
        public Task<AvailabilityBlockResponse> GetByIdAsync(long DoctorId);
        public Task<List<AvailabilityBlock>> GetByIdAsyncList(long DoctorId);
    }
}
