using Application.DTOs;
using Application.Interfaces.IDoctorAvailability;
using Domain.Enum;

namespace Application.Services.DoctorAvailabilityService
{
    public class SearchDoctorAvailabilityService : ISearchDoctorAvailabilityService
    {
        private readonly IDoctorAvailabilityQuery _query;
        private readonly IDoctorAvailabilityMapper _mapper;

        public SearchDoctorAvailabilityService(IDoctorAvailabilityQuery query, IDoctorAvailabilityMapper mapper)
        {
            _query = query;
            _mapper = mapper;
        }

        public async Task<List<DoctorAvailabilityResponse>> GetAllAsync()
        {
            var result = await _query.GetAllAsync();
            return _mapper.ToResponseList(result);
        }

        public async Task<DoctorAvailabilityResponse?> GetByIdAsync(long id)
        {
            var entity = await _query.GetByIdAsync(id);
            return entity == null ? null : _mapper.ToResponse(entity);
        }

        public async Task<List<DoctorAvailabilityResponse>> SearchAsync(long? doctorId, WeekDay? dayOfWeek)
        {
            var result = await _query.SearchAsync(doctorId, dayOfWeek);
            return _mapper.ToResponseList(result);
        }
    }
}
