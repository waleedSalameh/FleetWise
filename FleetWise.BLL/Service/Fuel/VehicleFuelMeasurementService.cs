using System.Data;
using FleetWise.BLL.DTOs.FuelRecords;
using FleetWise.BLL.Interface.Fuel;
using FleetWise.DAL.Data;
using FleetWise.DAL.Entities.Fuel;
using FleetWise.DAL.Enums.Fuel;
using FleetWise.BLL.Service.Concurrency;
using Microsoft.EntityFrameworkCore;

namespace FleetWise.BLL.Service.Fuel;

public class VehicleFuelMeasurementService(FleetWiseDbContext db) : IVehicleFuelMeasurementService
{
    public async Task<FuelMeasurementDto> CreateAsync(CreateFuelMeasurementDto dto)
    {
        var vehicle = await db.Vehicles.FirstOrDefaultAsync(x =>
            x.VehicleId == dto.VehicleId && !x.IsDeleted
        ) ?? throw new ArgumentException("Vehicle not found.");
        if (dto.MeasuredAt == default)
            throw new ArgumentException("Measurement date and time are required.");
        if (dto.MeasuredQuantity < 0 || dto.MeasuredQuantity > vehicle.FuelTankCapacity)
            throw new ArgumentException(
                "Measured fuel quantity must be within the vehicle tank capacity."
            );
        if (!Enum.IsDefined(dto.MeasurementType))
            throw new ArgumentException("Invalid measurement type.");
        if (dto.MeasurementType == FuelMeasurementType.Opening && dto.MeasuredValueJod is null)
            throw new ArgumentException("Opening measurement requires an opening value in JOD.");
        if (
            dto.MeasurementType == FuelMeasurementType.Closing
            && await db.MonthlyFuelClosings.AnyAsync(x =>
                x.VehicleId == dto.VehicleId
                && x.PeriodEnd >= DateOnly.FromDateTime(dto.MeasuredAt.Date)
            )
        )
            throw new InvalidOperationException("This accounting period is already closed.");
        if (
            dto.MeasurementType == FuelMeasurementType.Closing
            && dto.MeasuredAt.Date
                != new DateTime(
                    dto.MeasuredAt.Year,
                    dto.MeasuredAt.Month,
                    DateTime.DaysInMonth(dto.MeasuredAt.Year, dto.MeasuredAt.Month)
                )
        )
            throw new ArgumentException(
                "Monthly closing measurement must be recorded on the last day of the month."
            );
        var measurement = new VehicleFuelMeasurement
        {
            VehicleId = dto.VehicleId,
            MeasuredAt = dto.MeasuredAt,
            MeasuredQuantity = dto.MeasuredQuantity,
            MeasuredValueJod = dto.MeasuredValueJod,
            MeasurementType = dto.MeasurementType,
            Notes = dto.Notes?.Trim(),
            Status = FuelApprovalStatus.Draft,
        };
        db.VehicleFuelMeasurements.Add(measurement);
        await db.SaveChangesAsync();
        return await GetDto(measurement.Id);
    }

    public async Task<FuelMeasurementDto> ApproveAsync(long id, string approvedBy)
    {
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
        await DatabaseApplicationLock.AcquireAsync(
            db, $"FleetWise:FuelMeasurement:{id}");

        var m = await db
            .VehicleFuelMeasurements.Include(x => x.Vehicle)
            .FirstOrDefaultAsync(x => x.Id == id && x.Status == FuelApprovalStatus.Draft)
            ?? throw new ArgumentException("Pending fuel measurement was not found.");

        await DatabaseApplicationLock.AcquireAsync(
            db, $"FleetWise:Fuel:Vehicle:{m.VehicleId}");
        if (m.MeasuredQuantity < 0 || m.MeasuredQuantity > m.Vehicle.FuelTankCapacity)
            throw new ArgumentException("Measurement exceeds the vehicle tank capacity.");
        if (m.MeasurementType == FuelMeasurementType.Opening)
        {
            if (m.MeasuredValueJod is null || m.MeasuredValueJod < 0)
                throw new ArgumentException(
                    "Opening measurement requires a non-negative JOD value."
                );
            if (
                await db.VehicleFuelMeasurements.AnyAsync(x =>
                    x.VehicleId == m.VehicleId
                    && x.MeasurementType == FuelMeasurementType.Opening
                    && x.Status == FuelApprovalStatus.Approved
                )
            )
                throw new InvalidOperationException(
                    "An opening measurement has already been approved for this vehicle."
                );
            if (await db.VehicleFuelLedgerEntries.AnyAsync(x => x.VehicleId == m.VehicleId))
                throw new InvalidOperationException(
                    "Opening measurement must be recorded before fuel transactions."
                );
            if (
                await db.FuelRecords.AnyAsync(x =>
                    x.VehicleId == m.VehicleId && !x.IsDeleted && x.Date >= m.MeasuredAt
                )
            )
                throw new InvalidOperationException(
                    "Opening measurement cannot be dated before or at a recorded fuel transaction."
                );
            db.VehicleFuelLedgerEntries.Add(
                new VehicleFuelLedgerEntry
                {
                    VehicleId = m.VehicleId,
                    OccurredAt = m.MeasuredAt,
                    EntryType = FuelLedgerEntryType.OpeningMeasurement,
                    QuantityDelta = m.MeasuredQuantity,
                    ValueDeltaJod = m.MeasuredValueJod.Value,
                    MeasurementId = m.Id,
                }
            );
        }
        else if (m.MeasurementType == FuelMeasurementType.Closing)
        {
            if (
                m.MeasuredAt.Date
                != new DateTime(
                    m.MeasuredAt.Year,
                    m.MeasuredAt.Month,
                    DateTime.DaysInMonth(m.MeasuredAt.Year, m.MeasuredAt.Month)
                )
            )
                throw new ArgumentException(
                    "Monthly closing measurement must be recorded on the last day of the month."
                );
            var start = new DateOnly(m.MeasuredAt.Year, m.MeasuredAt.Month, 1);
            var end = DateOnly.FromDateTime(m.MeasuredAt.Date);
            var opening = await db
                .MonthlyFuelClosings.Where(x =>
                    x.VehicleId == m.VehicleId
                    && x.Status == FuelApprovalStatus.Approved
                    && x.PeriodEnd < start
                )
                .OrderByDescending(x => x.PeriodEnd)
                .FirstOrDefaultAsync();
            decimal openingQuantity;
            decimal openingValue;
            VehicleFuelMeasurement? initialOpening = null;
            if (opening is null)
            {
                initialOpening = await db
                    .VehicleFuelMeasurements.Where(x =>
                        x.VehicleId == m.VehicleId
                        && x.MeasurementType == FuelMeasurementType.Opening
                        && x.Status == FuelApprovalStatus.Approved
                        && x.MeasuredAt < m.MeasuredAt
                    )
                    .OrderByDescending(x => x.MeasuredAt)
                    .FirstOrDefaultAsync()
                    ?? throw new InvalidOperationException(
                        "A reliable opening measurement is required before closing this period."
                    );
                start = DateOnly.FromDateTime(initialOpening.MeasuredAt);
                openingQuantity = initialOpening.MeasuredQuantity;
                openingValue = initialOpening.MeasuredValueJod ?? 0;
            }
            else
            {
                openingQuantity = opening.ClosingQuantity;
                openingValue = opening.ClosingValueJod;
            }
            var lowerBound = initialOpening?.MeasuredAt ?? start.ToDateTime(TimeOnly.MinValue);
            var entries = await db
                .VehicleFuelLedgerEntries.Where(x =>
                    x.VehicleId == m.VehicleId
                    && x.OccurredAt >= lowerBound
                    && x.OccurredAt <= m.MeasuredAt
                    && x.EntryType != FuelLedgerEntryType.MonthlyClosing
                    && (initialOpening == null || x.MeasurementId != initialOpening.Id)
                )
                .ToListAsync();
            var currentQuantity = openingQuantity + entries.Sum(x => x.QuantityDelta);
            var currentValue = openingValue + entries.Sum(x => x.ValueDeltaJod);
            if (m.MeasuredQuantity > currentQuantity)
                throw new InvalidOperationException(
                    "Closing measured quantity exceeds the available ledger quantity."
                );
            var average = currentQuantity == 0 ? 0 : currentValue / currentQuantity;
            var closingValue = Math.Round(
                m.MeasuredQuantity * average,
                3,
                MidpointRounding.AwayFromZero
            );
            m.MeasuredValueJod = closingValue;
            var fueling = await db
                .FuelRecords.Where(x =>
                    x.VehicleId == m.VehicleId
                    && !x.IsDeleted
                    && x.Date >= lowerBound
                    && x.Date <= m.MeasuredAt
                )
                .ToListAsync();
            var incoming = entries
                .Where(x => x.EntryType == FuelLedgerEntryType.TransferIn)
                .ToList();
            var outgoing = entries
                .Where(x => x.EntryType == FuelLedgerEntryType.TransferOut)
                .ToList();
            var corrections = entries
                .Where(x => x.EntryType == FuelLedgerEntryType.Correction)
                .ToList();
            var consumptionQty =
                openingQuantity
                + fueling.Sum(x => x.Quantity)
                + incoming.Sum(x => x.QuantityDelta)
                - outgoing.Sum(x => -x.QuantityDelta)
                - m.MeasuredQuantity;
            var consumptionValue = Math.Round(
                openingValue
                    + fueling.Sum(x => x.TotalCost)
                    + incoming.Sum(x => x.ValueDeltaJod)
                    - outgoing.Sum(x => -x.ValueDeltaJod)
                    - closingValue,
                3,
                MidpointRounding.AwayFromZero
            );
            consumptionQty += corrections.Sum(x => x.QuantityDelta);
            consumptionValue = Math.Round(
                consumptionValue + corrections.Sum(x => x.ValueDeltaJod),
                3,
                MidpointRounding.AwayFromZero
            );
            if (consumptionQty < 0 || consumptionValue < 0)
                throw new InvalidOperationException(
                    "Reconciliation produced a negative consumption balance; review the period transactions."
                );
            db.MonthlyFuelClosings.Add(
                new MonthlyFuelClosing
                {
                    VehicleId = m.VehicleId,
                    PeriodStart = start,
                    PeriodEnd = end,
                    OpeningMeasurementId = opening?.ClosingMeasurementId ?? initialOpening!.Id,
                    ClosingMeasurementId = m.Id,
                    OpeningQuantity = openingQuantity,
                    OpeningValueJod = openingValue,
                    TotalFuelingQuantity = fueling.Sum(x => x.Quantity),
                    TotalFuelingValueJod = fueling.Sum(x => x.TotalCost),
                    IncomingTransferQuantity = incoming.Sum(x => x.QuantityDelta),
                    IncomingTransferValueJod = incoming.Sum(x => x.ValueDeltaJod),
                    OutgoingTransferQuantity = outgoing.Sum(x => -x.QuantityDelta),
                    OutgoingTransferValueJod = outgoing.Sum(x => -x.ValueDeltaJod),
                    ClosingQuantity = m.MeasuredQuantity,
                    ClosingValueJod = closingValue,
                    ActualConsumptionQuantity = consumptionQty,
                    ActualConsumptionValueJod = consumptionValue,
                    InternalFuelingCount = fueling.Count(x =>
                        x.FuelSourceType == FleetWise.DAL.Enums.FuelSourceType.Internal
                    ),
                    ExternalFuelingCount = fueling.Count(x =>
                        x.FuelSourceType == FleetWise.DAL.Enums.FuelSourceType.External
                    ),
                    IncomingTransferCount = incoming
                        .Where(x => x.FuelTransferDetailId != null)
                        .Select(x => x.FuelTransferDetailId)
                        .Distinct()
                        .Count(),
                    OutgoingTransferCount = outgoing.Count,
                    Status = FuelApprovalStatus.Approved,
                    ApprovedBy = approvedBy.Trim(),
                    ApprovedAt = DateTime.UtcNow,
                }
            );
            db.VehicleFuelLedgerEntries.Add(
                new VehicleFuelLedgerEntry
                {
                    VehicleId = m.VehicleId,
                    OccurredAt = m.MeasuredAt,
                    EntryType = FuelLedgerEntryType.MonthlyClosing,
                    QuantityDelta = m.MeasuredQuantity - currentQuantity,
                    ValueDeltaJod = closingValue - currentValue,
                    MeasurementId = m.Id,
                }
            );
        }
        else if (m.MeasurementType == FuelMeasurementType.Correction)
        {
            if (
                await db.MonthlyFuelClosings.AnyAsync(x =>
                    x.VehicleId == m.VehicleId
                    && x.Status == FuelApprovalStatus.Approved
                    && x.PeriodStart <= DateOnly.FromDateTime(m.MeasuredAt)
                    && x.PeriodEnd >= DateOnly.FromDateTime(m.MeasuredAt)
                )
            )
                throw new InvalidOperationException(
                    "Corrections to an approved accounting period are not allowed."
                );
            var openingExists = await db.VehicleFuelMeasurements.AnyAsync(x =>
                x.VehicleId == m.VehicleId
                && x.MeasurementType == FuelMeasurementType.Opening
                && x.Status == FuelApprovalStatus.Approved
            );
            if (!openingExists)
                throw new InvalidOperationException(
                    "Record and approve the vehicle opening measurement before corrections."
                );
            var latestEntry = await db
                .VehicleFuelLedgerEntries.Where(x => x.VehicleId == m.VehicleId)
                .MaxAsync(x => (DateTime?)x.OccurredAt);
            if (latestEntry.HasValue && m.MeasuredAt < latestEntry.Value)
                throw new InvalidOperationException(
                    "Fuel balance corrections cannot be backdated before existing fuel transactions."
                );
            var currentQuantity = await db
                .VehicleFuelLedgerEntries.Where(x => x.VehicleId == m.VehicleId)
                .SumAsync(x => (decimal?)x.QuantityDelta)
                ?? 0;
            var currentValue = await db
                .VehicleFuelLedgerEntries.Where(x => x.VehicleId == m.VehicleId)
                .SumAsync(x => (decimal?)x.ValueDeltaJod)
                ?? 0;
            var unitValue = currentQuantity == 0 ? 0 : currentValue / currentQuantity;
            var correctedValue = Math.Round(
                m.MeasuredQuantity * unitValue,
                3,
                MidpointRounding.AwayFromZero
            );
            m.MeasuredValueJod = correctedValue;
            db.VehicleFuelLedgerEntries.Add(
                new VehicleFuelLedgerEntry
                {
                    VehicleId = m.VehicleId,
                    OccurredAt = m.MeasuredAt,
                    EntryType = FuelLedgerEntryType.Correction,
                    QuantityDelta = m.MeasuredQuantity - currentQuantity,
                    ValueDeltaJod = correctedValue - currentValue,
                    MeasurementId = m.Id,
                }
            );
        }
        m.Status = FuelApprovalStatus.Approved;
        m.ApprovedBy = approvedBy.Trim();
        m.ApprovedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        await tx.CommitAsync();
        return await GetDto(m.Id);
    }

    public async Task<List<FuelMeasurementDto>> GetHistoryAsync(int vehicleId) =>
        await db
            .VehicleFuelMeasurements.AsNoTracking()
            .Where(x => x.VehicleId == vehicleId)
            .OrderByDescending(x => x.MeasuredAt)
            .Select(x => new FuelMeasurementDto
            {
                Id = x.Id,
                VehicleId = x.VehicleId,
                PlateNumber = x.Vehicle.PlateNumber,
                MeasuredAt = x.MeasuredAt,
                MeasuredQuantity = x.MeasuredQuantity,
                MeasuredValueJod = x.MeasuredValueJod,
                MeasurementType = x.MeasurementType,
                Status = x.Status,
                Notes = x.Notes,
                ApprovedBy = x.ApprovedBy,
                ApprovedAt = x.ApprovedAt,
            })
            .ToListAsync();

    private async Task<FuelMeasurementDto> GetDto(long id) =>
        await db
            .VehicleFuelMeasurements.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new FuelMeasurementDto
            {
                Id = x.Id,
                VehicleId = x.VehicleId,
                PlateNumber = x.Vehicle.PlateNumber,
                MeasuredAt = x.MeasuredAt,
                MeasuredQuantity = x.MeasuredQuantity,
                MeasuredValueJod = x.MeasuredValueJod,
                MeasurementType = x.MeasurementType,
                Status = x.Status,
                Notes = x.Notes,
                ApprovedBy = x.ApprovedBy,
                ApprovedAt = x.ApprovedAt,
            })
            .FirstAsync();
}
