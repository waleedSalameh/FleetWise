using FleetWise.BLL.DTOs.Containers.InlandTransport;
using FleetWise.BLL.Interface.Containers.InlandTransport;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.API.Controllers.Containers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InlandTransportController : ControllerBase
    {
        private readonly IInlandTransport _service;

        public InlandTransportController(IInlandTransport service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
                return NotFound(new { message = "Inland transport not found." });

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateInlandTransportDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateInlandTransportDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}