using System.ComponentModel.DataAnnotations;
using FleetWise.DAL.Enums.Fuel;

namespace FleetWise.BLL.DTOs.FuelRecords;

public class CreateFuelTransferDto
{
    [Range(1, int.MaxValue)]
    public int SourceVehicleId { get; set; }
    public DateTime OccurredAt { get; set; }

    [Required, MinLength(1)]
    public List<CreateFuelTransferDetailDto> Destinations { get; set; } = new();

    [MaxLength(1000)]
    public string? Notes { get; set; }
}

public class CreateFuelTransferDetailDto
{
    public FuelTransferDestinationType DestinationType { get; set; }
    public int? DestinationVehicleId { get; set; }

    [Range(typeof(decimal), "0.01", "9999999999999999")]
    public decimal Quantity { get; set; }
}

public class FuelTransferDto
{
    public int Id { get; set; }
    public int SourceVehicleId { get; set; }
    public int SourcePlateNumber { get; set; }
    public DateTime OccurredAt { get; set; }
    public string? Notes { get; set; }
    public List<FuelTransferDetailResultDto> Destinations { get; set; } = new();
}

public class FuelTransferDetailResultDto
{
    public int Id { get; set; }
    public FuelTransferDestinationType DestinationType { get; set; }
    public int? DestinationVehicleId { get; set; }
    public int? DestinationPlateNumber { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCostJod { get; set; }
    public decimal TotalCostJod { get; set; }
    public int? ReturnedFuelPurchaseId { get; set; }
}
