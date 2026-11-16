using FleetWise.BLL.DTOs.Drivers;
using FleetWise.BLL.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriversController : ControllerBase
    {
        private readonly IDriver _driverService;

        public DriversController(IDriver driverService)
        {
            _driverService = driverService;
        }

       
        [HttpGet]
        public async Task<ActionResult<List<DriverDto>>> GetAll()
        {
            try
            {
                var drivers = await _driverService.GetAllAsync();
                return Ok(drivers);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = ex.Message });
            }
        }

       
        [HttpGet("{id:int}")]
        public async Task<ActionResult<DriverDto>> GetById(int id)
        {
            try
            {
                var driver = await _driverService.GetByIdAsync(id);

                if (driver == null)
                    return NotFound(new
                    {
                        message = "Driver not found."
                    });

                return Ok(driver);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = ex.Message });
            }
        }

       
        [HttpPost]
        public async Task<IActionResult> Create(CreateDriverDto dto)
        {
            try
            {
                var result = await _driverService.CreateAsync(dto);

                return StatusCode(
                    StatusCodes.Status201Created,
                    result);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = ex.Message });
            }
        }

      
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateDriverDto dto)
        {
            try
            {
                var updated = await _driverService.UpdateAsync(id, dto);

                if (!updated)
                    return NotFound(new
                    {
                        message = "Driver not found."
                    });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _driverService.DeleteAsync(id);

                if (!deleted)
                    return NotFound(new
                    {
                        message = "Driver not found."
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