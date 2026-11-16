using FleetWise.DAL.Entities.Vehicles;
using FleetWise.DAL.Enums.Fuel;

namespace FleetWise.DAL.Entities.Fuel;

public class FuelTransfer
{
    public int Id { get; set; }
    public int SourceVehicleId { get; set; }
    public Vehicle SourceVehicle { get; set; } = null!;
    public DateTime OccurredAt { get; set; }
    public string? Notes { get; set; }
    public FuelApprovalStatus Status { get; set; } = FuelApprovalStatus.Approved;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? ReversesTransferId { get; set; }
    public FuelTransfer? ReversesTransfer { get; set; }
    public ICollection<FuelTransferDetail> Details { get; set; } = new List<FuelTransferDetail>();
}
