using FleetWise.DAL.Enums;
using System.ComponentModel.DataAnnotations;

namespace FleetWise.BLL.DTOs.Containers.InlandTransport
{
    public class CreateInlandTransportDto
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public InlandTripStatus TripStatus { get; set; }

        [Required]
        public InternationalCargoType CargoType { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "999999999")]
        public decimal Weight { get; set; }

        [Required]
        [MaxLength(200)]
        public string LoadingLocation { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string UnloadingLocation { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}