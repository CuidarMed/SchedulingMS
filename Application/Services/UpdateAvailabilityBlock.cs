using Application.DTOs;
using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UpdateAvailabilityBlock : IUpdateAvailabilityBlock
    {
        private readonly IAvailabilityBlockCommand command;
        private readonly IAvailabilityBlockQuery query;

        public UpdateAvailabilityBlock(IAvailabilityBlockCommand command, IAvailabilityBlockQuery query)
        {
            this.command = command;
            this.query = query;
        }

        public async Task<AvailabilityBlockUpdate> Update(long DoctorId, AvailabilityBlockUpdate availabilityBlock)
        {
            var block = await query.GetByIdAsync(DoctorId);
            block.StartTime = availabilityBlock.StartTime;  
            block.EndTime = availabilityBlock.EndTime;
            block.Reason = availabilityBlock.Reason;
            block.IsBlock = availabilityBlock.IsActive ?? block.IsBlock;

            await command.UpdateAsync(block);

            var BlockResponseDto = new AvailabilityBlockUpdate
            {
                StartTime = block.StartTime,
                EndTime = block.EndTime,
                Reason = block.Reason,
                IsActive = block.IsBlock
            };

            return BlockResponseDto;
        }
    }
}
