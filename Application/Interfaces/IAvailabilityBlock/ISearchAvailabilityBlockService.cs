using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces.IAvailabilityBlock
{
    public interface ISearchAvailabilityBlockService
    {
       Task<List<AvailabilityBlockResponse>> GetByDoctorAsync(long doctorId);
       Task<AvailabilityBlockResponse?> GetByIdAsync(long doctorId,long blockId);
    }
}
