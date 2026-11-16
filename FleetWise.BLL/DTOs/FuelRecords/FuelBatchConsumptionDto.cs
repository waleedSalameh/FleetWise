using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FleetWise.BLL.DTOs.FuelRecords
{
    public class FuelBatchConsumptionDto
    {
        [Range(1, int.MaxValue)]
        public int FuelPurchaseId { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }
    }
}
