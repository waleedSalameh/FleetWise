using FleetWise.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Entities.Maintenance
{
    public class SparePartMovement
    {
        public int SparePartMovementId { get; set; }

        public int SparePartId { get; set; }

        public InventoryMovementType MovementType { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnitCost { get; set; }

        public DateTime Date { get; set; }

        public int? MaintenanceRecordId { get; set; }

        public string? ReferenceNumber { get; set; }

        public string? Notes { get; set; }

        public SparePart SparePart { get; set; }
    }
}
