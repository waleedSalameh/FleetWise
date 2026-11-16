using FleetWise.DAL.Entities.Vehicles;
using FleetWise.DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FleetWise.DAL.Entities.Fuel
{
    public class FuelRecord
    {
        public int FuelRecordId { get; set; }

        public int VehicleId { get; set; }

        public FuelSourceType FuelSourceType { get; set; }

        public DateTime Date { get; set; }

        public decimal Quantity { get; set; }

        public decimal PricePerLiter { get; set; }

        public decimal TotalCost { get; set; }

        public int Odometer { get; set; }

        public string? StationName { get; set; }

        public Vehicle Vehicle { get; set; }

        public ICollection<FuelRecordConsumption> Consumptions { get; set; }
            = new List<FuelRecordConsumption>();

        public bool IsDeleted { get; set; }
    }
}
