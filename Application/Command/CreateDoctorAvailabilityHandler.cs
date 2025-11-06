using System;
using Application.DTOs;
using Application.Interfaces.Command;
using Application.Interfaces.Queries;
using FluentValidation;
using MediatR;

namespace Application.Command.DoctorAvailability;

public sealed class CreateDoctorAvailabilityHandler
    : IRequestHandler<CreateDoctorAvailabilityCommand, DoctorAvailabilityResponse>
{
    private readonly IDoctorAvailabilityCommandRepository _cmd;
    private readonly IDoctorAvailabilityQueryRepository _qry;
    private readonly IValidator<DoctorAvailabilityCreate> _validator;

    public CreateDoctorAvailabilityHandler(
        IDoctorAvailabilityCommandRepository cmd,
        IDoctorAvailabilityQueryRepository qry,
        IValidator<DoctorAvailabilityCreate> validator)
    {
        _cmd = cmd ?? throw new ArgumentNullException(nameof(cmd));
        _qry = qry ?? throw new ArgumentNullException(nameof(qry));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<DoctorAvailabilityResponse> Handle(CreateDoctorAvailabilityCommand rq, CancellationToken ct)
    {
        await _validator.ValidateAndThrowAsync(rq.Body, ct);

        var start = rq.Body.StartTime;
        var end = rq.Body.EndTime;

        if (end <= start)
            throw new ValidationException("EndTime debe ser mayor a StartTime.");

        var overlap = await _qry.ExistsOverlapAsync(
            rq.DoctorId, rq.Body.DayOfWeek, start, end, excludeId: null, ct);

        if (overlap)
            throw new ValidationException("Existe solapamiento con otra disponibilidad.");

        var entity = new Domain.Entities.DoctorAvailability
        {
            DoctorId = rq.DoctorId,
            DayOfWeek = rq.Body.DayOfWeek,
            StartTime = start,
            EndTime = end,
            DurationMinutes = rq.Body.DurationMinutes,
            IsActive = true
        };

        await _cmd.AddAsync(entity, ct);
        await _cmd.SaveChangesAsync(ct);

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
}
