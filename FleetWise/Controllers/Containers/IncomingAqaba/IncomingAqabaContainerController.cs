using FleetWise.BLL.DTOs.Containers.ExportAqaba;
using FleetWise.BLL.Interface.Containers.IncomingAqaba;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.API.Controllers.Containers.IncomingAqaba
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncomingAqabaContainerController : ControllerBase
    {
        private readonly IIncomingAqabaContainer _service;

        public IncomingAqabaContainerController(
            IIncomingAqabaContainer service)
        {
            _service = service;
        }

        // GET: api/IncomingAqabaContainer/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);

                if (result == null)
                    return NotFound(new { message = "Container not found." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST: api/IncomingAqabaContainer/5
        // Add container to bill with ID 5
        [HttpPost("{billId}")]
        public async Task<IActionResult> Create(
            int billId,
            [FromBody] CreateIncomingAqabaContainerDto dto)
        {
            try
            {
                var result = await _service.CreateAsync(billId, dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result.Id },
                    result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PUT: api/IncomingAqabaContainer/10
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateIncomingAqabaContainerDto dto)
        {
            try
            {
                bool result = await _service.UpdateAsync(id, dto);

                if (!result)
                    return NotFound(new { message = "Container not found." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // DELETE: api/IncomingAqabaContainer/10
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                bool result = await _service.DeleteAsync(id);

                if (!result)
                    return NotFound(new { message = "Container not found." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}