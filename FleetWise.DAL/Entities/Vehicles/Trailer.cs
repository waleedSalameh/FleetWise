using FleetWise.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Entities.Vehicles
{
    public class Trailer
    {
        public int Id { get; set; }

        public int PlateNumber { get; set; }

        public int AxleCount { get; set; }

        public decimal MaxLoad { get; set; }

        public TrailerType TrailerType { get; set; }

        public DateOnly LicenseExpiryDate { get; set; }

        public bool IsDeleted { get; set; }
        public ICollection<VehicleAssignment> Assignments { get; set; }

    }
}
