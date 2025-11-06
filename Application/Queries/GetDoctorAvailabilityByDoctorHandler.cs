using Application.DTOs;
using Application.Interfaces.Queries;
using MediatR;

namespace Application.Queries.DoctorAvailability;

public sealed class GetDoctorAvailabilityByDoctorHandler
    : IRequestHandler<GetDoctorAvailabilityByDoctorQuery, List<DoctorAvailabilityResponse>>
{
    private readonly IDoctorAvailabilityQueryRepository _repo;
    public GetDoctorAvailabilityByDoctorHandler(IDoctorAvailabilityQueryRepository repo) => _repo = repo;

    public async Task<List<DoctorAvailabilityResponse>> Handle(GetDoctorAvailabilityByDoctorQuery rq, CancellationToken ct)
    {
        var list = await _repo.GetByDoctorAsync(rq.DoctorId, ct);

        return list
            .OrderBy(x => x.DayOfWeek).ThenBy(x => x.StartTime)
            .Select(x => new DoctorAvailabilityResponse
            {
                AvailabilityId = x.AvailabilityId,
                DoctorId = x.DoctorId,
                DayOfWeek = x.DayOfWeek,
                StartTime = x.StartTime,   
                EndTime = x.EndTime,     
                DurationMinutes = x.DurationMinutes,
                IsActive = x.IsActive,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToList();
    }
}
