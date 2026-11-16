using System.Data;
using FleetWise.BLL.DTOs.FuelRecords;
using FleetWise.BLL.Interface.Fuel;
using FleetWise.DAL.Data;
using FleetWise.DAL.Entities.Fuel;
using FleetWise.DAL.Enums;
using FleetWise.DAL.Enums.Fuel;
using FleetWise.BLL.Service.Concurrency;
using Microsoft.EntityFrameworkCore;

namespace FleetWise.BLL.Service.Fuel;

public class FuelTransferService(FleetWiseDbContext db) : IFuelTransferService
{
    public async Task<FuelTransferDto> CreateAsync(CreateFuelTransferDto dto)
    {
        if (dto.OccurredAt == default)
            throw new ArgumentException("Transfer date and time are required.");
        if (dto.Destinations.Count == 0 || dto.Destinations.Any(x => x.Quantity <= 0))
            throw new ArgumentException(
                "At least one destination with a positive quantity is required."
            );
        var vehicleDestinations = dto
            .Destinations.Where(x => x.DestinationType == FuelTransferDestinationType.Vehicle)
            .ToList();
        if (
            vehicleDestinations.Any(x =>
                x.DestinationVehicleId is null || x.DestinationVehicleId == dto.SourceVehicleId
            )
            || vehicleDestinations.Select(x => x.DestinationVehicleId).Distinct().Count()
                != vehicleDestinations.Count
            || dto.Destinations.Any(x => !Enum.IsDefined(x.DestinationType))
            || dto.Destinations.Count(x =>
                x.DestinationType == FuelTransferDestinationType.Inventory
            ) > 1
        )
            throw new ArgumentException("Transfer destinations are invalid or duplicated.");

        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var vehicleIds = vehicleDestinations
            .Select(x => x.DestinationVehicleId!.Value)
            .Append(dto.SourceVehicleId)
            .Distinct()
            .Order()
            .ToList();
        foreach (var vehicleId in vehicleIds.Order())
        {
            await DatabaseApplicationLock.AcquireAsync(
                db, $"FleetWise:Fuel:Vehicle:{vehicleId}");
        }

        var vehicles = await db
            .Vehicles.Where(v => vehicleIds.Contains(v.VehicleId) && !v.IsDeleted)
            .OrderBy(v => v.VehicleId)
            .ToDictionaryAsync(v => v.VehicleId);
        if (vehicles.Count != vehicleIds.Count)
            throw new ArgumentException("A source or destination vehicle was not found.");

        foreach (var id in vehicleIds)
        {
            if (
                await db.MonthlyFuelClosings.AnyAsync(x =>
                    x.VehicleId == id
                    && x.Status == FuelApprovalStatus.Approved
                    && x.PeriodStart <= DateOnly.FromDateTime(dto.OccurredAt)
                    && x.PeriodEnd >= DateOnly.FromDateTime(dto.OccurredAt)
                )
            )
                throw new InvalidOperationException(
                    "Fuel transfers are not allowed in an approved accounting period."
                );
            var latest = await db
                .VehicleFuelLedgerEntries.Where(x => x.VehicleId == id)
                .MaxAsync(x => (DateTime?)x.OccurredAt);
            if (latest.HasValue && dto.OccurredAt < latest.Value)
                throw new InvalidOperationException("Backdated fuel transfers are not allowed.");
        }

        var sourceBalance = await Balance(dto.SourceVehicleId);
        var transferQuantity = dto.Destinations.Sum(x => x.Quantity);
        if (sourceBalance.Quantity < transferQuantity)
            throw new InvalidOperationException("Insufficient fuel in the source vehicle.");
        var unitCost =
            sourceBalance.Quantity == 0
                ? 0
                : Math.Round(
                    sourceBalance.Value / sourceBalance.Quantity,
                    3,
                    MidpointRounding.AwayFromZero
                );
        foreach (var d in vehicleDestinations)
        {
            var destBalance = await Balance(d.DestinationVehicleId!.Value);
            if (
                destBalance.Quantity + d.Quantity
                > vehicles[d.DestinationVehicleId.Value].FuelTankCapacity
            )
                throw new InvalidOperationException(
                    $"Transfer exceeds the fuel tank capacity of vehicle {vehicles[d.DestinationVehicleId.Value].PlateNumber}."
                );
        }

        var header = new FuelTransfer
        {
            SourceVehicleId = dto.SourceVehicleId,
            OccurredAt = dto.OccurredAt,
            Notes = dto.Notes?.Trim(),
            Status = FuelApprovalStatus.Approved,
        };
        foreach (var d in dto.Destinations)
        {
            var value = Math.Round(d.Quantity * unitCost, 3, MidpointRounding.AwayFromZero);
            header.Details.Add(
                new FuelTransferDetail
                {
                    DestinationType = d.DestinationType,
                    DestinationVehicleId = d.DestinationVehicleId,
                    Quantity = d.Quantity,
                    UnitCostJod = unitCost,
                    TotalCostJod = value,
                }
            );
        }
        db.FuelTransfers.Add(header);
        await db.SaveChangesAsync();

        var sourceValue = -header.Details.Sum(x => x.TotalCostJod);
        db.VehicleFuelLedgerEntries.Add(
            new VehicleFuelLedgerEntry
            {
                VehicleId = dto.SourceVehicleId,
                OccurredAt = dto.OccurredAt,
                EntryType = FuelLedgerEntryType.TransferOut,
                QuantityDelta = -transferQuantity,
                ValueDeltaJod = sourceValue,
                FuelTransferDetailId = header.Details.First().Id,
            }
        );
        foreach (var detail in header.Details)
        {
            if (detail.DestinationType == FuelTransferDestinationType.Vehicle)
                db.VehicleFuelLedgerEntries.Add(
                    new VehicleFuelLedgerEntry
                    {
                        VehicleId = detail.DestinationVehicleId!.Value,
                        OccurredAt = dto.OccurredAt,
                        EntryType = FuelLedgerEntryType.TransferIn,
                        QuantityDelta = detail.Quantity,
                        ValueDeltaJod = detail.TotalCostJod,
                        FuelTransferDetailId = detail.Id,
                    }
                );
            else
            {
                var batch = new FuelPurchase
                {
                    PurchaseDate = dto.OccurredAt,
                    Quantity = detail.Quantity,
                    RemainingQuantity = detail.Quantity,
                    PricePerLiter = Math.Round(
                        detail.UnitCostJod,
                        3,
                        MidpointRounding.AwayFromZero
                    ),
                    Status = FuelPurchaseStatus.InUse,
                    BatchOrigin = FuelInventoryBatchOrigin.VehicleReturn,
                    SourceVehicleId = dto.SourceVehicleId,
                };
                db.FuelPurchases.Add(batch);
                await db.SaveChangesAsync();
                detail.ReturnedFuelPurchaseId = batch.FuelPurchaseId;
            }
        }
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return (await GetByIdAsync(header.Id))!;
    }

    public Task<List<FuelTransferDto>> GetAllAsync() =>
        Project(db.FuelTransfers.AsNoTracking()
            .Where(x => x.Status == FuelApprovalStatus.Approved && x.ReversesTransferId == null)
            .OrderByDescending(x => x.OccurredAt)).ToListAsync();

    public Task<FuelTransferDto?> GetByIdAsync(int id) =>
        Project(db.FuelTransfers.AsNoTracking().Where(x => x.Id == id)).FirstOrDefaultAsync();

    public async Task<FuelTransferDto> ReverseAsync(int id, string notes)
    {
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        await DatabaseApplicationLock.AcquireAsync(
            db, $"FleetWise:FuelTransfer:{id}");

        var original = await db
            .FuelTransfers.Include(x => x.Details)
                .ThenInclude(x => x.ReturnedFuelPurchase)
            .FirstOrDefaultAsync(x => x.Id == id && x.Status == FuelApprovalStatus.Approved
                && x.ReversesTransferId == null)
            ?? throw new ArgumentException("Fuel transfer was not found or is already reversed.");
        if (await db.FuelTransfers.AnyAsync(x => x.ReversesTransferId == id))
            throw new InvalidOperationException("This transfer has already been reversed.");

        var totalQty = original.Details.Sum(x => x.Quantity);
        var totalValue = original.Details.Sum(x => x.TotalCostJod);
        var affectedVehicleIds = original
            .Details.Where(x => x.DestinationType == FuelTransferDestinationType.Vehicle)
            .Select(x => x.DestinationVehicleId!.Value)
            .Append(original.SourceVehicleId)
            .Distinct()
            .ToList();

        foreach (var vehicleId in affectedVehicleIds.Order())
        {
            await DatabaseApplicationLock.AcquireAsync(
                db, $"FleetWise:Fuel:Vehicle:{vehicleId}");
        }

        foreach (var batchId in original.Details
            .Where(detail => detail.ReturnedFuelPurchaseId.HasValue)
            .Select(detail => detail.ReturnedFuelPurchaseId!.Value)
            .Distinct()
            .Order())
        {
            await DatabaseApplicationLock.AcquireAsync(
                db, $"FleetWise:Fuel:Batch:{batchId}");
        }

        foreach (var vehicleId in affectedVehicleIds)
        {
            if (
                await db.MonthlyFuelClosings.AnyAsync(x =>
                    x.VehicleId == vehicleId
                    && x.Status == FuelApprovalStatus.Approved
                    && x.PeriodEnd >= DateOnly.FromDateTime(original.OccurredAt)
                )
            )
                throw new InvalidOperationException(
                    "A transfer cannot be deleted after its vehicle fuel period has been approved."
                );

            if (await db.VehicleFuelMeasurements.AnyAsync(x =>
                x.VehicleId == vehicleId
                && x.Status == FuelApprovalStatus.Approved
                && x.MeasurementType != FuelMeasurementType.Opening
                && x.MeasuredAt >= original.OccurredAt))
                throw new InvalidOperationException(
                    "A later approved fuel measurement depends on this transfer."
                );

            if (await db.FuelTransfers.AnyAsync(x =>
                x.Id != original.Id
                && x.SourceVehicleId == vehicleId
                && x.Status == FuelApprovalStatus.Approved
                && x.ReversesTransferId == null
                && x.OccurredAt >= original.OccurredAt))
                throw new InvalidOperationException(
                    "Delete later fuel transfers from affected vehicles before deleting this transfer."
                );
        }

        var source = await db.Vehicles.FirstAsync(x => x.VehicleId == original.SourceVehicleId);
        var sourceBalance = await Balance(original.SourceVehicleId);
        if (sourceBalance.Quantity + totalQty > source.FuelTankCapacity)
            throw new InvalidOperationException(
                "Deleting the transfer would exceed the source vehicle tank capacity."
            );

        var firstDetailId = original.Details.Min(x => x.Id);
        var sourceEntry = await db.VehicleFuelLedgerEntries
            .SingleOrDefaultAsync(x => x.VehicleId == original.SourceVehicleId
                && x.FuelTransferDetailId == firstDetailId
                && x.EntryType == FuelLedgerEntryType.TransferOut);
        if (sourceEntry == null)
        {
            var legacyEntries = await db.VehicleFuelLedgerEntries
                .Where(x => x.VehicleId == original.SourceVehicleId
                    && x.OccurredAt == original.OccurredAt
                    && x.EntryType == FuelLedgerEntryType.TransferOut
                    && x.FuelTransferDetailId == null
                    && x.QuantityDelta == -totalQty
                    && x.ValueDeltaJod == -totalValue)
                .Take(2)
                .ToListAsync();
            if (legacyEntries.Count != 1)
                throw new InvalidOperationException(
                    "The source fuel ledger entry could not be identified uniquely."
                );
            sourceEntry = legacyEntries[0];
        }
        if (sourceEntry.OccurredAt != original.OccurredAt
            || sourceEntry.QuantityDelta != -totalQty
            || sourceEntry.ValueDeltaJod != -totalValue)
            throw new InvalidOperationException(
                "The source fuel ledger entry does not match this transfer."
            );

        foreach (
            var d in original.Details.Where(x =>
                x.DestinationType == FuelTransferDestinationType.Vehicle
            )
        )
        {
            var balance = await Balance(d.DestinationVehicleId!.Value);
            if (balance.Quantity < d.Quantity || balance.Value < d.TotalCostJod)
                throw new InvalidOperationException(
                    "A destination vehicle no longer has enough fuel and value to delete this transfer."
                );
        }

        foreach (var d in original.Details)
        {
            if (d.DestinationType == FuelTransferDestinationType.Inventory)
            {
                var batch = d.ReturnedFuelPurchase
                    ?? throw new InvalidOperationException("Returned inventory batch is missing.");
                if (batch.IsDeleted
                    || batch.BatchOrigin != FuelInventoryBatchOrigin.VehicleReturn
                    || batch.SourceVehicleId != original.SourceVehicleId
                    || batch.Quantity != d.Quantity
                    || batch.RemainingQuantity != d.Quantity)
                    throw new InvalidOperationException(
                        "Returned fuel has been used or changed and this transfer cannot be deleted."
                    );
                batch.RemainingQuantity = 0;
                batch.Status = FuelPurchaseStatus.Completed;
                batch.IsDeleted = true;
            }
            else
            {
                var destinationEntry = await db.VehicleFuelLedgerEntries
                    .SingleOrDefaultAsync(x => x.FuelTransferDetailId == d.Id
                        && x.VehicleId == d.DestinationVehicleId
                        && x.EntryType == FuelLedgerEntryType.TransferIn);
                if (destinationEntry == null
                    || destinationEntry.QuantityDelta != d.Quantity
                    || destinationEntry.ValueDeltaJod != d.TotalCostJod)
                    throw new InvalidOperationException(
                        "A destination fuel ledger entry does not match this transfer."
                    );

                destinationEntry.EntryType = FuelLedgerEntryType.Reversal;
                destinationEntry.QuantityDelta = 0;
                destinationEntry.ValueDeltaJod = 0;
            }
        }

        sourceEntry.EntryType = FuelLedgerEntryType.Reversal;
        sourceEntry.QuantityDelta = 0;
        sourceEntry.ValueDeltaJod = 0;
        original.Status = FuelApprovalStatus.Reversed;
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return (await GetByIdAsync(original.Id))!;
    }

    private async Task<(decimal Quantity, decimal Value)> Balance(int vehicleId)
    {
        var entries = db.VehicleFuelLedgerEntries.Where(x => x.VehicleId == vehicleId);
        var quantity = await entries.SumAsync(x => (decimal?)x.QuantityDelta) ?? 0;
        var value = await entries.SumAsync(x => (decimal?)x.ValueDeltaJod) ?? 0;
        return (quantity, value);
    }

    private static IQueryable<FuelTransferDto> Project(IQueryable<FuelTransfer> q) =>
        q.Select(x => new FuelTransferDto
        {
            Id = x.Id,
            SourceVehicleId = x.SourceVehicleId,
            SourcePlateNumber = x.SourceVehicle.PlateNumber,
            OccurredAt = x.OccurredAt,
            Notes = x.Notes,
            Destinations = x
                .Details.Select(d => new FuelTransferDetailResultDto
                {
                    Id = d.Id,
                    DestinationType = d.DestinationType,
                    DestinationVehicleId = d.DestinationVehicleId,
                    DestinationPlateNumber =
                        d.DestinationVehicle == null ? null : d.DestinationVehicle.PlateNumber,
                    Quantity = d.Quantity,
                    UnitCostJod = d.UnitCostJod,
                    TotalCostJod = d.TotalCostJod,
                    ReturnedFuelPurchaseId = d.ReturnedFuelPurchaseId,
                })
                .ToList(),
        });
}
