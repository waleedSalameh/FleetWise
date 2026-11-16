using FleetWise.BLL.DTOs.Containers.ExportAqaba;
using FleetWise.BLL.Interface;
using FleetWise.BLL.Interface.Containers.IncomingAqaba;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.API.Controllers.Containers.IncomingAqaba
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncomingAqabaBillController : ControllerBase
    {
        private readonly IIncomingAqabaBill _billService;

        public IncomingAqabaBillController(
            IIncomingAqabaBill billService)
        {
            _billService = billService;
        }

        // GET: api/IncomingAqabaBill
        [HttpGet]
        public async Task<ActionResult<List<IncomingAqabaBillResponseDto>>> GetAll()
        {
            try
            {
                var bills = await _billService.GetAllAsync();

                return Ok(bills);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = ex.Message });
            }
        }

        // GET: api/IncomingAqabaBill/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<IncomingAqabaBillResponseDto>> GetById(int id)
        {
            try
            {
                var bill = await _billService.GetByIdAsync(id);

                if (bill == null)
                    return NotFound(new
                    {
                        message = "Bill not found."
                    });

                return Ok(bill);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = ex.Message });
            }
        }

        // POST: api/IncomingAqabaBill
        [HttpPost]
        public async Task<IActionResult> Create(
            CreateIncomingAqabaBillDto dto)
        {
            try
            {
                var result = await _billService.CreateAsync(dto);

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

        // PUT: api/IncomingAqabaBill/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateIncomingAqabaBillDto dto)
        {
            try
            {
                var updated = await _billService.UpdateAsync(id, dto);

                if (!updated)
                    return NotFound(new
                    {
                        message = "Bill not found."
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

        // DELETE: api/IncomingAqabaBill/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _billService.DeleteAsync(id);

                if (!deleted)
                    return NotFound(new
                    {
                        message = "Bill not found."
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