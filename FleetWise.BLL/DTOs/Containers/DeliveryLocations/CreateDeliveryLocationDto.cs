using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FleetWise.BLL.DTOs.Containers.DeliveryLocations
{
    public class CreateDeliveryLocationDto
    {
        [Required(ErrorMessage = "Please enter the delivery location name.")]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;
    }
}
