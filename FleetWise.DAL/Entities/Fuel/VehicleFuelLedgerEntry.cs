using FleetWise.DAL.Entities.Vehicles;
using FleetWise.DAL.Enums.Fuel;

namespace FleetWise.DAL.Entities.Fuel;

public class VehicleFuelLedgerEntry
{
    public long Id { get; set; }
    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;
    public DateTime OccurredAt { get; set; }
    public FuelLedgerEntryType EntryType { get; set; }
    public decimal QuantityDelta { get; set; }
    public decimal ValueDeltaJod { get; set; }
    public int? FuelRecordId { get; set; }
    public FuelRecord? FuelRecord { get; set; }
    public int? FuelTransferDetailId { get; set; }
    public FuelTransferDetail? FuelTransferDetail { get; set; }
    public long? MeasurementId { get; set; }
    public VehicleFuelMeasurement? Measurement { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
