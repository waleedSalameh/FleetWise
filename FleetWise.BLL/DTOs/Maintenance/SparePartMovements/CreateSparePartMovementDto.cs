using System.ComponentModel.DataAnnotations;
using FleetWise.DAL.Enums;

namespace FleetWise.BLL.DTOs.Maintenance.SparePartMovements
{
    public class CreateSparePartMovementDto
    {
        [Required]
        public int SparePartId { get; set; }

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