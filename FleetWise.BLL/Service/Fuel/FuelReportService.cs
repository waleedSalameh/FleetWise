using FleetWise.BLL.DTOs.FuelRecords;
using FleetWise.BLL.Interface.Fuel;
using FleetWise.DAL.Data;
using FleetWise.DAL.Enums;
using FleetWise.DAL.Enums.Fuel;
using Microsoft.EntityFrameworkCore;

namespace FleetWise.BLL.Service.Fuel;

public class FuelReportService(FleetWiseDbContext db) : IFuelReportService
{
    public async Task<List<FuelTransactionDto>> GetTransactionsAsync(
        DateTime start,
        DateTime end,
        int? vehicleId
    )
    {
        if (end < start)
            throw new ArgumentException("End date must be on or after start date.");
        var result = new List<FuelTransactionDto>();
        var fueling = await db
            .FuelRecords.AsNoTracking()
            .Include(x => x.Vehicle)
            .Where(x =>
                !x.IsDeleted
                && x.Date >= start
                && x.Date <= end
                && (!vehicleId.HasValue || x.VehicleId == vehicleId)
            )
            .ToListAsync();
        result.AddRange(
            fueling.Select(x => new FuelTransactionDto
            {
                TransactionId = $"fuel-{x.FuelRecordId}",
                TransactionType =
                    x.FuelSourceType == FuelSourceType.Internal
                        ? "InternalFueling"
                        : "ExternalFueling",
                Date = x.Date,
                Source =
                    x.FuelSourceType == FuelSourceType.Internal
                        ? "CompanyInventory"
                        : x.StationName ?? "ExternalStation",
                Destination = $"Vehicle {x.Vehicle.PlateNumber}",
                Quantity = x.Quantity,
                UnitCostJod = x.PricePerLiter,
                TotalCostJod = x.TotalCost,
                RelatedVehicleId = x.VehicleId,
            })
        );
        var transfers = await db
            .FuelTransfers.AsNoTracking()
            .Include(x => x.SourceVehicle)
            .Include(x => x.Details)
                .ThenInclude(x => x.DestinationVehicle)
            .Where(x =>
                x.OccurredAt >= start
                && x.OccurredAt <= end
                && x.Status == FuelApprovalStatus.Approved
                && x.ReversesTransferId == null
                && (
                    !vehicleId.HasValue
                    || x.SourceVehicleId == vehicleId
                    || x.Details.Any(d => d.DestinationVehicleId == vehicleId)
                )
            )
            .ToListAsync();
        foreach (var transfer in transfers)
        foreach (
            var detail in transfer.Details.Where(x =>
                x.DestinationType == FuelTransferDestinationType.Vehicle
            )
        )
        {
            if (!vehicleId.HasValue || transfer.SourceVehicleId == vehicleId)
                result.Add(
                    new FuelTransactionDto
                    {
                        TransactionId = $"transfer-{transfer.Id}-{detail.Id}-out",
                        TransactionType = "TransferOut",
                        Date = transfer.OccurredAt,
                        Source = $"Vehicle {transfer.SourceVehicle.PlateNumber}",
                        Destination = $"Vehicle {detail.DestinationVehicle!.PlateNumber}",
                        Quantity = detail.Quantity,
                        UnitCostJod = detail.UnitCostJod,
                        TotalCostJod = detail.TotalCostJod,
                        // This ledger row belongs to the vehicle sending the fuel.
                        RelatedVehicleId = transfer.SourceVehicleId,
                    }
                );
            if (!vehicleId.HasValue || detail.DestinationVehicleId == vehicleId)
                result.Add(
                    new FuelTransactionDto
                    {
                        TransactionId = $"transfer-{transfer.Id}-{detail.Id}-in",
                        TransactionType = "TransferIn",
                        Date = transfer.OccurredAt,
                        Source = $"Vehicle {transfer.SourceVehicle.PlateNumber}",
                        Destination = $"Vehicle {detail.DestinationVehicle!.PlateNumber}",
                        Quantity = detail.Quantity,
                        UnitCostJod = detail.UnitCostJod,
                        TotalCostJod = detail.TotalCostJod,
                        // This ledger row belongs to the vehicle receiving the fuel.
                        RelatedVehicleId = detail.DestinationVehicleId,
                    }
                );
        }
        var batches = await db
            .FuelPurchases.AsNoTracking()
            .Where(x => !x.IsDeleted && x.PurchaseDate >= start && x.PurchaseDate <= end)
            .ToListAsync();
        result.AddRange(
            batches.Select(x => new FuelTransactionDto
            {
                TransactionId = $"batch-{x.FuelPurchaseId}",
                TransactionType =
                    x.BatchOrigin == FuelInventoryBatchOrigin.Purchase
                        ? "InventoryPurchase"
                        : "VehicleFuelReturn",
                Date = x.PurchaseDate,
                Source =
                    x.BatchOrigin == FuelInventoryBatchOrigin.Purchase
                        ? x.SupplierName ?? "Supplier"
                        : $"Vehicle {x.SourceVehicleId}",
                Destination = "CompanyInventory",
                Quantity = x.Quantity,
                UnitCostJod = x.PricePerLiter,
                TotalCostJod = Math.Round(
                    x.Quantity * x.PricePerLiter,
                    3,
                    MidpointRounding.AwayFromZero
                ),
                RelatedVehicleId = x.SourceVehicleId,
                FuelPurchaseId = x.FuelPurchaseId,
            })
        );
        return result.OrderBy(x => x.Date).ToList();
    }

    public async Task<List<MonthlyFuelReportDto>> GetMonthlyAsync(
        DateOnly start,
        DateOnly end,
        int? vehicleId
    )
    {
        if (end < start)
            throw new ArgumentException("End date must be on or after start date.");
        var vehicles = await db
            .Vehicles.AsNoTracking()
            .Where(v => !v.IsDeleted && (!vehicleId.HasValue || v.VehicleId == vehicleId))
            .OrderBy(v => v.PlateNumber)
            .ToListAsync();
        var closings = await db
            .MonthlyFuelClosings.AsNoTracking()
            .Where(x =>
                x.Status == FuelApprovalStatus.Approved
                && x.PeriodStart >= start
                && x.PeriodEnd <= end
                && (!vehicleId.HasValue || x.VehicleId == vehicleId)
            )
            .ToListAsync();
        return vehicles
            .Select(v =>
            {
                var periods = closings
                    .Where(x => x.VehicleId == v.VehicleId)
                    .OrderBy(x => x.PeriodStart)
                    .ToList();
                var complete =
                    periods.Count > 0
                    && periods[0].PeriodStart == start
                    && periods[^1].PeriodEnd == end
                    && periods
                        .Zip(periods.Skip(1), (a, b) => a.PeriodEnd.AddDays(1) == b.PeriodStart)
                        .All(x => x);
                return new MonthlyFuelReportDto
                {
                    VehicleId = v.VehicleId,
                    PlateNumber = v.PlateNumber,
                    PeriodStart = start,
                    PeriodEnd = end,
                    IsComplete = complete,
                    IncompleteReason = complete
                        ? null
                        : "Approved opening and closing reconciliations do not cover the full requested period.",
                    OpeningFuelQuantity = periods.FirstOrDefault()?.OpeningQuantity ?? 0,
                    OpeningFuelValueJod = periods.FirstOrDefault()?.OpeningValueJod ?? 0,
                    ClosingFuelQuantity = periods.LastOrDefault()?.ClosingQuantity ?? 0,
                    ClosingFuelValueJod = periods.LastOrDefault()?.ClosingValueJod ?? 0,
                    TotalFuelingQuantity = periods.Sum(x => x.TotalFuelingQuantity),
                    TotalFuelingCostJod = periods.Sum(x => x.TotalFuelingValueJod),
                    IncomingTransferQuantity = periods.Sum(x => x.IncomingTransferQuantity),
                    IncomingTransferValueJod = periods.Sum(x => x.IncomingTransferValueJod),
                    OutgoingTransferQuantity = periods.Sum(x => x.OutgoingTransferQuantity),
                    OutgoingTransferValueJod = periods.Sum(x => x.OutgoingTransferValueJod),
                    ActualConsumptionLiters = periods.Sum(x => x.ActualConsumptionQuantity),
                    ActualConsumptionCostJod = periods.Sum(x => x.ActualConsumptionValueJod),
                    NetFuelValueAllocatedJod = periods.Sum(x =>
                        x.TotalFuelingValueJod
                        + x.IncomingTransferValueJod
                        - x.OutgoingTransferValueJod
                    ),
                    InternalFuelingQuantity = db.FuelRecords.Where(x =>
                            x.VehicleId == v.VehicleId
                            && !x.IsDeleted
                            && x.FuelSourceType == FuelSourceType.Internal
                            && x.Date >= start.ToDateTime(TimeOnly.MinValue)
                            && x.Date <= end.ToDateTime(TimeOnly.MaxValue)
                        )
                        .Sum(x => (decimal?)x.Quantity)
                        ?? 0,
                    InternalFuelingCostJod = db.FuelRecords.Where(x =>
                            x.VehicleId == v.VehicleId
                            && !x.IsDeleted
                            && x.FuelSourceType == FuelSourceType.Internal
                            && x.Date >= start.ToDateTime(TimeOnly.MinValue)
                            && x.Date <= end.ToDateTime(TimeOnly.MaxValue)
                        )
                        .Sum(x => (decimal?)x.TotalCost)
                        ?? 0,
                    ExternalFuelingQuantity = db.FuelRecords.Where(x =>
                            x.VehicleId == v.VehicleId
                            && !x.IsDeleted
                            && x.FuelSourceType == FuelSourceType.External
                            && x.Date >= start.ToDateTime(TimeOnly.MinValue)
                            && x.Date <= end.ToDateTime(TimeOnly.MaxValue)
                        )
                        .Sum(x => (decimal?)x.Quantity)
                        ?? 0,
                    ExternalFuelingCostJod = db.FuelRecords.Where(x =>
                            x.VehicleId == v.VehicleId
                            && !x.IsDeleted
                            && x.FuelSourceType == FuelSourceType.External
                            && x.Date >= start.ToDateTime(TimeOnly.MinValue)
                            && x.Date <= end.ToDateTime(TimeOnly.MaxValue)
                        )
                        .Sum(x => (decimal?)x.TotalCost)
                        ?? 0,
                    InternalFuelingCount = periods.Sum(x => x.InternalFuelingCount),
                    ExternalFuelingCount = periods.Sum(x => x.ExternalFuelingCount),
                    IncomingTransferCount = periods.Sum(x => x.IncomingTransferCount),
                    OutgoingTransferCount = periods.Sum(x => x.OutgoingTransferCount),
                };
            })
            .ToList();
    }
}
