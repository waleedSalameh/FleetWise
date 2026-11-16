using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.DTOs.Vehicles
{
    public class VehicleDto
    {

        public int VehicleId { get; set; }
        public int PlateNumber { get; set; }
        public string Make { get; set; } 
        public int Year { get; set; }
        public string Model { get; set; }
        public int CurrentOdometer { get; set; }
        public decimal FuelTankCapacity { get; set; }
        public int Status { get; set; }
        public bool IsDeleted { get; set; }
    }
}
