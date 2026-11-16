using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FleetWise.BLL.DTOs.FuelRecords
{
    public class UpdateFuelRecordDto
    {
        [Range(1, int.MaxValue)]
        public int VehicleId { get; set; }

        public DateTime Date { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Quantity { get; set; }

        [Range(0.001, double.MaxValue)]
        public decimal PricePerLiter { get; set; }

        [Range(0, int.MaxValue)]
        public int Odometer { get; set; }

        [Required]
        [MaxLength(150)]
        public string StationName { get; set; } = string.Empty;
    }
}
