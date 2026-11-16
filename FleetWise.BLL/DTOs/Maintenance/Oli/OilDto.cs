namespace FleetWise.BLL.DTOs.Maintenance.Oils
{
    public class OilDto
    {
        public int OilId { get; set; }

        public string OilName { get; set; } = string.Empty;

        public string? Brand { get; set; }

        public string? Viscosity { get; set; }

        public string Unit { get; set; } = "Liter";

        public decimal PurchasePrice { get; set; }

        public decimal MinimumQuantity { get; set; }
    }
}