using FleetWise.BLL.DTOs.Containers.InternationalExport;
using FleetWise.BLL.Interface.Containers.IInternationalExport;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.API.Controllers.InternationalExport
{
    [Route("api/[controller]")]
    [ApiController]
    public class InternationalExportController : ControllerBase
    {
        private readonly IInternationalExport _service;

        public InternationalExportController(IInternationalExport service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var exports = await _service.GetAllAsync();
            return Ok(exports);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var export = await _service.GetByIdAsync(id);

            if (export == null)
                return NotFound(new { message = "Export record not found." });

            return Ok(export);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateInternationalExportDto dto)
        {
            var export = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = export.Id }, export);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateInternationalExportDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);

            if (!result)
                return NotFound(new { message = "Export record not found." });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result)
                return NotFound(new { message = "Export record not found." });

            return NoContent();
        }
    }
}