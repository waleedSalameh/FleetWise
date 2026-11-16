using System.ComponentModel.DataAnnotations;

namespace FleetWise.BLL.DTOs.Containers.ShippingLines;

public class UpdateShippingLineDto
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string Code { get; set; } = string.Empty;
}
