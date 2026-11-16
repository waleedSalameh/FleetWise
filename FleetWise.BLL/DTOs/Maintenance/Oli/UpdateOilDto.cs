using System.ComponentModel.DataAnnotations;

namespace FleetWise.BLL.DTOs.Maintenance.Oils
{
    public class UpdateOilDto
    {
        [Required]
        [MaxLength(150)]
        public string OilName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Brand { get; set; }

        [MaxLength(50)]
        public string? Viscosity { get; set; }

        [Required]
        [MaxLength(30)]
        public string Unit { get; set; } = "Liter";

        [Range(0, double.MaxValue)]
        public decimal PurchasePrice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal MinimumQuantity { get; set; }
    }
}