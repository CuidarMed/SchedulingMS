using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUpdateAvailabilityBlock
    {
        public Task<AvailabilityBlockUpdate> Update(long DoctorId, AvailabilityBlockUpdate availabilityBlock);
    }
}
