using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FleetWise.BLL.DTOs.FuelRecords
{
    public class CreateInternalFuelRecordDto
    {
        [Range(1, int.MaxValue)]
        public int VehicleId { get; set; }

        public DateTime Date { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }

        [Range(0, int.MaxValue)]
        public int Odometer { get; set; }

        [Required]
        [MinLength(1)]
        public List<FuelBatchConsumptionDto> Batches { get; set; }
            = new();
    }
}
