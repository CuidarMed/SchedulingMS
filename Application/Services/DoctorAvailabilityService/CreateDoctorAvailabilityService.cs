using Application.DTOs;
using Application.Interfaces.IDoctorAvailability;
using Domain.Entities;

namespace Application.Services
{
    public class CreateDoctorAvailabilityService : ICreateDoctorAvailabilityService
    {
        private readonly IDoctorAvailabilityCommand _command;
        private readonly IDoctorAvailabilityMapper _mapper;

        public CreateDoctorAvailabilityService(IDoctorAvailabilityCommand command, IDoctorAvailabilityMapper mapper)
        {
            _command = command;
            _mapper = mapper;
        }

        public async Task<DoctorAvailabilityResponse> CreateAsync(long doctorId, DoctorAvailabilityCreate dto)
        {
            var entity = new DoctorAvailability
            {
                DoctorId = doctorId,
                DayOfWeek = dto.DayOfWeek,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                DurationMinutes = dto.DurationMinutes,
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            var created = await _command.CreateAsync(entity);
            return _mapper.ToResponse(created);
        }
    }
}
