using Application.DTOs;
using Application.Interfaces.IAvailabilityBlock;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.AvailabilityBlockService
{
    public class CreateAvailabilityBlockService:ICreateAvailabilityBlockService
    {
        private readonly IAvailabilityBlockCommand _command;
        private readonly IAvailabilityBlockQuery _query;
        private readonly IAvailabilityBlockMapper _mapper;
        private readonly IValidator<AvailabilityBlockCreate> _validator;
        public CreateAvailabilityBlockService(IAvailabilityBlockCommand comand, IAvailabilityBlockQuery query, IAvailabilityBlockMapper mapper, IValidator<AvailabilityBlockCreate> validator)
        {
            _command = comand;
            _query = query;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<AvailabilityBlockResponse> CreateAsync(long doctorId, AvailabilityBlockCreate dto)
        {
            // Validar DTO
            await _validator.ValidateAndThrowAsync(dto);
            // Crear entidad
            var block = new AvailabilityBlock
            {
                DoctorId = doctorId,
                StartTime = (DateTimeOffset)dto.StartTime,
                EndTime = (DateTimeOffset)dto.EndTime,
                Reason = dto.Reason,
                Note = dto.Note,
                AllDay = dto.AllDay,
                IsBlock = true,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            var created = await _command.CreateAsync(block);
            return _mapper.ToResponse(created);
        }
    }
}
