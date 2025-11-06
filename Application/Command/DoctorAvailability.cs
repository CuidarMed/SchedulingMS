using System;
using Application.DTOs;
using Application.Interfaces.Command;
using Application.Interfaces.Queries;
using FluentValidation;
using MediatR;

namespace Application.Command.DoctorAvailability;

public sealed class UpdateDoctorAvailabilityHandler
    : IRequestHandler<UpdateDoctorAvailabilityCommand, DoctorAvailabilityResponse>
{
    private readonly IDoctorAvailabilityCommandRepository _cmd;
    private readonly IDoctorAvailabilityQueryRepository _qry;

    public UpdateDoctorAvailabilityHandler(
        IDoctorAvailabilityCommandRepository cmd,
        IDoctorAvailabilityQueryRepository qry)
    {
        _cmd = cmd ?? throw new ArgumentNullException(nameof(cmd));
        _qry = qry ?? throw new ArgumentNullException(nameof(qry));
    }

    public async Task<DoctorAvailabilityResponse> Handle(UpdateDoctorAvailabilityCommand rq, CancellationToken ct)
    {
        var entity = await _qry.GetByIdAsync(rq.DoctorId, rq.AvailabilityId, ct)
                    ?? throw new FluentValidation.ValidationException("Disponibilidad no encontrada para este doctor.");

        var newStart = rq.Body.StartTime;          
        var newEnd = rq.Body.EndTime;            
        var newDur = rq.Body.DurationMinutes;
        var newActive = rq.Body.IsActive ?? entity.IsActive;

        if (newEnd <= newStart)
            throw new FluentValidation.ValidationException("EndTime debe ser mayor a StartTime.");

        var overlap = await _qry.ExistsOverlapAsync(
            rq.DoctorId, entity.DayOfWeek, newStart, newEnd, excludeId: entity.AvailabilityId, ct);

        if (overlap)
            throw new FluentValidation.ValidationException("Se solapa con otra disponibilidad.");

        entity.StartTime = newStart;
        entity.EndTime = newEnd;
        entity.DurationMinutes = newDur;
        entity.IsActive = newActive;
        entity.UpdatedAt = DateTimeOffset.UtcNow;

        await _cmd.UpdateAsync(entity, ct);
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
