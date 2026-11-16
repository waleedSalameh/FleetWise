using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Entities.Maintenance
{
    public class SparePart
    {
        public int SparePartId { get; set; }

        public string PartNumber { get; set; }

        public string PartName { get; set; }

        public string? Brand { get; set; }

        public string? Description { get; set; }

        public string Unit { get; set; } = "Piece";

        public decimal PurchasePrice { get; set; }

        public decimal MinimumQuantity { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<SparePartMovement> StockMovements { get; set; }
            = new List<SparePartMovement>();
    }
}
