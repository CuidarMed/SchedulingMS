using Application.DTOs;
using MediatR;

namespace Application.Command.DoctorAvailability;

public record CreateDoctorAvailabilityCommand(long DoctorId, DoctorAvailabilityCreate Body)
    : IRequest<DoctorAvailabilityResponse>;
