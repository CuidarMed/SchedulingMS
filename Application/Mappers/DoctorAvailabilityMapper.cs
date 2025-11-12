using Application.DTOs;
using Application.Interfaces.IDoctorAvailability;
using Domain.Entities;

namespace Application.Mappers
{
    public class DoctorAvailabilityMapper : IDoctorAvailabilityMapper
    {
        public DoctorAvailabilityResponse ToResponse(DoctorAvailability entity)
        {
            return new DoctorAvailabilityResponse
            {
                AvailabilityId = entity.AvailabilityId,
                DoctorId = entity.DoctorId,
                DayOfWeek = entity.DayOfWeek,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                DurationMinutes = entity.DurationMinutes,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        public List<DoctorAvailabilityResponse> ToResponseList(IEnumerable<DoctorAvailability> entities)
        {
            return entities.Select(ToResponse).ToList();
        }
    }
}
