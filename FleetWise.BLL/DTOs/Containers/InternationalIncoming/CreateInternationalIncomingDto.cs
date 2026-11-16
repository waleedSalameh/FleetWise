using System.ComponentModel.DataAnnotations;
using FleetWise.DAL.Enums;

namespace FleetWise.BLL.DTOs.Containers.InternationalIncoming
{
    public class CreateInternationalIncomingDto
    {
        [Required(ErrorMessage = "Please enter the cargo number.")]
        [StringLength(100)]
        public string CargoNumber { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Please select an active customer.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Please enter the country.")]
        [StringLength(100)]
        public string Country { get; set; } = string.Empty;

        [EnumDataType(typeof(InternationalCargoType), ErrorMessage = "Please select a valid cargo type.")]
        public InternationalCargoType CargoType { get; set; }

        [Required(ErrorMessage = "Please enter the loading location.")]
        [StringLength(200)]
        public string LoadingLocation { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the factory name.")]
        [StringLength(200)]
        public string FactoryName { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "9999999999999999.99", ErrorMessage = "Weight must be greater than zero.")]
        public decimal Weight { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "Please enter the customs declaration number.")]
        [StringLength(100)]
        public string CustomsDeclarationNumber { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Trip duration must be greater than zero.")]
        public int TripDuration { get; set; }

        [Required(ErrorMessage = "Please enter the unloading location.")]
        [StringLength(200)]
        public string UnloadingLocation { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "9999999999999999.99", ErrorMessage = "Distance must be greater than zero.")]
        public decimal Distance { get; set; }

        [EnumDataType(typeof(InternationalOrderStatus), ErrorMessage = "Please select a valid status.")]
        public InternationalOrderStatus Status { get; set; } = InternationalOrderStatus.New;

        public bool IsPaid { get; set; }
    }
}
