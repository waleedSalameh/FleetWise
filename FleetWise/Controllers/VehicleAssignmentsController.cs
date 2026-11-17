using Microsoft.AspNetCore.Authorization;
using FleetWise.BLL.DTOs.Vehicles.VehicleAssignments;
using FleetWise.BLL.Interface.Vehicles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleAssignmentsController : ControllerBase
    {
        private readonly IVehicleAssignment _vehicleAssignment;

        public VehicleAssignmentsController(
            IVehicleAssignment vehicleAssignment)
        {
            _vehicleAssignment = vehicleAssignment;
        }

        [Authorize(Policy = "Permission:VehicleAssignments.View")]
        [HttpGet]
        public async Task<ActionResult<List<VehicleAssignmentDto>>> GetAll()
        {
            var assignments = await _vehicleAssignment.GetAllAsync();
            return Ok(assignments);
        }

        [Authorize(Policy = "Permission:VehicleAssignments.View")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<VehicleAssignmentDto>> GetById(int id)
        {
            var assignment = await _vehicleAssignment.GetByIdAsync(id);

            if (assignment == null)
                return NotFound(new
                {
                    message = "Vehicle assignment not found."
                });

            return Ok(assignment);
        }

        [Authorize(Policy = "Permission:VehicleAssignments.Create")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateVehicleAssignmentDto dto)
        {
            try
            {
                var result = await _vehicleAssignment.CreateAsync(dto);

                return StatusCode(
                    StatusCodes.Status201Created,
                    result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        message = ex.Message
                    });
            }
        }

        [Authorize(Policy = "Permission:VehicleAssignments.Update")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateVehicleAssignmentDto dto)
        {
            try
            {
                var updated = await _vehicleAssignment.UpdateAsync(id, dto);

                if (!updated)
                    return NotFound(new
                    {
                        message = "Vehicle assignment not found."
                    });

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        message = ex.Message
                    });
            }
        }

        [Authorize(Policy = "Permission:VehicleAssignments.Delete")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _vehicleAssignment.DeleteAsync(id);

                if (!deleted)
                    return NotFound(new
                    {
                        message = "Vehicle assignment not found."
                    });

                return NoContent();
            }
            catch (Exception ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
