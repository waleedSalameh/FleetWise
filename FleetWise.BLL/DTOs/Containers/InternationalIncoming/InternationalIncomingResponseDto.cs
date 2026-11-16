using FleetWise.DAL.Enums;

namespace FleetWise.BLL.DTOs.Containers.InternationalIncoming
{
    public class InternationalIncomingResponseDto
    {
        public int Id { get; set; }

        public string CargoNumber { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string? CustomerPhoneNumber { get; set; }

        public string Country { get; set; } = string.Empty;

        public InternationalCargoType CargoType { get; set; }

        public string LoadingLocation { get; set; } = string.Empty;

        public string FactoryName { get; set; } = string.Empty;

        public decimal Weight { get; set; }

        public string? Notes { get; set; }

        public string CustomsDeclarationNumber { get; set; } = string.Empty;

        public int TripDuration { get; set; }

        public string UnloadingLocation { get; set; } = string.Empty;

        public decimal Distance { get; set; }

        public InternationalOrderStatus Status { get; set; }

        public bool IsPaid { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
