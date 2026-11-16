using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.DTOs.FuelRecords
{
    public class FuelBatchConsumptionResultDto
    {
        public int FuelPurchaseId { get; set; }
        public decimal Quantity { get; set; }
        public decimal PricePerLiter { get; set; }
        public decimal TotalCost { get; set; }
    }
}
