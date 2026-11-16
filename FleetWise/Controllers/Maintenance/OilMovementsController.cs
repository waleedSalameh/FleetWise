using FleetWise.BLL.DTOs.Maintenance.OilMovements;
using FleetWise.BLL.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.API.Controllers.Maintenance
{
    [Route("api/[controller]")]
    [ApiController]
    public class OilMovementsController : ControllerBase
    {
        private readonly IOilMovementService _oilMovementService;

        public OilMovementsController(IOilMovementService oilMovementService)
        {
            _oilMovementService = oilMovementService;
        }

        // GET: api/OilMovements
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var movements = await _oilMovementService.GetAllAsync();
                return Ok(movements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/OilMovements/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var movement = await _oilMovementService.GetByIdAsync(id);

                if (movement == null)
                    return NotFound("Oil movement not found.");

                return Ok(movement);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/OilMovements/oil/1
        [HttpGet("oil/{oilId}")]
        public async Task<IActionResult> GetByOilId(int oilId)
        {
            try
            {
                var movements = await _oilMovementService.GetByOilIdAsync(oilId);
                return Ok(movements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/OilMovements/stock/1
        [HttpGet("stock/{oilId}")]
        public async Task<IActionResult> GetCurrentStock(int oilId)
        {
            try
            {
                var stock = await _oilMovementService.GetCurrentStockAsync(oilId);
                return Ok(stock);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: api/OilMovements
        [HttpPost]
        public async Task<IActionResult> Create(CreateOilMovementDto dto)
        {
            try
            {
                var movement = await _oilMovementService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = movement.OilStockMovementId },
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