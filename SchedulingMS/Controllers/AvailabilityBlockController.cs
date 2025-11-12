using Application.DTOs;
using Application.Interfaces.IAvailabilityBlock;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SchedulingMS.Controllers
{
    [ApiController]
    [Route("api/v1/doctors/{doctorId:long}/blocks")]
    public class AvailabilityBlockController : ControllerBase
    {
        private readonly ICreateAvailabilityBlockService _createService;
        private readonly ISearchAvailabilityBlockService _searchService;
        private readonly IUpdateAvailabilityBlockService _updateService;

        public AvailabilityBlockController(
            ICreateAvailabilityBlockService createService,
            ISearchAvailabilityBlockService searchService,
            IUpdateAvailabilityBlockService updateService)
        {
            _createService = createService;
            _searchService = searchService;
            _updateService = updateService;
        }

        // POST /api/v1/doctors/{doctorId}/blocks
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromRoute] long doctorId, [FromBody] AvailabilityBlockCreate dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Los datos enviados no son válidos." });

                var newBlock = await _createService.CreateAsync(doctorId, dto);
                return CreatedAtAction(nameof(GetByDoctor), new { doctorId }, newBlock);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { message = "Error al guardar el bloqueo en la base de datos." });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Ha ocurrido un error inesperado." });
            }
        }

        // GET /api/v1/doctors/{doctorId}/blocks
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByDoctor([FromRoute] long doctorId)
        {
            try
            {
                var blocks = await _searchService.GetByDoctorAsync(doctorId);
                return Ok(blocks);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "No se pudieron obtener los bloqueos." });
            }
        }

        // PATCH /api/v1/doctors/{doctorId}/blocks/{id}
        [HttpPatch("{id:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] long doctorId, [FromRoute] long id, [FromBody] AvailabilityBlockUpdate dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Los datos enviados no son válidos." });

                var updated = await _updateService.UpdateAsync(doctorId, id, dto);
                if (updated == null)
                    return NotFound(new { message = "El bloqueo no existe." });

                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Ha ocurrido un error inesperado." });
            }
        }
    }
}
