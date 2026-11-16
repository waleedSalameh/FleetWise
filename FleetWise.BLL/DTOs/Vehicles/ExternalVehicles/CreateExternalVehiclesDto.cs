using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FleetWise.BLL.DTOs.Vehicles.ExternalVehicles
{
    public class CreateExternalVehiclesDto
    {
        [Required]
        [MaxLength(100)]
        public string DriverName { get; set; } = string.Empty;

        [Required]
       
        public int PlateNumber { get; set; }

        [Required]
        [MaxLength(30)]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
