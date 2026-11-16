using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Entities.Maintenance
{
    namespace FleetWise.DAL.Entities.Maintenance
    {
        public class Oil
        {
            public int OilId { get; set; }

            public string OilName { get; set; }

            public string? Brand { get; set; }

            public string? Viscosity { get; set; }

            public string Unit { get; set; } = "Liter";

            public decimal PurchasePrice { get; set; }

            public decimal MinimumQuantity { get; set; }

            public bool IsDeleted { get; set; }

            public ICollection<OilStockMovement> StockMovements { get; set; }
                = new List<OilStockMovement>();
        }
    }
}
