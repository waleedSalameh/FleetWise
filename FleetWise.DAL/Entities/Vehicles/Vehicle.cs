using FleetWise.DAL.Entities.Fuel;
using FleetWise.DAL.Entities.Maintenance;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Entities.Vehicles
{
    public class Vehicle
    {
        public int VehicleId { get; set; }
        public  int PlateNumber { get; set; }
        public string Make {  get; set; }
        public int Year { get; set; }
        public string Model { get; set; }
        public int CurrentOdometer { get; set; }
        public decimal FuelTankCapacity { get; set; }

        public VehicleStatus Status { get; set; } = VehicleStatus.Available;
        public bool IsDeleted { get; set; }
        public ICollection<VehicleAssignment> Assignments { get; set; }
        public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; }
        public ICollection<FuelRecord> FuelRecords { get; set; }
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();



    }
    public enum VehicleStatus
    {
        Available = 0,
        InUse = 1,
        UnderMaintenance = 2,
        Inactive = 3
    }
}
