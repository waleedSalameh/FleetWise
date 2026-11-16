using FleetWise.DAL.Enums;
using FleetWise.DAL.Enums.Fuel;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.DTOs.FuelRecords
{
    public class FuelInventorySummaryDto
    {
        public decimal TotalQuantity { get; set; }
        public decimal TotalValueJod { get; set; }
        public decimal PurchasedQuantity { get; set; }
        public decimal ReturnedQuantity { get; set; }
        public int AvailableBatchCount { get; set; }
    }

    public class FuelPurchaseDto
    {
        public int FuelPurchaseId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal RemainingQuantity { get; set; }
        public decimal PricePerLiter { get; set; }
        public string? SupplierName { get; set; }
        public string? InvoiceNumber { get; set; }
        public FuelPurchaseStatus Status { get;  set; }
        public FuelInventoryBatchOrigin BatchOrigin { get; set; }
        public int? SourceVehicleId { get; set; }
    }
}
