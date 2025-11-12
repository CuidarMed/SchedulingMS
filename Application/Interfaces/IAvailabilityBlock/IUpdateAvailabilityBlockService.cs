using Application.DTOs;

namespace Application.Interfaces.IAvailabilityBlock
{
    public interface IUpdateAvailabilityBlockService
    {
        Task<AvailabilityBlockResponse> UpdateAsync(long doctorId, long blockId, AvailabilityBlockUpdate dto);
        Task<bool> DeleteAsync(long doctorId, long blockId);
    }
}
