using FleetWise.DAL.Entities.Vehicles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FleetWise.DAL.Entities.Maintenance
{
    public class MaintenanceRecord
    {
        
            public int MaintenanceRecordId { get; set; }

            public int VehicleId { get; set; }
            public Vehicle Vehicle { get; set; } = null!;

            public string MaintenanceType { get; set; } = string.Empty;

            public DateTime MaintenanceDate { get; set; }

            public int Odometer { get; set; }

            public string? Description { get; set; }

            public string? WorkshopName { get; set; }

            public string? TechnicianName { get; set; }

            public decimal PartsCost { get; set; }

            public decimal LaborCost { get; set; }

            public decimal TotalCost { get; set; }

            public MaintenanceStatus Status { get; set; }
        public MaintenanceLocation MaintenanceLocation { get; set; }

        public bool IsDeleted { get; set; }
        public ICollection<OilStockMovement> OilMovements { get; set; }
    = new List<OilStockMovement>();
        public ICollection<SparePartMovement> SparePartMovements { get; set; }
    = new List<SparePartMovement>();
    }

    }

    public enum MaintenanceStatus
    {
        Scheduled = 0,
        InProgress = 1,
        Completed = 2,
        Cancelled = 3
    }
public enum MaintenanceLocation
{
    Internal = 0,
    External = 1
}
