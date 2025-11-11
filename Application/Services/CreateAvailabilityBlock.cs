using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CreateAvailabilityBlock:ICreateAvailabilityBlock
    {
        private readonly IAvailabilityBlockCommand _comand;
        private readonly IAvailabilityBlockQuery _query;   

        public CreateAvailabilityBlock(IAvailabilityBlockCommand comand, IAvailabilityBlockQuery query)
        {
            _comand = comand;
            _query = query;
        }

        public async Task<AvailabilityBlockResponse> CreateAvailabilityBlockAsync(AvailabilityBlockCreate request)
        {
            // Crear la entidad
            var block = new AvailabilityBlock
            {
                DoctorId = 1,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Reason = request.Reason,
                IsBlock = true,
                Note = null,
                AllDay = false
            };

            // Guardar en la base de datos
            block = await _comand.CreateAsync(block);

            // Retornar la respuesta
            return new AvailabilityBlockResponse
            {
                BlockId = block.BlockId,
                DoctorId = block.DoctorId,
                StartTime = block.StartTime,
                EndTime = block.EndTime,
                Reason = block.Reason,
                IsActive = block.IsBlock,
                CreatedAt = block.CreatedAt,
                UpdatedAt = block.UpdatedAt
            };
        }
    }
}
