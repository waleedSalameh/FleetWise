using FleetWise.BLL.DTOs.Vehicles.ExternalVehicles;
using FleetWise.BLL.Interface.Vehicles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.API.Controllers.Vehicles
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExternalVehiclesController : ControllerBase
    {
        private readonly IExternalVehicles _service;

        public ExternalVehiclesController(IExternalVehicles service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<ExternalVehiclesResponseDto>>> GetAll()
        {
            var vehicles = await _service.GetAllAsync();
            return Ok(vehicles);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ExternalVehiclesResponseDto>> GetById(int id)
        {
            var vehicle = await _service.GetByIdAsync(id);

            if (vehicle == null)
                return NotFound(new
                {
                    message = "External vehicle not found."
                });

            return Ok(vehicle);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateExternalVehiclesDto dto)
        {
            try
            {
                var result = await _service.CreateAsync(dto);

                return StatusCode(
                    StatusCodes.Status201Created,
                    result
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        message = ex.Message
                    }
                );
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id);

                if (!deleted)
                    return NotFound(new
                    {
                        message = "External vehicle not found."
                    });

                return StatusCode(StatusCodes.Status204NoContent);
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