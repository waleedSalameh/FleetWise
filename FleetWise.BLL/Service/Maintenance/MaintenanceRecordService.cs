
using FleetWise.BLL.DTOs.Maintenance.MaintenanceConsumption;
using FleetWise.BLL.DTOs.Maintenance.MaintenanceRecords;
using FleetWise.BLL.Interface.Maintenance;
using FleetWise.DAL.Data;
using FleetWise.DAL.Entities;
using FleetWise.DAL.Entities.Maintenance;
using FleetWise.DAL.Enums;
using FleetWise.BLL.Service.Concurrency;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FleetWise.BLL.Service.Maintenance
{
    public class MaintenanceRecordService : IMaintenanceRecord
    {
        private readonly FleetWiseDbContext _context;

        public MaintenanceRecordService(FleetWiseDbContext context)
        {
            _context = context;
        }

        // GET ALL
        public async Task<List<MaintenanceRecordDto>> GetAllAsync()
        {
            var records = await _context.MaintenanceRecords
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.MaintenanceDate)
                .Select(x => new MaintenanceRecordDto
                {
                    MaintenanceRecordId = x.MaintenanceRecordId,
                    VehicleId = x.VehicleId,
                    PlateNumber = x.Vehicle.PlateNumber.ToString(),
                    MaintenanceType = x.MaintenanceType,
                    MaintenanceLocation = x.MaintenanceLocation,
                    MaintenanceDate = x.MaintenanceDate,
                    Odometer = x.Odometer,
                    Description = x.Description,
                    WorkshopName = x.WorkshopName,
                    TechnicianName = x.TechnicianName,
                    PartsCost = x.PartsCost,
                    LaborCost = x.LaborCost,
                    TotalCost = x.TotalCost,
                    Status = x.Status
                })
                .ToListAsync();

            await LoadConsumptionAsync(records);
            return records;
        }

        // GET BY ID
        public async Task<MaintenanceRecordDto?> GetByIdAsync(int id)
        {
            var record = await _context.MaintenanceRecords
                .Where(x => x.MaintenanceRecordId == id && !x.IsDeleted)
                .Select(x => new MaintenanceRecordDto
                {
                    MaintenanceRecordId = x.MaintenanceRecordId,
                    VehicleId = x.VehicleId,
                    PlateNumber = x.Vehicle.PlateNumber.ToString(),
                    MaintenanceType = x.MaintenanceType,
                    MaintenanceLocation = x.MaintenanceLocation,
                    MaintenanceDate = x.MaintenanceDate,
                    Odometer = x.Odometer,
                    Description = x.Description,
                    WorkshopName = x.WorkshopName,
                    TechnicianName = x.TechnicianName,
                    PartsCost = x.PartsCost,
                    LaborCost = x.LaborCost,
                    TotalCost = x.TotalCost,
                    Status = x.Status
                })
                .FirstOrDefaultAsync();

            if (record != null)
                await LoadConsumptionAsync(new List<MaintenanceRecordDto> { record });
            return record;
        }

        // CREATE
        public async Task<MaintenanceRecordDto> CreateAsync(
            CreateMaintenanceRecordDto dto)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(x =>
                    x.VehicleId == dto.VehicleId && !x.IsDeleted)
                ?? throw new ArgumentException("Vehicle not found.");

            ValidateMaintenance(
                dto.Odometer,
                dto.LaborCost,
                dto.MaintenanceLocation);

            var maintenanceRecord = new MaintenanceRecord
            {
                VehicleId = dto.VehicleId,
                MaintenanceType = dto.MaintenanceType.Trim(),
                MaintenanceLocation = dto.MaintenanceLocation,
                MaintenanceDate = dto.MaintenanceDate,
                Odometer = dto.Odometer,
                Description = dto.Description,
                WorkshopName = dto.WorkshopName,
                TechnicianName = dto.TechnicianName,
                PartsCost = 0,
                LaborCost = dto.LaborCost,
                TotalCost = dto.LaborCost,
                Status = MaintenanceStatus.Scheduled
            };

            if (dto.Odometer > vehicle.CurrentOdometer)
                vehicle.CurrentOdometer = dto.Odometer;

            _context.MaintenanceRecords.Add(maintenanceRecord);

            await _context.SaveChangesAsync();

            decimal partsCost = 0;

            if (dto.MaintenanceLocation == MaintenanceLocation.Internal)
            {
                partsCost = await ConsumeMaterialsAsync(
                    maintenanceRecord.MaintenanceRecordId,
                    dto.Oils,
                    dto.SpareParts);
            }

            maintenanceRecord.PartsCost = partsCost;
            maintenanceRecord.TotalCost = partsCost + dto.LaborCost;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (await GetByIdAsync(
                maintenanceRecord.MaintenanceRecordId))!;
        }

        // UPDATE
        public async Task<MaintenanceRecordDto?> UpdateAsync(
            int id,
            UpdateMaintenanceRecordDto dto)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            await DatabaseApplicationLock.AcquireAsync(
                _context, $"FleetWise:MaintenanceRecord:{id}");

            var maintenanceRecord = await _context.MaintenanceRecords
                .FirstOrDefaultAsync(x =>
                    x.MaintenanceRecordId == id && !x.IsDeleted);

            if (maintenanceRecord == null)
                return null;

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(x =>
                    x.VehicleId == maintenanceRecord.VehicleId &&
                    !x.IsDeleted)
                ?? throw new ArgumentException("Vehicle not found.");

            ValidateMaintenance(
                dto.Odometer,
                dto.LaborCost,
                dto.MaintenanceLocation);

            if (!Enum.IsDefined(typeof(MaintenanceStatus), dto.Status))
                throw new ArgumentException("Invalid maintenance status.");

            // Return all previous consumed materials.
            await ReturnPreviousConsumptionAsync(id);

            maintenanceRecord.MaintenanceType = dto.MaintenanceType.Trim();
            maintenanceRecord.MaintenanceLocation = dto.MaintenanceLocation;
            maintenanceRecord.MaintenanceDate = dto.MaintenanceDate;
            maintenanceRecord.Odometer = dto.Odometer;
            maintenanceRecord.Description = dto.Description;
            maintenanceRecord.WorkshopName = dto.WorkshopName;
            maintenanceRecord.TechnicianName = dto.TechnicianName;
            maintenanceRecord.LaborCost = dto.LaborCost;
            maintenanceRecord.Status = dto.Status;

            if (dto.Odometer > vehicle.CurrentOdometer)
                vehicle.CurrentOdometer = dto.Odometer;

            decimal partsCost = 0;

            // Do not consume materials for external or cancelled maintenance.
            if (dto.MaintenanceLocation == MaintenanceLocation.Internal &&
                dto.Status != MaintenanceStatus.Cancelled)
            {
                partsCost = await ConsumeMaterialsAsync(
                    id,
                    dto.Oils,
                    dto.SpareParts);
            }

            maintenanceRecord.PartsCost = partsCost;
            maintenanceRecord.TotalCost = partsCost + dto.LaborCost;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return await GetByIdAsync(id);
        }

        // DELETE (SOFT DELETE)
        public async Task<bool> DeleteAsync(int id)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            await DatabaseApplicationLock.AcquireAsync(
                _context, $"FleetWise:MaintenanceRecord:{id}");
            var maintenanceRecord = await _context.MaintenanceRecords
                .FirstOrDefaultAsync(x =>
                    x.MaintenanceRecordId == id && !x.IsDeleted);

            if (maintenanceRecord == null)
                return false;

            await ReturnPreviousConsumptionAsync(id);
            maintenanceRecord.IsDeleted = true;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }

        private async Task LoadConsumptionAsync(List<MaintenanceRecordDto> records)
        {
            if (records.Count == 0) return;
            var ids = records.Select(x => x.MaintenanceRecordId).ToList();
            var oilMovements = await _context.OilStockMovements
                .Where(x => x.MaintenanceRecordId != null && ids.Contains(x.MaintenanceRecordId.Value))
                .Select(x => new { x.MaintenanceRecordId, x.OilId, x.MovementType, x.Quantity })
                .ToListAsync();
            var partMovements = await _context.SparePartMovements
                .Where(x => x.MaintenanceRecordId != null && ids.Contains(x.MaintenanceRecordId.Value))
                .Select(x => new { x.MaintenanceRecordId, x.SparePartId, x.MovementType, x.Quantity })
                .ToListAsync();

            foreach (var record in records)
            {
                record.Oils = oilMovements
                    .Where(x => x.MaintenanceRecordId == record.MaintenanceRecordId)
                    .GroupBy(x => x.OilId)
                    .Select(g => new OilConsumptionDto
                    {
                        OilId = g.Key,
                        Quantity = g.Sum(x => x.MovementType == InventoryMovementType.Issue ? x.Quantity :
                            x.MovementType == InventoryMovementType.Return ? -x.Quantity : 0)
                    })
                    .Where(x => x.Quantity > 0)
                    .ToList();
                record.SpareParts = partMovements
                    .Where(x => x.MaintenanceRecordId == record.MaintenanceRecordId)
                    .GroupBy(x => x.SparePartId)
                    .Select(g => new SparePartConsumptionDto
                    {
                        SparePartId = g.Key,
                        Quantity = g.Sum(x => x.MovementType == InventoryMovementType.Issue ? x.Quantity :
                            x.MovementType == InventoryMovementType.Return ? -x.Quantity : 0)
                    })
                    .Where(x => x.Quantity > 0)
                    .ToList();
            }
        }

        // CONSUME OILS AND SPARE PARTS
        private async Task<decimal> ConsumeMaterialsAsync(
            int maintenanceRecordId,
            List<OilConsumptionDto>? oils,
            List<SparePartConsumptionDto>? spareParts)
        {
            decimal totalPartsCost = 0;
            var oilGroups = (oils ?? new()).GroupBy(x => x.OilId)
                .OrderBy(group => group.Key).ToList();
            var sparePartGroups = (spareParts ?? new()).GroupBy(x => x.SparePartId)
                .OrderBy(group => group.Key).ToList();

            foreach (var group in oilGroups)
            {
                await DatabaseApplicationLock.AcquireAsync(
                    _context, $"FleetWise:Inventory:Oil:{group.Key}");
            }

            foreach (var group in sparePartGroups)
            {
                await DatabaseApplicationLock.AcquireAsync(
                    _context, $"FleetWise:Inventory:SparePart:{group.Key}");
            }

            // OILS
            foreach (var item in oilGroups)
            {
                decimal quantity = item.Sum(x => x.Quantity);

                if (quantity <= 0)
                    throw new ArgumentException("Oil quantity must be greater than zero.");

                var oil = await _context.Oils
                    .FirstOrDefaultAsync(x =>
                        x.OilId == item.Key && !x.IsDeleted)
                    ?? throw new ArgumentException(
                        $"Oil with ID {item.Key} not found.");

                decimal currentStock = await GetOilStockAsync(oil.OilId);

                if (quantity > currentStock)
                    throw new ArgumentException(
                        $"Insufficient stock for oil: {oil.OilName}. Available: {currentStock}");

                _context.OilStockMovements.Add(new OilStockMovement
                {
                    OilId = oil.OilId,
                    MovementType = InventoryMovementType.Issue,
                    Quantity = quantity,
                    UnitCost = oil.PurchasePrice,
                    Date = DateTime.UtcNow,
                    MaintenanceRecordId = maintenanceRecordId,
                    Notes = "Oil consumed during internal maintenance"
                });

                totalPartsCost += quantity * oil.PurchasePrice;
            }

            // SPARE PARTS
            foreach (var item in sparePartGroups)
            {
                decimal quantity = item.Sum(x => x.Quantity);

                if (quantity <= 0)
                    throw new ArgumentException(
                        "Spare part quantity must be greater than zero.");

                var sparePart = await _context.SpareParts
                    .FirstOrDefaultAsync(x =>
                        x.SparePartId == item.Key && !x.IsDeleted)
                    ?? throw new ArgumentException(
                        $"Spare part with ID {item.Key} not found.");

                decimal currentStock =
                    await GetSparePartStockAsync(sparePart.SparePartId);

                if (quantity > currentStock)
                    throw new ArgumentException(
                        $"Insufficient stock for spare part: {sparePart.PartName}. Available: {currentStock}");

                _context.SparePartMovements.Add(new SparePartMovement
                {
                    SparePartId = sparePart.SparePartId,
                    MovementType = InventoryMovementType.Issue,
                    Quantity = quantity,
                    UnitCost = sparePart.PurchasePrice,
                    Date = DateTime.UtcNow,
                    MaintenanceRecordId = maintenanceRecordId,
                    Notes = "Spare part consumed during internal maintenance"
                });

                totalPartsCost += quantity * sparePart.PurchasePrice;
            }

            await _context.SaveChangesAsync();

            return totalPartsCost;
        }

        // RETURN PREVIOUS OIL AND SPARE PART CONSUMPTION
        private async Task ReturnPreviousConsumptionAsync(int maintenanceRecordId)
        {
            // OILS
            var oilMovements = await _context.OilStockMovements
                .Where(x => x.MaintenanceRecordId == maintenanceRecordId)
                .ToListAsync();

            var oilConsumption = oilMovements
                .GroupBy(x => new { x.OilId, x.UnitCost })
                .Select(g => new
                {
                    g.Key.OilId,
                    g.Key.UnitCost,
                    Quantity = g.Sum(x =>
                        x.MovementType == InventoryMovementType.Issue
                            ? x.Quantity
                            : x.MovementType == InventoryMovementType.Return
                                ? -x.Quantity
                                : 0)
                })
                .Where(x => x.Quantity > 0)
                .ToList();

            foreach (var item in oilConsumption)
            {
                _context.OilStockMovements.Add(new OilStockMovement
                {
                    OilId = item.OilId,
                    MovementType = InventoryMovementType.Return,
                    Quantity = item.Quantity,
                    UnitCost = item.UnitCost,
                    Date = DateTime.UtcNow,
                    MaintenanceRecordId = maintenanceRecordId,
                    Notes = "Returned due to maintenance update or cancellation"
                });
            }

            // SPARE PARTS
            var sparePartMovements = await _context.SparePartMovements
                .Where(x => x.MaintenanceRecordId == maintenanceRecordId)
                .ToListAsync();

            var sparePartConsumption = sparePartMovements
                .GroupBy(x => new { x.SparePartId, x.UnitCost })
                .Select(g => new
                {
                    g.Key.SparePartId,
                    g.Key.UnitCost,
                    Quantity = g.Sum(x =>
                        x.MovementType == InventoryMovementType.Issue
                            ? x.Quantity
                            : x.MovementType == InventoryMovementType.Return
                                ? -x.Quantity
                                : 0)
                })
                .Where(x => x.Quantity > 0)
                .ToList();

            foreach (var item in oilConsumption.Select(x => x.OilId).Distinct().Order())
            {
                await DatabaseApplicationLock.AcquireAsync(
                    _context, $"FleetWise:Inventory:Oil:{item}");
            }

            foreach (var item in sparePartConsumption.Select(x => x.SparePartId).Distinct().Order())
            {
                await DatabaseApplicationLock.AcquireAsync(
                    _context, $"FleetWise:Inventory:SparePart:{item}");
            }

            foreach (var item in sparePartConsumption)
            {
                _context.SparePartMovements.Add(new SparePartMovement
                {
                    SparePartId = item.SparePartId,
                    MovementType = InventoryMovementType.Return,
                    Quantity = item.Quantity,
                    UnitCost = item.UnitCost,
                    Date = DateTime.UtcNow,
                    MaintenanceRecordId = maintenanceRecordId,
                    Notes = "Returned due to maintenance update or cancellation"
                });
            }

            await _context.SaveChangesAsync();
        }

        // GET CURRENT OIL STOCK
        private async Task<decimal> GetOilStockAsync(int oilId)
        {
            var movements = await _context.OilStockMovements
                .Where(x => x.OilId == oilId)
                .Select(x => new { x.MovementType, x.Quantity })
                .ToListAsync();

            return movements.Sum(x =>
                x.MovementType == InventoryMovementType.Receipt ||
                x.MovementType == InventoryMovementType.Return ||
                x.MovementType == InventoryMovementType.Adjustment
                    ? x.Quantity
                    : x.MovementType == InventoryMovementType.Issue
                        ? -x.Quantity
                        : 0);
        }

        // GET CURRENT SPARE PART STOCK
        private async Task<decimal> GetSparePartStockAsync(int sparePartId)
        {
            var movements = await _context.SparePartMovements
                .Where(x => x.SparePartId == sparePartId)
                .Select(x => new { x.MovementType, x.Quantity })
                .ToListAsync();

            return movements.Sum(x =>
                x.MovementType == InventoryMovementType.Receipt ||
                x.MovementType == InventoryMovementType.Return ||
                x.MovementType == InventoryMovementType.Adjustment
                    ? x.Quantity
                    : x.MovementType == InventoryMovementType.Issue
                        ? -x.Quantity
                        : 0);
        }

        // VALIDATION
        private void ValidateMaintenance(
            int odometer,
            decimal laborCost,
            MaintenanceLocation location)
        {
            if (odometer < 0)
                throw new ArgumentException(
                    "Odometer cannot be negative.");

            if (laborCost < 0)
                throw new ArgumentException(
                    "Labor cost cannot be negative.");

            if (!Enum.IsDefined(typeof(MaintenanceLocation), location))
                throw new ArgumentException(
                    "Invalid maintenance location.");
        }
    }
}
