using FleetWise.DAL.Entities.Vehicles;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Entities
{
    public class Driver
    {
        public int DriverId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string LicenseNumber { get; set; }
        public DateOnly LicenseExpiryDate { get; set; }
        public DriverStatus Status { get; set; } = DriverStatus.Active;
        public bool IsDeleted { get; set; }
        public ICollection<VehicleAssignment> Assignments { get; set; }
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
    public enum DriverStatus
    {
        Active = 0,
        Inactive = 1,
        Suspended = 2
    }
}
