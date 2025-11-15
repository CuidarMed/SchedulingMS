using Application.DTOs;
using Application.Interfaces.IDoctorAvailability;

namespace Application.Services.DoctorAvailabilityService
{
    public class UpdateDoctorAvailabilityService : IUpdateDoctorAvailabilityService
    {
        private readonly IDoctorAvailabilityCommand _command;
        private readonly IDoctorAvailabilityQuery _query;
        private readonly IDoctorAvailabilityMapper _mapper;

        public UpdateDoctorAvailabilityService(IDoctorAvailabilityCommand command, IDoctorAvailabilityQuery query, IDoctorAvailabilityMapper mapper)
        {
            _command = command;
            _query = query;
            _mapper = mapper;
        }

        public async Task<DoctorAvailabilityResponse> UpdateAsync(long id, DoctorAvailabilityUpdate dto)
        {
            var entity = await _query.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Disponibilidad no encontrada.");

            entity.StartTime = dto.StartTime;
            entity.EndTime = dto.EndTime;
            entity.DurationMinutes = dto.DurationMinutes;
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            if (dto.IsActive.HasValue)
                entity.IsActive = dto.IsActive.Value;

            var updated = await _command.UpdateAsync(entity);
            return _mapper.ToResponse(updated);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _query.GetByIdAsync(id);
            if (entity == null)
                return false;

            await _command.DeleteAsync(id);
            return true;
        }
    }
}
