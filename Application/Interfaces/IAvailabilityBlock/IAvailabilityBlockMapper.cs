using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces.IAvailabilityBlock
{
    public interface IAvailabilityBlockMapper
    {
        AvailabilityBlockResponse ToResponse(AvailabilityBlock entity);
        List<AvailabilityBlockResponse> ToResponseList(IEnumerable<AvailabilityBlock> entities);
    }
}
