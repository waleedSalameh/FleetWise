using Microsoft.AspNetCore.Authorization;

using FleetWise.BLL.DTOs.Maintenance.MaintenanceRecords;
using FleetWise.BLL.Interface.Maintenance;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.API.Controllers.Maintenance
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceRecordsController : ControllerBase
    {
        private readonly IMaintenanceRecord _maintenanceService;

        public MaintenanceRecordsController(
            IMaintenanceRecord maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        // GET: api/MaintenanceRecords
        [Authorize(Policy = "Permission:Maintenance.View")]
        [HttpGet]
        public async Task<ActionResult<List<MaintenanceRecordDto>>> GetAll()
        {
            try
            {
                var records = await _maintenanceService.GetAllAsync();

                return Ok(records);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = ex.Message });
            }
        }

        // GET: api/MaintenanceRecords/5
        [Authorize(Policy = "Permission:Maintenance.View")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<MaintenanceRecordDto>> GetById(int id)
        {
            try
            {
                var record = await _maintenanceService.GetByIdAsync(id);

                if (record == null)
                {
                    return NotFound(new
                    {
                        message = "Maintenance record not found."
                    });
                }

                return Ok(record);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = ex.Message });
            }
        }

        // POST: api/MaintenanceRecords
        [Authorize(Policy = "Permission:Maintenance.Create")]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateMaintenanceRecordDto dto)
        {
            try
            {
                var result = await _maintenanceService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result.MaintenanceRecordId },
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
                    new { message = ex.Message });
            }
        }

        // PUT: api/MaintenanceRecords/5
        [Authorize(Policy = "Permission:Maintenance.Update")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateMaintenanceRecordDto dto)
        {
            try
            {
                var updated = await _maintenanceService.UpdateAsync(id, dto);

                if (updated == null)
                {
                    return NotFound(new
                    {
                        message = "Maintenance record not found."
                    });
                }

                return Ok(updated);
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
                    new { message = ex.Message });
            }
        }

        // DELETE: api/MaintenanceRecords/5
        [Authorize(Policy = "Permission:Maintenance.Delete")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _maintenanceService.DeleteAsync(id);

                if (!deleted)
                {
                    return NotFound(new
                    {
                        message = "Maintenance record not found."
                    });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = ex.Message });
            }
        }
    }
}
