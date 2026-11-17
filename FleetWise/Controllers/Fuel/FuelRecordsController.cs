using Microsoft.AspNetCore.Authorization;
using FleetWise.BLL.DTOs.FuelRecords;
using FleetWise.BLL.Interface.Fuel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.API.Controllers.Fuel
{
    [Route("api/[controller]")]
    [ApiController]
    public class FuelRecordsController : ControllerBase
    {
        private readonly IFuelRecord _fuelRecord;

        public FuelRecordsController(IFuelRecord fuelRecord)
        {
            _fuelRecord = fuelRecord;
        }

        [Authorize(Policy = "Permission:FuelRecords.View")]
        [HttpGet]
        public async Task<ActionResult<List<FuelRecordDto>>> GetAll()
        {
            return Ok(await _fuelRecord.GetAllAsync());
        }

        [Authorize(Policy = "Permission:FuelRecords.View")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<FuelRecordDto>> GetById(int id)
        {
            var record = await _fuelRecord.GetByIdAsync(id);

            if (record == null)
                return NotFound(new { message = "Fuel record not found." });

            return Ok(record);
        }

        [Authorize(Policy = "Permission:FuelRecords.Create")]
        [HttpPost]
        public async Task<IActionResult> CreateExternal(CreateFuelRecordDto dto)
        {
            return await CreateExternalInternal(dto);
        }

        [Authorize(Policy = "Permission:FuelRecords.Create")]
        [HttpPost("external")]
        public async Task<IActionResult> CreateExternalRoute(CreateFuelRecordDto dto)
        {
            return await CreateExternalInternal(dto);
        }

        private async Task<IActionResult> CreateExternalInternal(CreateFuelRecordDto dto)
        {
            try
            {
                var result = await _fuelRecord.CreateAsync(dto);
                return StatusCode(StatusCodes.Status201Created, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "Permission:FuelRecords.Create")]
        [HttpPost("internal")]
        public async Task<IActionResult> CreateInternal(CreateInternalFuelRecordDto dto)
        {
            try
            {
                var result = await _fuelRecord.CreateInternalAsync(dto);

                return StatusCode(StatusCodes.Status201Created, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "Permission:FuelRecords.Update")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateFuelRecordDto dto)
        {
            try
            {
                var updated = await _fuelRecord.UpdateAsync(id, dto);

                if (!updated)
                    return NotFound(new { message = "Fuel record not found." });

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "Permission:FuelRecords.Delete")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _fuelRecord.DeleteAsync(id);

                if (!deleted)
                    return NotFound(new { message = "Fuel record not found." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
