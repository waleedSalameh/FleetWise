using FleetWise.BLL.DTOs.Containers.DeliveryLocations;
using FleetWise.BLL.Interface;
using FleetWise.BLL.Interface.Containers;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.API.Controllers.Containers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryLocationController : ControllerBase
    {
        private readonly IDeliveryLocation _deliveryLocationService;

        public DeliveryLocationController(IDeliveryLocation deliveryLocationService)
        {
            _deliveryLocationService = deliveryLocationService;
        }

        // GET: api/DeliveryLocation
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _deliveryLocationService.GetAllAsync();
            return Ok(result);
        }

        // POST: api/DeliveryLocation
        [HttpPost]
        public async Task<IActionResult> Create(CreateDeliveryLocationDto dto)
        {
            var result = await _deliveryLocationService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateDeliveryLocationDto dto)
        {
            try
            {
                return await _deliveryLocationService.UpdateAsync(id, dto) ? NoContent() : NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                return await _deliveryLocationService.DeleteAsync(id) ? NoContent() : NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
