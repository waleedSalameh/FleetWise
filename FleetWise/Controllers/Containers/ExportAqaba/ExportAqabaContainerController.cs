using Microsoft.AspNetCore.Authorization;
using FleetWise.BLL.DTOs.Containers.ExportAqaba;
using FleetWise.BLL.Interface.Containers;
using FleetWise.BLL.Interface.Containers.ExportAqaba;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportAqabaContainerController : ControllerBase
    {
        private readonly IExportAqabaContainer _containerService;

        public ExportAqabaContainerController(
            IExportAqabaContainer containerService)
        {
            _containerService = containerService;
        }

        [Authorize(Policy = "Permission:ExportAqabaContainers.View")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var container = await _containerService.GetByIdAsync(id);

            if (container == null)
                return NotFound(new { message = "Container not found." });

            return Ok(container);
        }

        [Authorize(Policy = "Permission:ExportAqabaContainers.Create")]
        [HttpPost("{bookingId}")]
        public async Task<IActionResult> Create(
            int bookingId,
            CreateExportAqabaContainerDto dto)
        {
            try
            {
                var container = await _containerService.CreateAsync(
                    bookingId,
                    dto);

                return Ok(container);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "Permission:ExportAqabaContainers.Update")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateExportAqabaContainerDto dto)
        {
            try
            {
                var result = await _containerService.UpdateAsync(id, dto);

                if (!result)
                    return NotFound(new { message = "Container not found." });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "Permission:ExportAqabaContainers.Delete")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _containerService.DeleteAsync(id);

            if (!result)
                return NotFound(new { message = "Container not found." });

            return NoContent();
        }
    }
}