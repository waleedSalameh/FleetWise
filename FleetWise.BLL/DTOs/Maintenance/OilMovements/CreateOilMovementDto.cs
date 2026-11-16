using FleetWise.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace FleetWise.BLL.DTOs.Maintenance.OilMovements
{
    public class CreateOilMovementDto
    {
        [Required]
        public int OilId { get; set; }

        [Required]
        public InventoryMovementType MovementType { get; set; }

        [Range(0.001, double.MaxValue)]
        public decimal Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal UnitCost { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public int? MaintenanceRecordId { get; set; }

        [MaxLength(100)]
        public string? ReferenceNumber { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}