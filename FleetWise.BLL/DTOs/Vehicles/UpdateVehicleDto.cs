using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.DTOs.Vehicles
{
    public class UpdateVehicleDto
    {
        public int PlateNumber { get; set; }
        public string Make { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Model { get; set; } = string.Empty;
        public int CurrentOdometer { get; set; }
        public decimal FuelTankCapacity { get; set; }
        public int Status { get; set; }
    }
}
