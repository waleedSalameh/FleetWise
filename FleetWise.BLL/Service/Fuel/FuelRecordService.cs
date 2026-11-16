using FleetWise.BLL.DTOs.FuelRecords;
using FleetWise.BLL.Interface.Fuel;
using FleetWise.DAL.Data;
using FleetWise.DAL.Entities;
using FleetWise.DAL.Entities.Fuel;
using FleetWise.DAL.Enums;
using FleetWise.DAL.Enums.Fuel;
using FleetWise.BLL.Service.Concurrency;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FleetWise.BLL.Service.Fuel
{
    public class FuelRecordService : IFuelRecord
    {
        private readonly FleetWiseDbContext _context;

        public FuelRecordService(FleetWiseDbContext context)
        {
            _context = context;
        }

        public async Task<FuelRecordDto> CreateAsync(CreateFuelRecordDto dto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            await DatabaseApplicationLock.AcquireAsync(
                _context, $"FleetWise:Fuel:Vehicle:{dto.VehicleId}");

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v =>
                    v.VehicleId == dto.VehicleId && !v.IsDeleted);

            if (vehicle == null)
                throw new ArgumentException("Vehicle not found.");

            ValidateFuelRecord(
                vehicle.CurrentOdometer,
                vehicle.FuelTankCapacity,
                dto.Odometer,
                dto.Quantity);
            await EnsurePeriodOpen(dto.VehicleId, dto.Date);
            await EnsureNotBackdated(dto.VehicleId, dto.Date);

            var totalCost = dto.Quantity * dto.PricePerLiter;

            var fuelRecord = new FuelRecord
            {
                VehicleId = dto.VehicleId,
                FuelSourceType = FuelSourceType.External,
                Date = dto.Date,
                Quantity = dto.Quantity,
                PricePerLiter = dto.PricePerLiter,
                TotalCost = totalCost,
                Odometer = dto.Odometer,
                StationName = dto.StationName.Trim()
            };

            _context.FuelRecords.Add(fuelRecord);
            _context.VehicleFuelLedgerEntries.Add(new VehicleFuelLedgerEntry
            {
                VehicleId = dto.VehicleId, OccurredAt = dto.Date, EntryType = FuelLedgerEntryType.Fueling,
                QuantityDelta = dto.Quantity, ValueDeltaJod = totalCost, FuelRecord = fuelRecord
            });

            vehicle.CurrentOdometer = dto.Odometer;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (await GetByIdAsync(fuelRecord.FuelRecordId))!;
        }

        public async Task<FuelRecordDto> CreateInternalAsync(CreateInternalFuelRecordDto dto)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            await DatabaseApplicationLock.AcquireAsync(
                _context, $"FleetWise:Fuel:Vehicle:{dto.VehicleId}");

            foreach (var batchId in (dto.Batches ?? new()).Select(x => x.FuelPurchaseId).Distinct().Order())
            {
                await DatabaseApplicationLock.AcquireAsync(
                    _context, $"FleetWise:Fuel:Batch:{batchId}");
            }

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.VehicleId == dto.VehicleId && !v.IsDeleted)
                ?? throw new ArgumentException("Vehicle not found.");

            ValidateFuelRecord(
                vehicle.CurrentOdometer,
                vehicle.FuelTankCapacity,
                dto.Odometer,
                dto.Quantity);
            await EnsurePeriodOpen(dto.VehicleId, dto.Date);
            await EnsureNotBackdated(dto.VehicleId, dto.Date);

            if (dto.Batches == null || dto.Batches.Count == 0)
                throw new ArgumentException("At least one fuel batch must be selected.");

            if (dto.Quantity <= 0 ||
                dto.Batches.Any(x => x.Quantity <= 0) ||
                dto.Batches.GroupBy(x => x.FuelPurchaseId).Any(g => g.Count() > 1))
                throw new ArgumentException("Invalid fuel batch selection.");

            if (dto.Batches.Sum(x => x.Quantity) != dto.Quantity)
                throw new ArgumentException("Batch quantities must equal the fuel record quantity.");

            var batchIds = dto.Batches.Select(x => x.FuelPurchaseId).ToList();

            var batches = await _context.FuelPurchases
                .Where(x => batchIds.Contains(x.FuelPurchaseId) && !x.IsDeleted)
                .ToDictionaryAsync(x => x.FuelPurchaseId);

            if (batches.Count != batchIds.Count)
                throw new ArgumentException("One or more fuel batches were not found.");

            decimal totalCost = 0;

            var fuelRecord = new FuelRecord
            {
                VehicleId = dto.VehicleId,
                FuelSourceType = FuelSourceType.Internal,
                Date = dto.Date,
                Quantity = dto.Quantity,
                Odometer = dto.Odometer
            };

            foreach (var selected in dto.Batches)
            {
                var batch = batches[selected.FuelPurchaseId];

                if (batch.Status != FuelPurchaseStatus.InUse ||
                    batch.RemainingQuantity < selected.Quantity)
                    throw new InvalidOperationException(
                        $"Fuel batch {batch.FuelPurchaseId} is not available or has insufficient quantity.");

                var cost = selected.Quantity * batch.PricePerLiter;

                totalCost += cost;
                batch.RemainingQuantity -= selected.Quantity;
                batch.Status = batch.RemainingQuantity == 0? FuelPurchaseStatus.Completed: FuelPurchaseStatus.InUse;

                fuelRecord.Consumptions.Add(new FuelRecordConsumption
                {
                    FuelPurchaseId = batch.FuelPurchaseId,
                    Quantity = selected.Quantity,
                    PricePerLiter = batch.PricePerLiter,
                    TotalCost = cost
                });
            }

            fuelRecord.TotalCost = totalCost;
            fuelRecord.PricePerLiter = Math.Round(
                totalCost / dto.Quantity, 3, MidpointRounding.AwayFromZero);

            _context.FuelRecords.Add(fuelRecord);
            _context.VehicleFuelLedgerEntries.Add(new VehicleFuelLedgerEntry
            {
                VehicleId = dto.VehicleId, OccurredAt = dto.Date, EntryType = FuelLedgerEntryType.Fueling,
                QuantityDelta = dto.Quantity, ValueDeltaJod = totalCost, FuelRecord = fuelRecord
            });
            vehicle.CurrentOdometer = dto.Odometer;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (await GetByIdAsync(fuelRecord.FuelRecordId))!;
        }
        private void ValidateFuelRecord( int currentOdometer, decimal fuelTankCapacity, int odometer,decimal quantity)
        {
            Console.WriteLine($"Current Odometer: {currentOdometer}");
            Console.WriteLine($"New Odometer: {odometer}");
            if (odometer < currentOdometer)
            {
                throw new InvalidOperationException(
                    "Odometer reading cannot be less than the last recorded reading.");
            }

            if (quantity > fuelTankCapacity)
            {
                throw new InvalidOperationException(
                    "Fuel quantity cannot exceed the vehicle's fuel tank capacity.");
            }

            if (quantity <= 0)
            {
                throw new ArgumentException(
                    "Fuel quantity must be greater than zero.");
            }
        }

        public async Task<List<FuelRecordDto>> GetAllAsync()
        {
            return await _context.FuelRecords
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.Date)
                .Select(x => new FuelRecordDto
                {
                    FuelRecordId = x.FuelRecordId,
                    VehicleId = x.VehicleId,
                    Date = x.Date,
                    Quantity = x.Quantity,
                    PricePerLiter = x.PricePerLiter,
                    TotalCost = x.TotalCost,
                    Odometer = x.Odometer,
                    PlateNumber = x.Vehicle.PlateNumber,
                    FuelSourceType = x.FuelSourceType,
                    StationName = x.StationName,
                    IsDeleted = x.IsDeleted,

                    Batches = x.Consumptions.Select(c =>
                        new FuelBatchConsumptionResultDto
                        {
                            FuelPurchaseId = c.FuelPurchaseId,
                            Quantity = c.Quantity,
                            PricePerLiter = c.PricePerLiter,
                            TotalCost = c.TotalCost
                        }).ToList()
                })
                .ToListAsync();
        }

        public async Task<FuelRecordDto?> GetByIdAsync(int id)
        {
            return await _context.FuelRecords
                .Where(x => x.FuelRecordId == id && !x.IsDeleted)
                .Select(x => new FuelRecordDto
                {
                    FuelRecordId = x.FuelRecordId,
                    VehicleId = x.VehicleId,
                    Date = x.Date,
                    Quantity = x.Quantity,
                    PricePerLiter = x.PricePerLiter,
                    TotalCost = x.TotalCost,
                    Odometer = x.Odometer,
                    PlateNumber = x.Vehicle.PlateNumber,
                    FuelSourceType = x.FuelSourceType,
                    StationName = x.StationName,
                    IsDeleted = x.IsDeleted,

                    Batches = x.Consumptions.Select(c =>
                        new FuelBatchConsumptionResultDto
                        {
                            FuelPurchaseId = c.FuelPurchaseId,
                            Quantity = c.Quantity,
                            PricePerLiter = c.PricePerLiter,
                            TotalCost = c.TotalCost
                        }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAsync(int id, UpdateFuelRecordDto dto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            await DatabaseApplicationLock.AcquireAsync(
                _context, $"FleetWise:FuelRecord:{id}");

            var fuel = await _context.FuelRecords
                .FirstOrDefaultAsync(x =>
                    x.FuelRecordId == id && !x.IsDeleted);

            if (fuel == null)
                return false;

            await DatabaseApplicationLock.AcquireAsync(
                _context, $"FleetWise:Fuel:Vehicle:{fuel.VehicleId}");

            if (fuel.FuelSourceType != FuelSourceType.External)
            {
                throw new InvalidOperationException(
                    "Internal fuel records cannot be updated.");
            }
            await EnsurePeriodOpen(fuel.VehicleId, fuel.Date);
            await EnsurePeriodOpen(fuel.VehicleId, dto.Date);
            await EnsureNotBackdated(fuel.VehicleId, dto.Date, id);

            if (fuel.VehicleId != dto.VehicleId ||
                fuel.Odometer != dto.Odometer)
            {
                throw new InvalidOperationException(
                    "Vehicle and odometer cannot be changed when updating a fuel record.");
            }

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v =>
                    v.VehicleId == fuel.VehicleId && !v.IsDeleted);

            if (vehicle == null)
                throw new ArgumentException("Vehicle not found.");

            if (dto.Quantity <= 0 ||
                dto.Quantity > vehicle.FuelTankCapacity)
            {
                throw new ArgumentException(
                    "Invalid fuel quantity.");
            }

            fuel.Date = dto.Date;
            fuel.Quantity = dto.Quantity;
            fuel.PricePerLiter = dto.PricePerLiter;
            fuel.TotalCost = dto.Quantity * dto.PricePerLiter;
            fuel.StationName = dto.StationName.Trim();

            var ledger = await _context.VehicleFuelLedgerEntries.FirstOrDefaultAsync(x => x.FuelRecordId == id);
            if (ledger != null)
            {
                ledger.OccurredAt = dto.Date;
                ledger.QuantityDelta = dto.Quantity;
                ledger.ValueDeltaJod = fuel.TotalCost;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            try
            {
                await DatabaseApplicationLock.AcquireAsync(
                    _context, $"FleetWise:FuelRecord:{id}");

                var fuel = await _context.FuelRecords
                    .Include(x => x.Consumptions)
                    .FirstOrDefaultAsync(x =>
                        x.FuelRecordId == id && !x.IsDeleted);

                if (fuel == null)
                    return false;

                await DatabaseApplicationLock.AcquireAsync(
                    _context, $"FleetWise:Fuel:Vehicle:{fuel.VehicleId}");

                foreach (var batchId in fuel.Consumptions
                    .Select(x => x.FuelPurchaseId).Distinct().Order())
                {
                    await DatabaseApplicationLock.AcquireAsync(
                        _context, $"FleetWise:Fuel:Batch:{batchId}");
                }

                await EnsurePeriodOpen(fuel.VehicleId, fuel.Date);

                if (await _context.FuelTransfers.AnyAsync(x =>
                    x.SourceVehicleId == fuel.VehicleId &&
                    x.OccurredAt >= fuel.Date &&
                    x.ReversesTransferId == null &&
                    x.Status == FuelApprovalStatus.Approved))
                {
                    throw new InvalidOperationException(
                        "Reverse later fuel transfers from this vehicle before deleting this fueling record.");
                }

                var ledger = await _context.VehicleFuelLedgerEntries
                    .FirstOrDefaultAsync(x => x.FuelRecordId == id);

                if (ledger != null)
                {
                    if (ledger.EntryType != FuelLedgerEntryType.Fueling ||
                        ledger.QuantityDelta != fuel.Quantity ||
                        ledger.ValueDeltaJod != fuel.TotalCost)
                    {
                        throw new InvalidOperationException(
                            "The vehicle fuel ledger does not match this fueling record.");
                    }

                    var vehicleEntries = _context.VehicleFuelLedgerEntries
                        .Where(x => x.VehicleId == fuel.VehicleId);
                    var currentQuantity = await vehicleEntries
                        .SumAsync(x => (decimal?)x.QuantityDelta) ?? 0;
                    var currentValue = await vehicleEntries
                        .SumAsync(x => (decimal?)x.ValueDeltaJod) ?? 0;

                    if (currentQuantity < ledger.QuantityDelta ||
                        currentValue < ledger.ValueDeltaJod)
                    {
                        throw new InvalidOperationException(
                            "Deleting this fueling record would make the vehicle fuel balance negative.");
                    }
                }

                if (fuel.FuelSourceType == FuelSourceType.Internal)
                {
                    foreach (var consumption in fuel.Consumptions)
                    {
                        var batch = await _context.FuelPurchases
                            .FirstOrDefaultAsync(x =>
                                x.FuelPurchaseId == consumption.FuelPurchaseId);

                        if (batch == null || batch.IsDeleted)
                        {
                            throw new InvalidOperationException(
                                "An active fuel batch related to this record was not found.");
                        }

                        if (batch.RemainingQuantity + consumption.Quantity > batch.Quantity)
                            throw new InvalidOperationException(
                                "Restoring this fueling record would exceed the original fuel batch quantity.");

                        batch.RemainingQuantity += consumption.Quantity;

                        // إرجاع حالة الدفعة إلى قيد الاستخدام
                        batch.Status = FuelPurchaseStatus.InUse;
                    }
                }

                fuel.IsDeleted = true;
                if (ledger != null)
                {
                    ledger.EntryType = FuelLedgerEntryType.Reversal;
                    ledger.QuantityDelta = 0;
                    ledger.ValueDeltaJod = 0;
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task EnsurePeriodOpen(int vehicleId, DateTime date)
        {
            var day = DateOnly.FromDateTime(date);
            if (await _context.MonthlyFuelClosings.AnyAsync(x => x.VehicleId == vehicleId && x.Status == FuelApprovalStatus.Approved && x.PeriodStart <= day && x.PeriodEnd >= day))
                throw new InvalidOperationException("Fuel transactions cannot be changed in an approved accounting period.");
        }

        private async Task EnsureNotBackdated(int vehicleId, DateTime date, int? excludingFuelRecordId = null)
        {
            var latest = await _context.VehicleFuelLedgerEntries.Where(x => x.VehicleId == vehicleId && (!excludingFuelRecordId.HasValue || x.FuelRecordId != excludingFuelRecordId)).MaxAsync(x => (DateTime?)x.OccurredAt);
            if (latest.HasValue && date < latest.Value)
                throw new InvalidOperationException("Backdated fuel records are not allowed after fuel accounting has started.");
        }
    }
}
