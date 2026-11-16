using FleetWise.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.DTOs.FuelRecords
{
    public class FuelRecordDto
    {
        public int FuelRecordId { get; set; }
        public int VehicleId { get; set; }
        public DateTime Date { get; set; }
        public decimal Quantity { get; set; }
        public decimal PricePerLiter { get; set; }
        public decimal TotalCost { get; set; }
        public int Odometer { get; set; }
        public int PlateNumber { get; set; }
        public FuelSourceType FuelSourceType { get; set; }
        public string? StationName { get; set; }
        public bool IsDeleted { get; set; }

        public List<FuelBatchConsumptionResultDto> Batches { get; set; }
            = new();
    }
}
