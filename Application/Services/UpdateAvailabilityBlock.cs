using Application.DTOs;
using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UpdateAvailabilityBlock:IUpdateAvailabilityBlock
    {
        private readonly IAvailabilityBlockCommand command;
        private readonly IAvailabilityBlockQuery query;

        public UpdateAvailabilityBlock(IAvailabilityBlockCommand command, IAvailabilityBlockQuery query)
        {
            this.command = command;
            this.query = query;
        }

        public async Task<AvailabilityBlockUpdate> ServiceUpdateDishTask(long DoctorId, AvailabilityBlockUpdate DishDto)
        {
            var block = await query.GetByIdAsync(DoctorId);
            dish.Name = DishDto.name;
            dish.Description = DishDto.description;
            dish.Price = DishDto.price;
            dish.Category = DishDto.category;
            dish.ImageUrl = DishDto.image;
            dish.Available = DishDto.isActive;
            dish.UpdateDate = DateTime.Now;

            await command.CommandUpdateDishTask(dish);

            var result = await query.BuscarId(Id);

            var DishResponseDto = new DishResponseDto
            {
                id = result.DishId,
                name = result.Name,
                description = result.Description,
                price = result.Price,
                category = new CategoryResponseDto
                {
                    id = result.CategoryIns.Id,
                    name = result.CategoryIns.Name,
                },
                image = result.ImageUrl,
                isActive = result.Available,
                createdAt = result.CreateDate,
                updatedAt = result.UpdateDate,
            };

            return DishResponseDto;
        }
}
