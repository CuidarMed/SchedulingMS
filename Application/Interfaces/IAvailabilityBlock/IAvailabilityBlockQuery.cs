using Domain.Entities;

namespace Application.Interfaces.IAvailabilityBlock
{
    public interface IAvailabilityBlockQuery
    {
        Task<IEnumerable<AvailabilityBlock>> GetAllAsync();
        Task<AvailabilityBlock?> GetByIdAsync(long doctorId,long blockId);
        Task<IEnumerable<AvailabilityBlock>> GetByDoctorAsync(long doctorId);
        Task<bool> HasOverlapAsync(long doctorId,DateTimeOffset startTime, DateTimeOffset endTime,long? excludeBlockId = null);
    }
}
