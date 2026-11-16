namespace FleetWise.BLL.DTOs.Maintenance.OilMovements
{
    public class OilMovementDto
    {
        public int OilStockMovementId { get; set; }

        public int OilId { get; set; }

        public string OilName { get; set; } = string.Empty;

        public string MovementType { get; set; } = string.Empty;

        public decimal Quantity { get; set; }

        public decimal UnitCost { get; set; }

        public decimal TotalCost => Quantity * UnitCost;

        public DateTime Date { get; set; }

        public int? MaintenanceRecordId { get; set; }

        public string? ReferenceNumber { get; set; }

        public string? Notes { get; set; }
    }
}