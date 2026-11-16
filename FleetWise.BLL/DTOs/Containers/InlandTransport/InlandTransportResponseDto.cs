using FleetWise.DAL.Enums;

namespace FleetWise.BLL.DTOs.Containers.InlandTransport
{
    public class InlandTransportResponseDto
    {
        public int Id { get; set; }

        public string TripCode { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public InlandTripStatus TripStatus { get; set; }

        public InternationalCargoType CargoType { get; set; }

        public decimal Weight { get; set; }

        public string LoadingLocation { get; set; } = string.Empty;

        public string UnloadingLocation { get; set; } = string.Empty;

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}