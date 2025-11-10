using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAvailabilityBlockQuery
    {
        public Task<AvailabilityBlock> GetByIdAsync(long DoctorId);
    }
}
