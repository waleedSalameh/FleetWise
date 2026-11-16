using System.ComponentModel.DataAnnotations;

namespace FleetWise.BLL.DTOs.Maintenance.SpareParts
{
    public class CreateSparePartDto
    {
        [Required]
        [MaxLength(50)]
        public string PartNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string PartName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Brand { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(30)]
        public string Unit { get; set; } = "Piece";

        [Range(0, double.MaxValue)]
        public decimal PurchasePrice { get; set; }

        [Range(0, double.MaxValue)]
        public decimal MinimumQuantity { get; set; }
    }
}