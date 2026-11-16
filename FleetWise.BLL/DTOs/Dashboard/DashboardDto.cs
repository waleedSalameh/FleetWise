using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.DTOs.Dashboard
{
    public class DashboardDto
    {
        public int TotalVehicles { get; set; }
        public int TotalDrivers { get; set; }
        public int TotalTrips { get; set; }
        public int TotalAssignments { get; set; }

        public int AvailableVehicles { get; set; }
        public int InUseVehicles { get; set; }
        public int VehiclesUnderMaintenance { get; set; }
        public int InactiveVehicles { get; set; }

        public int ActiveDrivers { get; set; }
        public int InactiveDrivers { get; set; }
        public int SuspendedDrivers { get; set; }
        public int PlannedTrips { get; set; }
        public int InProgressTrips { get; set; }
        public int CompletedTrips { get; set; }
        public int CancelledTrips { get; set; }

        public decimal TotalFuelQuantity { get; set; }
        public decimal TotalFuelCost { get; set; }

        public int TotalMaintenanceRecords { get; set; }
        public decimal TotalMaintenanceCost { get; set; }
    }
}
