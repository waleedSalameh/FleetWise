using FleetWise.DAL.Entities.Vehicles;
using FleetWise.DAL.Enums.Fuel;

namespace FleetWise.DAL.Entities.Fuel;

public class FuelTransferDetail
{
    public int Id { get; set; }
    public int FuelTransferId { get; set; }
    public FuelTransfer FuelTransfer { get; set; } = null!;
    public FuelTransferDestinationType DestinationType { get; set; }
    public int? DestinationVehicleId { get; set; }
    public Vehicle? DestinationVehicle { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCostJod { get; set; }
    public decimal TotalCostJod { get; set; }
    public int? ReturnedFuelPurchaseId { get; set; }
    public FuelPurchase? ReturnedFuelPurchase { get; set; }
}
