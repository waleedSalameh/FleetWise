using Microsoft.AspNetCore.Authorization;
using FleetWise.BLL.DTOs.FuelRecords;
using FleetWise.BLL.Interface.Fuel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.API.Controllers.Fuel
{
    [Route("api/[controller]")]
    [ApiController]
    public class FuelPurchasesController : ControllerBase
    {
        private readonly IFuelPurchaseService _fuelPurchaseService;

        public FuelPurchasesController(IFuelPurchaseService fuelPurchaseService)
        {
            _fuelPurchaseService = fuelPurchaseService;
        }
        [Authorize(Policy = "Permission:FuelPurchases.Create")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateFuelPurchaseDto dto)
        {

            try
            {
                var id = await _fuelPurchaseService.CreateAsync(dto);

                return Ok(new
                {
                    message = "Fuel purchase created successfully",
                    id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while creating fuel purchase.",
                    error = ex.Message
                });
            }
        }
        [Authorize(Policy = "Permission:FuelPurchases.View")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var purchases = await _fuelPurchaseService.GetAllAsync();

                return Ok(purchases);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving fuel purchases.",
                    error = ex.Message
                });
            }
        }

        [Authorize(Policy = "Permission:FuelPurchases.View")]
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            return Ok(await _fuelPurchaseService.GetSummaryAsync());
        }
    }
}

