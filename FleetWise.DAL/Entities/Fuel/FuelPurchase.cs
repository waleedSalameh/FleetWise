using FleetWise.DAL.Enums;
using FleetWise.DAL.Enums.Fuel;
using FleetWise.DAL.Entities.Vehicles;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.DAL.Entities.Fuel
{
    public class FuelPurchase
    {
        public int FuelPurchaseId { get; set; }

        public DateTime PurchaseDate { get; set; }

        public decimal Quantity { get; set; }

        public decimal RemainingQuantity { get; set; }

        public decimal PricePerLiter { get; set; }

        public string? SupplierName { get; set; }

        public string? InvoiceNumber { get; set; }
        public FuelPurchaseStatus Status { get; set; } = FuelPurchaseStatus.InUse;

        public FuelInventoryBatchOrigin BatchOrigin { get; set; } = FuelInventoryBatchOrigin.Purchase;

        public int? SourceVehicleId { get; set; }

        public Vehicle? SourceVehicle { get; set; }

        public bool IsDeleted { get; set; }
        public ICollection<FuelRecordConsumption> Consumptions { get; set; }= new List<FuelRecordConsumption>();
    }
}
