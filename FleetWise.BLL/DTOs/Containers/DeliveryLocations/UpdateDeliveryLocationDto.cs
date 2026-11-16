using System.ComponentModel.DataAnnotations;

namespace FleetWise.BLL.DTOs.Containers.DeliveryLocations;

public class UpdateDeliveryLocationDto
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;
}
