using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces.IDoctorAvailability
{
    public interface IDoctorAvailabilityMapper
    {
        public DoctorAvailabilityResponse ToResponse(DoctorAvailability entity);
        public List<DoctorAvailabilityResponse> ToResponseList(IEnumerable<DoctorAvailability> entities);
    }
}
