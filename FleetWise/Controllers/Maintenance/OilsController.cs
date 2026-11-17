using Microsoft.AspNetCore.Authorization;
using FleetWise.BLL.DTOs.Maintenance.Oils;
using FleetWise.BLL.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.API.Controllers.Maintenance
{
    [Route("api/[controller]")]
    [ApiController]
    public class OilsController : ControllerBase
    {
        private readonly IOilService _oilService;

        public OilsController(IOilService oilService)
        {
            _oilService = oilService;
        }

        [Authorize(Policy = "Permission:Oils.View")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var oils = await _oilService.GetAllAsync();
                return Ok(oils);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Policy = "Permission:Oils.View")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var oil = await _oilService.GetByIdAsync(id);

                if (oil == null)
                    return NotFound("Oil not found.");

                return Ok(oil);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Policy = "Permission:Oils.Create")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateOilDto dto)
        {
            try
            {
                var oil = await _oilService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = oil.OilId },
                    oil
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Policy = "Permission:Oils.Update")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateOilDto dto)
        {
            try
            {
                var updated = await _oilService.UpdateAsync(id, dto);

                if (!updated)
                    return NotFound("Oil not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Policy = "Permission:Oils.Delete")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _oilService.DeleteAsync(id);

                if (!deleted)
                    return BadRequest("Oil not found or has stock movements.");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}