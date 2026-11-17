using Microsoft.AspNetCore.Authorization;

using FleetWise.BLL.DTOs.Trips;
using FleetWise.BLL.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController : ControllerBase
    {
        private readonly ITrip _tripService;

        public TripsController(ITrip tripService)
        {
            _tripService = tripService;
        }

        [Authorize(Policy = "Permission:Trips.View")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var trips = await _tripService.GetAllAsync();

            return Ok(trips);
        }

        [Authorize(Policy = "Permission:Trips.View")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var trip = await _tripService.GetByIdAsync(id);

            if (trip == null)
            {
                return NotFound(new
                {
                    message = "Trip not found."
                });
            }

            return Ok(trip);
        }

        [Authorize(Policy = "Permission:Trips.Create")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateTripDto dto)
        {
            try
            {
                var tripId = await _tripService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = tripId },
                    new
                    {
                        tripId,
                        message = "Trip created successfully."
                    });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        [Authorize(Policy = "Permission:Trips.Update")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateTripDto dto)
        {
            try
            {
                var updated = await _tripService.UpdateAsync(id, dto);

                if (!updated)
                {
                    return NotFound(new
                    {
                        message = "Trip not found."
                    });
                }

                return Ok(new
                {
                    message = "Trip updated successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        [Authorize(Policy = "Permission:Trips.Delete")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _tripService.DeleteAsync(id);

                if (!deleted)
                {
                    return NotFound(new
                    {
                        message = "Trip not found."
                    });
                }

                return Ok(new
                {
                    message = "Trip deleted successfully."
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
