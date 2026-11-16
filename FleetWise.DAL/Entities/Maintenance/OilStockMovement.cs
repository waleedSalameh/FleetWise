using FleetWise.DAL.Entities.Maintenance.FleetWise.DAL.Entities.Maintenance;
using FleetWise.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Entities.Maintenance
{
    public class OilStockMovement
    {
        public int OilStockMovementId { get; set; }

        public int OilId { get; set; }

        public InventoryMovementType MovementType { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitCost { get; set; }

        public DateTime Date { get; set; }

        public int? MaintenanceRecordId { get; set; }

        public string? ReferenceNumber { get; set; }

        public string? Notes { get; set; }

        public Oil Oil { get; set; }
    }
}
