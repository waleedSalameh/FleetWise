using Microsoft.AspNetCore.Authorization;
using FleetWise.BLL.DTOs.FuelRecords;
using FleetWise.BLL.Interface.Fuel;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.API.Controllers.Fuel;

[ApiController, Route("api/[controller]")]
public class FuelReportsController(IFuelReportService service) : ControllerBase
{
    [Authorize(Policy = "Permission:FuelReports.View")]
    [HttpGet("transactions")]
    public async Task<ActionResult<List<FuelTransactionDto>>> Transactions(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] int? vehicleId
    )
    {
        try
        {
            return Ok(await service.GetTransactionsAsync(startDate, endDate, vehicleId));
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [Authorize(Policy = "Permission:FuelReports.View")]
    [HttpGet("monthly")]
    public async Task<ActionResult<List<MonthlyFuelReportDto>>> Monthly(
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate,
        [FromQuery] int? vehicleId
    )
    {
        try
        {
            return Ok(await service.GetMonthlyAsync(startDate, endDate, vehicleId));
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { message = e.Message });
        }
    }
}
