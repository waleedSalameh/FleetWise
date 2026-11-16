using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.DTOs.Vehicles.ExternalVehicles
{
    public class ExternalVehiclesResponseDto
    {
        public int Id { get; set; }

        public string DriverName { get; set; } = string.Empty;

        public int PlateNumber { get; set; } 

        public string PhoneNumber { get; set; } = string.Empty;
    }
}
