using FleetWise.BLL.DTOs.Vehicles;
using FleetWise.BLL.Interface.Vehicles;
using FleetWise.BLL.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.API.Controllers.Vehicles
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicle _vehicleService;

        public VehiclesController(IVehicle vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [Authorize(Policy = "Permission:Vehicles.View")]
        [HttpGet]
        public async Task<ActionResult<List<VehicleDto>>> GetAll()
        {
            var vehicles = await _vehicleService.GetAllAsync();
            return Ok(vehicles);
        }

        [Authorize(Policy = "Permission:Vehicles.View")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<VehicleDto>> GetById(int id)
        {
            var vehicle = await _vehicleService.GetByIdAsync(id);

            if (vehicle == null)
                return NotFound(new { message = "Vehicle not found." });

            return Ok(vehicle);
        }

        [Authorize(Policy = "Permission:Vehicles.Create")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateVehicleDto dto)
        {
            try
            {
                var result = await _vehicleService.CreateAsync(dto);

                return StatusCode(StatusCodes.Status201Created, result);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = ex.Message }
                );
            }
        }

        [Authorize(Policy = "Permission:Vehicles.Update")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateVehicleDto dto)
        {
            var updated = await _vehicleService.UpdateAsync(id, dto);

            if (!updated)
                return NotFound(new { message = "Vehicle not found." });

            return NoContent();
        }

        [Authorize(Policy = "Permission:Vehicles.Delete")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _vehicleService.DeleteAsync(id);

                if (!deleted)
                    return NotFound(new { message = "Vehicle not found." });
                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
