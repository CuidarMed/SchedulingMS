using Application.DTOs;
using Application.Interfaces.IAvailabilityBlock;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Services.AvailabilityBlockService
{
    public class UpdateAvailabilityBlockService : IUpdateAvailabilityBlockService
    {
        private readonly IAvailabilityBlockCommand _command;
        private readonly IAvailabilityBlockQuery _query;
        private readonly IAvailabilityBlockMapper _mapper;
        private readonly IValidator<AvailabilityBlockUpdate> _validator;

        public UpdateAvailabilityBlockService(IAvailabilityBlockCommand command,IAvailabilityBlockQuery query,IAvailabilityBlockMapper mapper,IValidator<AvailabilityBlockUpdate> validator)
        {
            _command = command;
            _query = query;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<AvailabilityBlockResponse> UpdateAsync(long doctorId, long blockId, AvailabilityBlockUpdate dto)
        {
            // Validar DTO
            await _validator.ValidateAndThrowAsync(dto);
            
            // Buscar el bloqueo por ID

            var block = await _query.GetByIdAsync(doctorId, blockId);
            if (block == null)
                throw new InvalidOperationException("El bloqueo no existe.");

            // Actualizar solo los campos enviados
            if (dto.StartTime.HasValue)
                block.StartTime = dto.StartTime.Value;

            if (dto.EndTime.HasValue)
                block.EndTime = dto.EndTime.Value;

            if (dto.Reason != null)
                block.Reason = dto.Reason;

            if (dto.Note != null)
                block.Note = dto.Note;

            if (dto.AllDay.HasValue)
            {
                block.AllDay = dto.AllDay.Value;

                // Si se cambia a AllDay, ajustar horarios
                if (dto.AllDay.Value)
                {
                    block.StartTime = block.StartTime.Date;
                    block.EndTime = block.StartTime.Date.AddDays(1).AddTicks(-1);
                }
            }

            // Verificar solapamiento(excluyendo el actual)
            var hasOverlap = await _query.HasOverlapAsync(
                doctorId,
                block.StartTime,
                block.EndTime,
                excludeBlockId: blockId);

            if (hasOverlap)
            {
                throw new ValidationException("Existe solapamiento con otro bloqueo activo del doctor.");
            }
            block.UpdatedAt = DateTimeOffset.UtcNow;

            // Guardar cambios
            await _command.UpdateAsync(block);

            // Devolver el response
            return _mapper.ToResponse(block);
        }

        public async Task<bool> DeleteAsync(long doctorId, long blockId)
        {
            var block = await _query.GetByIdAsync(doctorId, blockId);
            if (block == null)
                return false;

            return await _command.DeleteAsync(doctorId, blockId);
        }
    }
}
