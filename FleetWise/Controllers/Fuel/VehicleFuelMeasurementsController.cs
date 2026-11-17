using Microsoft.AspNetCore.Authorization;
using FleetWise.BLL.DTOs.FuelRecords;
using FleetWise.BLL.Interface.Fuel;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.API.Controllers.Fuel;

[ApiController, Route("api/vehicles/{vehicleId:int}/fuel-measurements")]
public class VehicleFuelMeasurementsController(IVehicleFuelMeasurementService service)
    : ControllerBase
{
    [Authorize(Policy = "Permission:VehicleFuelMeasurements.View")]
    [HttpGet]
    public async Task<ActionResult<List<FuelMeasurementDto>>> History(int vehicleId) =>
        Ok(await service.GetHistoryAsync(vehicleId));

    [Authorize(Policy = "Permission:VehicleFuelMeasurements.Create")]
    [HttpPost]
    public async Task<ActionResult<FuelMeasurementDto>> Create(
        int vehicleId,
        CreateFuelMeasurementDto dto
    )
    {
        if (vehicleId != dto.VehicleId)
            return BadRequest(new { message = "Vehicle ID does not match route." });
        try
        {
            return Ok(await service.CreateAsync(dto));
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

    [Authorize(Policy = "Permission:VehicleFuelMeasurements.Approve")]
    [HttpPost("{id:long}/approve")]
    public async Task<ActionResult<FuelMeasurementDto>> Approve(
        long id,
        ApproveFuelMeasurementDto dto
    )
    {
        try
        {
            return Ok(await service.ApproveAsync(id, dto.ApprovedBy));
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
