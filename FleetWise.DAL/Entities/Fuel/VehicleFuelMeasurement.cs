using FleetWise.DAL.Entities.Vehicles;
using FleetWise.DAL.Enums.Fuel;

namespace FleetWise.DAL.Entities.Fuel;

public class VehicleFuelMeasurement
{
    public long Id { get; set; }
    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;
    public DateTime MeasuredAt { get; set; }
    public decimal MeasuredQuantity { get; set; }
    public decimal? MeasuredValueJod { get; set; }
    public FuelMeasurementType MeasurementType { get; set; }
    public FuelApprovalStatus Status { get; set; } = FuelApprovalStatus.Draft;
    public string? Notes { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
