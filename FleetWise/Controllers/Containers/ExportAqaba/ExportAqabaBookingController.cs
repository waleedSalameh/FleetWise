using Microsoft.AspNetCore.Authorization;
using FleetWise.BLL.DTOs.Containers.ExportAqaba;
using FleetWise.BLL.Interface.Containers;
using FleetWise.BLL.Interface.Containers.ExportAqaba;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportAqabaBookingController : ControllerBase
    {
        private readonly IExportAqabaBooking _bookingService;

        public ExportAqabaBookingController(IExportAqabaBooking bookingService)
        {
            _bookingService = bookingService;
        }

        [Authorize(Policy = "Permission:ExportAqabaBookings.View")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bookings = await _bookingService.GetAllAsync();
            return Ok(bookings);
        }

        [Authorize(Policy = "Permission:ExportAqabaBookings.View")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var booking = await _bookingService.GetByIdAsync(id);

            if (booking == null)
                return NotFound(new { message = "Booking not found." });

            return Ok(booking);
        }

        [Authorize(Policy = "Permission:ExportAqabaBookings.Create")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateExportAqabaBookingDto dto)
        {
            try
            {
                var booking = await _bookingService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = booking.Id },
                    booking);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "Permission:ExportAqabaBookings.Update")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateExportAqabaBookingDto dto)
        {
            try
            {
                var result = await _bookingService.UpdateAsync(id, dto);

                if (!result)
                    return NotFound(new { message = "Booking not found." });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "Permission:ExportAqabaBookings.Delete")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _bookingService.DeleteAsync(id);

            if (!result)
                return NotFound(new { message = "Booking not found." });

            return NoContent();
        }
    }
}