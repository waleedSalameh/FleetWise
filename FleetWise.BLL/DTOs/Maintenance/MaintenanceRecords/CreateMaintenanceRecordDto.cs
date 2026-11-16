using FleetWise.BLL.DTOs.Maintenance.MaintenanceConsumption;
using FleetWise.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace FleetWise.BLL.DTOs.Maintenance.MaintenanceRecords
{
    public class CreateMaintenanceRecordDto
    {
        [Required]
        public int VehicleId { get; set; }

        [Required]
        [MaxLength(100)]
        public string MaintenanceType { get; set; } = string.Empty;

        [Required]
        public MaintenanceLocation MaintenanceLocation { get; set; }

        [Required]
        public DateTime MaintenanceDate { get; set; }

        [Range(0, int.MaxValue)]
        public int Odometer { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(150)]
        public string? WorkshopName { get; set; }

        [MaxLength(150)]
        public string? TechnicianName { get; set; }

        [Range(0, double.MaxValue)]
        public decimal LaborCost { get; set; }

        public MaintenanceStatus Status { get; set; }
        public List<OilConsumptionDto> Oils { get; set; } = new();

        public List<SparePartConsumptionDto> SpareParts { get; set; } = new();
    }
}