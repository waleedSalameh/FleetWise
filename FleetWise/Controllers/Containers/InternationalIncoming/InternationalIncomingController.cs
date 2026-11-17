using Microsoft.AspNetCore.Authorization;
using FleetWise.BLL.DTOs.Containers.InternationalIncoming;
using FleetWise.BLL.Interface.Containers.InternationalIncoming;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.API.Controllers.Containers.InternationalIncoming
{
    [Route("api/[controller]")]
    [ApiController]
    public class InternationalIncomingController : ControllerBase
    {
        private readonly IInternationalIncoming _incomingService;

        public InternationalIncomingController(IInternationalIncoming incomingService)
        {
            _incomingService = incomingService;
        }

        [Authorize(Policy = "Permission:InternationalIncoming.View")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _incomingService.GetAllAsync());
        }

        [Authorize(Policy = "Permission:InternationalIncoming.View")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var incoming = await _incomingService.GetByIdAsync(id);

            if (incoming == null)
                return NotFound(new { message = "International incoming record not found." });

            return Ok(incoming);
        }

        [Authorize(Policy = "Permission:InternationalIncoming.Create")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInternationalIncomingDto dto)
        {
            try
            {
                var incoming = await _incomingService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = incoming.Id }, incoming);
            }
            catch (InvalidOperationException exception)
            {
                return Conflict(new { message = exception.Message });
            }
        }

        [Authorize(Policy = "Permission:InternationalIncoming.Update")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateInternationalIncomingDto dto)
        {
            try
            {
                bool updated = await _incomingService.UpdateAsync(id, dto);

                if (!updated)
                    return NotFound(new { message = "International incoming record not found." });

                return NoContent();
            }
            catch (InvalidOperationException exception)
            {
                return Conflict(new { message = exception.Message });
            }
        }

        [Authorize(Policy = "Permission:InternationalIncoming.Delete")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await _incomingService.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { message = "International incoming record not found." });

            return NoContent();
        }
    }
}
