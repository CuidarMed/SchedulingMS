using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAvailabilityBlockCommand
    {
        public Task<Domain.Entities.AvailabilityBlock> CreateAsync(Domain.Entities.AvailabilityBlock availabilityBlock);
        public Task<Domain.Entities.AvailabilityBlock> UpdateAsync(Domain.Entities.AvailabilityBlock availabilityBlock);
    }
}
