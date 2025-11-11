using Application.Command.DoctorAvailability;
using Application.DTOs;
using Application.Interfaces;
using Application.Queries.DoctorAvailability;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SchedulingMS.Controllers
{
    [Route("/api/v1/doctors/")]
    public class AvailabilityBlockController : ControllerBase
    {
        private readonly ICreateAvailabilityBlock _createService;
        private readonly ISearchAvailabilityBlock _searchService;
        private readonly IUpdateAvailabilityBlock _updateService;

        public AvailabilityBlockController(ICreateAvailabilityBlock createService, ISearchAvailabilityBlock searchService, IUpdateAvailabilityBlock updateService)
        {
            _createService = createService;
            _searchService = searchService;
            _updateService = updateService;
        }

        [HttpPost("{doctorId}/blocks")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create(AvailabilityBlockCreate dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "The data provided is not valid." });
                }
                var newAvailabityBlock = await _createService.CreateAvailabilityBlockAsync(dto);
                return Created(string.Empty, newAvailabityBlock);
            }
            catch (InvalidOperationException argEx)
            {
                return Conflict(new { message = argEx.Message });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return BadRequest(new { message = "Error DbUpdateConcurrency." });
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new { message = "Error DbUpdate." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "A mistake has occurred." });
            }
        }

        
        [HttpGet("{doctorId}/blocks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(long DoctorId)
        {
            try
            {
                var project = await _searchService.GetByIdAsyncList(DoctorId);
                return Ok(project);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Parámetros de ordenamiento inválidos." });
            }
        }

        
        [HttpPatch("{doctorId}/blocks/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(long id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "The data provided is not valid." });
                }
                var task = await _updateService.ServiceUpdateItemStatus(id);
                return Ok(task);
            }

            catch (InvalidOperationException argEx)
            {
                return BadRequest(new { message = argEx.Message });
            }

            catch (ArgumentNullException argEx)
            {
                return BadRequest(new { message = argEx.Message });
            }

            catch (Exception ex)
            {
                return BadRequest(new { message = "A mistake has occurred." });
            }
        }
    }
}
