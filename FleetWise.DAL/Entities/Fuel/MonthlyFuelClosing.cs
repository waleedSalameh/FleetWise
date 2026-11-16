using FleetWise.DAL.Entities.Vehicles;
using FleetWise.DAL.Enums.Fuel;

namespace FleetWise.DAL.Entities.Fuel;

public class MonthlyFuelClosing
{
    public long Id { get; set; }
    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }
    public long OpeningMeasurementId { get; set; }
    public long ClosingMeasurementId { get; set; }
    public decimal OpeningQuantity { get; set; }
    public decimal OpeningValueJod { get; set; }
    public decimal TotalFuelingQuantity { get; set; }
    public decimal TotalFuelingValueJod { get; set; }
    public decimal IncomingTransferQuantity { get; set; }
    public decimal IncomingTransferValueJod { get; set; }
    public decimal OutgoingTransferQuantity { get; set; }
    public decimal OutgoingTransferValueJod { get; set; }
    public decimal ClosingQuantity { get; set; }
    public decimal ClosingValueJod { get; set; }
    public decimal ActualConsumptionQuantity { get; set; }
    public decimal ActualConsumptionValueJod { get; set; }
    public int InternalFuelingCount { get; set; }
    public int ExternalFuelingCount { get; set; }
    public int IncomingTransferCount { get; set; }
    public int OutgoingTransferCount { get; set; }
    public FuelApprovalStatus Status { get; set; } = FuelApprovalStatus.Draft;
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
