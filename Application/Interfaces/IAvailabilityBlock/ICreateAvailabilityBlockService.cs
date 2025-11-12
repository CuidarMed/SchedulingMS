using Application.DTOs;

namespace Application.Interfaces.IAvailabilityBlock
{
    public interface ICreateAvailabilityBlockService
    {
        Task<AvailabilityBlockResponse> CreateAsync(long doctorId, AvailabilityBlockCreate dto);
    }
}
