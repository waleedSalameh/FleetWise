using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.DTOs.Containers.DeliveryLocations
{
    public class DeliveryLocationResponseDto
    {
        public int DeliveryLocationId { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
