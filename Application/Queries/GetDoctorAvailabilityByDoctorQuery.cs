using Application.DTOs;
using MediatR;

namespace Application.Queries.DoctorAvailability;

public record GetDoctorAvailabilityByDoctorQuery(long DoctorId) : IRequest<List<DoctorAvailabilityResponse>>;
