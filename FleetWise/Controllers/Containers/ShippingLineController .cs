using Microsoft.AspNetCore.Authorization;
using FleetWise.BLL.DTOs.Containers.ShippingLines;
using FleetWise.BLL.Interface;
using FleetWise.BLL.Interface.Containers;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingLineController : ControllerBase
    {
        private readonly IShippingLine _shippingLineService;

        public ShippingLineController(IShippingLine shippingLineService)
        {
            _shippingLineService = shippingLineService;
        }

        // GET: api/ShippingLine
        [Authorize(Policy = "Permission:ShippingLines.View")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _shippingLineService.GetAllAsync();
            return Ok(result);
        }

        // POST: api/ShippingLine
        [Authorize(Policy = "Permission:ShippingLines.Create")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateShippingLineDto dto)
        {
            var result = await _shippingLineService.CreateAsync(dto);
            return Ok(result);
        }

        [Authorize(Policy = "Permission:ShippingLines.Update")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateShippingLineDto dto)
        {
            try
            {
                return await _shippingLineService.UpdateAsync(id, dto) ? NoContent() : NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [Authorize(Policy = "Permission:ShippingLines.Delete")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                return await _shippingLineService.DeleteAsync(id) ? NoContent() : NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
