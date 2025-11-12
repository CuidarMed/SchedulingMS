using Domain.Entities;

namespace Application.Interfaces.IAvailabilityBlock
{
    public interface IAvailabilityBlockCommand
    {
        Task<AvailabilityBlock> CreateAsync(AvailabilityBlock block);
        Task<AvailabilityBlock> UpdateAsync(AvailabilityBlock block);
        Task<bool> DeleteAsync(long doctorId, long blockId);
    }
}

