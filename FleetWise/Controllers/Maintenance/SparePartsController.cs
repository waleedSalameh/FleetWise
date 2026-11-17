using Microsoft.AspNetCore.Authorization;
using FleetWise.BLL.DTOs.Maintenance.SpareParts;
using FleetWise.BLL.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.API.Controllers.Maintenance
{
    [Route("api/[controller]")]
    [ApiController]
    public class SparePartsController : ControllerBase
    {
        private readonly ISparePartService _sparePartService;

        public SparePartsController(ISparePartService sparePartService)
        {
            _sparePartService = sparePartService;
        }

        // GET: api/SpareParts
        [Authorize(Policy = "Permission:SpareParts.View")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var spareParts = await _sparePartService.GetAllAsync();
                return Ok(spareParts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/SpareParts/5
        [Authorize(Policy = "Permission:SpareParts.View")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var sparePart = await _sparePartService.GetByIdAsync(id);

                if (sparePart == null)
                    return NotFound("Spare part not found.");

                return Ok(sparePart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/SpareParts
        [Authorize(Policy = "Permission:SpareParts.Create")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateSparePartDto dto)
        {
            try
            {
                var sparePart = await _sparePartService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = sparePart.SparePartId },
                    sparePart
                );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/SpareParts/5
        [Authorize(Policy = "Permission:SpareParts.Update")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateSparePartDto dto)
        {
            try
            {
                var updated = await _sparePartService.UpdateAsync(id, dto);

                if (!updated)
                    return NotFound("Spare part not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/SpareParts/5
        [Authorize(Policy = "Permission:SpareParts.Delete")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _sparePartService.DeleteAsync(id);

                if (!deleted)
                    return BadRequest("Spare part not found or has stock movements.");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}