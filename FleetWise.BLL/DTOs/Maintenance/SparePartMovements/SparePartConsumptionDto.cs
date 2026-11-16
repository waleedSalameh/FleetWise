using System.ComponentModel.DataAnnotations;

namespace FleetWise.BLL.DTOs.Maintenance.MaintenanceConsumption
{
    public class SparePartConsumptionDto
    {
        [Required]
        public int SparePartId { get; set; }

        [Range(0.001, double.MaxValue)]
        public decimal Quantity { get; set; }
    }
}