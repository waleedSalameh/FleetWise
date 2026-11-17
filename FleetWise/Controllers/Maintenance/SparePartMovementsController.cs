using Microsoft.AspNetCore.Authorization;
using FleetWise.BLL.DTOs.Maintenance.SparePartMovements;
using FleetWise.BLL.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.API.Controllers.Maintenance
{
    [Route("api/[controller]")]
    [ApiController]
    public class SparePartMovementsController : ControllerBase
    {
        private readonly ISparePartMovementService _service;

        public SparePartMovementsController(ISparePartMovementService service)
        {
            _service = service;
        }

        // GET: api/SparePartMovements
        [Authorize(Policy = "Permission:SparePartMovements.View")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var movements = await _service.GetAllAsync();
                return Ok(movements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/SparePartMovements/5
        [Authorize(Policy = "Permission:SparePartMovements.View")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var movement = await _service.GetByIdAsync(id);

                if (movement == null)
                    return NotFound("Movement not found.");

                return Ok(movement);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/SparePartMovements/spare-part/5
        [Authorize(Policy = "Permission:SparePartMovements.View")]
        [HttpGet("spare-part/{sparePartId}")]
        public async Task<IActionResult> GetBySparePartId(int sparePartId)
        {
            try
            {
                var movements = await _service.GetBySparePartIdAsync(sparePartId);
                return Ok(movements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/SparePartMovements/stock/5
        [Authorize(Policy = "Permission:SparePartMovements.View")]
        [HttpGet("stock/{sparePartId}")]
        public async Task<IActionResult> GetCurrentStock(int sparePartId)
        {
            try
            {
                var stock = await _service.GetCurrentStockAsync(sparePartId);
                return Ok(stock);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/SparePartMovements
        [Authorize(Policy = "Permission:SparePartMovements.Create")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateSparePartMovementDto dto)
        {
            try
            {
                var movement = await _service.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = movement.SparePartMovementId },
                    movement
                );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}