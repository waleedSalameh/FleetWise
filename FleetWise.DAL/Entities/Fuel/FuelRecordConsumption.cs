using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Entities.Fuel
{
    public class FuelRecordConsumption
    {
        public int Id { get; set; }

        public int FuelRecordId { get; set; }

        public int FuelPurchaseId { get; set; }

        public decimal Quantity { get; set; }

        public decimal PricePerLiter { get; set; }

        public decimal TotalCost { get; set; }

        public FuelRecord FuelRecord { get; set; }

        public FuelPurchase FuelPurchase { get; set; }
    }
}
