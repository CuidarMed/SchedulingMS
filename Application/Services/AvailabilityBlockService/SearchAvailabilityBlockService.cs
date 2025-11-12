using Application.DTOs;
using Application.Interfaces.IAvailabilityBlock;
using Domain.Entities;
using FluentValidation;

namespace Application.Services.AvailabilityBlockService
{
    public class SearchAvailabilityBlockService:ISearchAvailabilityBlockService
    {
        private readonly IAvailabilityBlockQuery _query;
        private readonly IAvailabilityBlockMapper _mapper;
        public SearchAvailabilityBlockService(IAvailabilityBlockQuery query, IAvailabilityBlockMapper mapper)
        {
            _query = query;
            _mapper = mapper;
           
        }
        public async Task<AvailabilityBlockResponse?> GetByIdAsync(long doctorId, long blockId)
        {
            var block = await _query.GetByIdAsync(doctorId, blockId);
            if (block == null)
            {
                throw new KeyNotFoundException(
                    $"Bloqueo {blockId} no encontrado para el doctor {doctorId}.");
            }

            return _mapper.ToResponse(block);
        }
        async Task<List<AvailabilityBlockResponse>> ISearchAvailabilityBlockService.GetByDoctorAsync(long doctorId)
        {
            var blocks = await _query.GetByDoctorAsync(doctorId);
            return _mapper.ToResponseList(blocks);
        }
    }
}
