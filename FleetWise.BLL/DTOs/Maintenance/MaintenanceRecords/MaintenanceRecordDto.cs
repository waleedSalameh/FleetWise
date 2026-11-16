namespace FleetWise.BLL.DTOs.Maintenance.MaintenanceRecords
{
    public class MaintenanceRecordDto
    {
        public int MaintenanceRecordId { get; set; }

        public int VehicleId { get; set; }

        public string PlateNumber { get; set; } = string.Empty;

        public string MaintenanceType { get; set; } = string.Empty;

        public MaintenanceLocation MaintenanceLocation { get; set; }

        public DateTime MaintenanceDate { get; set; }

        public int Odometer { get; set; }

        public string? Description { get; set; }

        public string? WorkshopName { get; set; }

        public string? TechnicianName { get; set; }

        public decimal PartsCost { get; set; }

        public decimal LaborCost { get; set; }

        public decimal TotalCost { get; set; }

        public MaintenanceStatus Status { get; set; }

        public List<FleetWise.BLL.DTOs.Maintenance.MaintenanceConsumption.OilConsumptionDto> Oils { get; set; } = new();

        public List<FleetWise.BLL.DTOs.Maintenance.MaintenanceConsumption.SparePartConsumptionDto> SpareParts { get; set; } = new();
    }
}
