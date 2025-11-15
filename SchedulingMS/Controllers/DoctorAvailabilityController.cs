using Application.DTOs;
using Application.Interfaces.IDoctorAvailability;
using Domain.Enum;
using Microsoft.AspNetCore.Mvc;

namespace SchedulingMS.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DoctorAvailabilityController : ControllerBase
    {
        private readonly ICreateDoctorAvailabilityService _createService;
        private readonly IUpdateDoctorAvailabilityService _updateService;
        private readonly ISearchDoctorAvailabilityService _searchService;

        public DoctorAvailabilityController(
            ICreateDoctorAvailabilityService createService,
            IUpdateDoctorAvailabilityService updateService,
            ISearchDoctorAvailabilityService searchService)
        {
            _createService = createService;
            _updateService = updateService;
            _searchService = searchService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _searchService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _searchService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] long? doctorId, [FromQuery] WeekDay? dayOfWeek)
        {
            var result = await _searchService.SearchAsync(doctorId, dayOfWeek);
            return Ok(result);
        }

        [HttpPost("{doctorId:long}")]
        public async Task<IActionResult> Create(long doctorId, [FromBody] DoctorAvailabilityCreate dto)
        {
            var result = await _createService.CreateAsync(doctorId, dto);
            return CreatedAtAction(nameof(GetById), new { id = result.AvailabilityId }, result);
        }

        [HttpPatch("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] DoctorAvailabilityUpdate dto)
        {
            var result = await _updateService.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var deleted = await _updateService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
