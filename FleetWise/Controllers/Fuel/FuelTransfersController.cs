using Microsoft.AspNetCore.Authorization;
using FleetWise.BLL.DTOs.FuelRecords;
using FleetWise.BLL.Interface.Fuel;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.API.Controllers.Fuel;

[ApiController, Route("api/[controller]")]
public class FuelTransfersController(IFuelTransferService service) : ControllerBase
{
    [Authorize(Policy = "Permission:FuelTransfers.View")]
    [HttpGet]
    public async Task<ActionResult<List<FuelTransferDto>>> GetAll() =>
        Ok(await service.GetAllAsync());

    [Authorize(Policy = "Permission:FuelTransfers.View")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<FuelTransferDto>> Get(int id) =>
        (await service.GetByIdAsync(id)) is { } x ? Ok(x) : NotFound();

    [Authorize(Policy = "Permission:FuelTransfers.Create")]
    [HttpPost]
    public async Task<ActionResult<FuelTransferDto>> Create(CreateFuelTransferDto dto)
    {
        try
        {
            var result = await service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (InvalidOperationException e)
        {
            return Conflict(new { message = e.Message });
        }
    }

    [Authorize(Policy = "Permission:FuelTransfers.Reverse")]
    [HttpPost("{id:int}/reverse")]
    public async Task<ActionResult<FuelTransferDto>> Reverse(int id, [FromBody] string notes)
    {
        try
        {
            return Ok(await service.ReverseAsync(id, notes));
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (InvalidOperationException e)
        {
            return Conflict(new { message = e.Message });
        }
    }

    [Authorize(Policy = "Permission:FuelTransfers.Delete")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await service.ReverseAsync(id, string.Empty);
            return NoContent();
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (InvalidOperationException e)
        {
            return Conflict(new { message = e.Message });
        }
    }
}
