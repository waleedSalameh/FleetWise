namespace FleetWise.BLL.DTOs.Maintenance.SparePartMovements
{
    public class SparePartMovementDto
    {
        public int SparePartMovementId { get; set; }

        public int SparePartId { get; set; }

        public string PartNumber { get; set; } = string.Empty;

        public string PartName { get; set; } = string.Empty;

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