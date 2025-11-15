using Application.DTOs;
using Application.Interfaces.IAvailabilityBlock;
using Domain.Entities;

namespace Application.Mappers
{
    public class AvailabilityBlockMapper : IAvailabilityBlockMapper
    {
        public AvailabilityBlockResponse ToResponse(AvailabilityBlock entity)
        {
            return new AvailabilityBlockResponse
            {
                BlockId = entity.BlockId,
                DoctorId = entity.DoctorId,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                Reason = entity.Reason,
                Note = entity.Note,
                AllDay = entity.AllDay,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }
        public List<AvailabilityBlockResponse> ToResponseList(IEnumerable<AvailabilityBlock> entities)
        {
            return entities?.Select(ToResponse).ToList() ?? new List<AvailabilityBlockResponse>();
        }
    }
}
