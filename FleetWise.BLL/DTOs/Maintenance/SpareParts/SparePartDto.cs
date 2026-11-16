namespace FleetWise.BLL.DTOs.Maintenance.SpareParts
{
    public class SparePartDto
    {
        public int SparePartId { get; set; }

        public string PartNumber { get; set; } = string.Empty;

        public string PartName { get; set; } = string.Empty;

        public string? Brand { get; set; }

        public string? Description { get; set; }

        public string Unit { get; set; } = "Piece";

        public decimal PurchasePrice { get; set; }

        public decimal MinimumQuantity { get; set; }
    }
}