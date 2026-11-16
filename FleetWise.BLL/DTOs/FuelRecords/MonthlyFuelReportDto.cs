namespace FleetWise.BLL.DTOs.FuelRecords;

public class FuelTransactionDto
{
    public string TransactionId { get; set; } = string.Empty;
    public string TransactionType { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Source { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitCostJod { get; set; }
    public decimal TotalCostJod { get; set; }
    public int? RelatedVehicleId { get; set; }
    public int? FuelPurchaseId { get; set; }
}

public class MonthlyFuelReportDto
{
    public int VehicleId { get; set; }
    public int PlateNumber { get; set; }
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }
    public bool IsComplete { get; set; }
    public string? IncompleteReason { get; set; }
    public decimal OpeningFuelQuantity { get; set; }
    public decimal OpeningFuelValueJod { get; set; }
    public decimal ClosingFuelQuantity { get; set; }
    public decimal ClosingFuelValueJod { get; set; }
    public decimal TotalFuelingQuantity { get; set; }
    public decimal TotalFuelingCostJod { get; set; }
    public decimal InternalFuelingQuantity { get; set; }
    public decimal InternalFuelingCostJod { get; set; }
    public decimal ExternalFuelingQuantity { get; set; }
    public decimal ExternalFuelingCostJod { get; set; }
    public decimal IncomingTransferQuantity { get; set; }
    public decimal IncomingTransferValueJod { get; set; }
    public decimal OutgoingTransferQuantity { get; set; }
    public decimal OutgoingTransferValueJod { get; set; }
    public decimal ActualConsumptionLiters { get; set; }
    public decimal ActualConsumptionCostJod { get; set; }
    public decimal NetFuelValueAllocatedJod { get; set; }
    public int InternalFuelingCount { get; set; }
    public int ExternalFuelingCount { get; set; }
    public int IncomingTransferCount { get; set; }
    public int OutgoingTransferCount { get; set; }
    public int TotalFuelingCount => InternalFuelingCount + ExternalFuelingCount;
    public int TotalTransferCount => IncomingTransferCount + OutgoingTransferCount;
}
