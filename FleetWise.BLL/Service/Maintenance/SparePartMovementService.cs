using FleetWise.BLL.DTOs.Maintenance.SparePartMovements;
using FleetWise.BLL.Interface;
using FleetWise.DAL.Data;
using FleetWise.DAL.Entities.Maintenance;
using FleetWise.DAL.Enums;
using FleetWise.BLL.Service.Concurrency;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FleetWise.BLL.Services.Maintenance
{
    public class SparePartMovementService : ISparePartMovementService
    {
        private readonly FleetWiseDbContext _context;

        public SparePartMovementService(FleetWiseDbContext context)
        {
            _context = context;
        }

        // Get All Movements
        public async Task<List<SparePartMovementDto>> GetAllAsync()
        {
            return await _context.SparePartMovements
                .Select(x => new SparePartMovementDto
                {
                    SparePartMovementId = x.SparePartMovementId,
                    SparePartId = x.SparePartId,
                    PartNumber = x.SparePart.PartNumber,
                    PartName = x.SparePart.PartName,
                    MovementType = x.MovementType.ToString(),
                    Quantity = x.Quantity,
                    UnitCost = x.UnitCost,
                    Date = x.Date,
                    MaintenanceRecordId = x.MaintenanceRecordId,
                    ReferenceNumber = x.ReferenceNumber,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        // Get Movements By Spare Part
        public async Task<List<SparePartMovementDto>> GetBySparePartIdAsync(int sparePartId)
        {
            return await _context.SparePartMovements
                .Where(x => x.SparePartId == sparePartId)
                .Select(x => new SparePartMovementDto
                {
                    SparePartMovementId = x.SparePartMovementId,
                    SparePartId = x.SparePartId,
                    PartNumber = x.SparePart.PartNumber,
                    PartName = x.SparePart.PartName,
                    MovementType = x.MovementType.ToString(),
                    Quantity = x.Quantity,
                    UnitCost = x.UnitCost,
                    Date = x.Date,
                    MaintenanceRecordId = x.MaintenanceRecordId,
                    ReferenceNumber = x.ReferenceNumber,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        // Get Movement By Id
        public async Task<SparePartMovementDto?> GetByIdAsync(int id)
        {
            return await _context.SparePartMovements
                .Where(x => x.SparePartMovementId == id)
                .Select(x => new SparePartMovementDto
                {
                    SparePartMovementId = x.SparePartMovementId,
                    SparePartId = x.SparePartId,
                    PartNumber = x.SparePart.PartNumber,
                    PartName = x.SparePart.PartName,
                    MovementType = x.MovementType.ToString(),
                    Quantity = x.Quantity,
                    UnitCost = x.UnitCost,
                    Date = x.Date,
                    MaintenanceRecordId = x.MaintenanceRecordId,
                    ReferenceNumber = x.ReferenceNumber,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();
        }

        // Get Current Stock
        public async Task<decimal> GetCurrentStockAsync(int sparePartId)
        {
            var movements = await _context.SparePartMovements
                .Where(x => x.SparePartId == sparePartId)
                .ToListAsync();

            decimal stock = 0;

            foreach (var movement in movements)
            {
                switch (movement.MovementType)
                {
                    case InventoryMovementType.Receipt:
                    case InventoryMovementType.Return:
                    case InventoryMovementType.Adjustment:
                        stock += movement.Quantity;
                        break;

                    case InventoryMovementType.Issue:
                        stock -= movement.Quantity;
                        break;
                }
            }

            return stock;
        }

        // Create Movement
        public async Task<SparePartMovementDto> CreateAsync(CreateSparePartMovementDto dto)
        {
            if (dto.MaintenanceRecordId.HasValue)
                throw new ArgumentException("Maintenance-linked movements must be created through maintenance records.");

            if (dto.Quantity <= 0)
                throw new Exception("Quantity must be greater than zero.");

            if (dto.UnitCost < 0)
                throw new Exception("Unit cost cannot be negative.");

            await using var transaction = await _context.Database
                .BeginTransactionAsync(IsolationLevel.Serializable);

            await DatabaseApplicationLock.AcquireAsync(
                _context, $"FleetWise:Inventory:SparePart:{dto.SparePartId}");

            var sparePart = await _context.SpareParts
                .FirstOrDefaultAsync(x => x.SparePartId == dto.SparePartId);

            if (sparePart == null)
                throw new Exception("Spare part not found.");

            if (dto.MovementType == InventoryMovementType.Issue)
            {
                var currentStock = await GetCurrentStockAsync(dto.SparePartId);

                if (dto.Quantity > currentStock)
                    throw new Exception("Insufficient stock.");
            }

            var movement = new SparePartMovement
            {
                SparePartId = dto.SparePartId,
                MovementType = dto.MovementType,
                Quantity = dto.Quantity,
                UnitCost = dto.UnitCost,
                Date = dto.Date,
                MaintenanceRecordId = dto.MaintenanceRecordId,
                ReferenceNumber = dto.ReferenceNumber,
                Notes = dto.Notes
            };

            _context.SparePartMovements.Add(movement);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new SparePartMovementDto
            {
                SparePartMovementId = movement.SparePartMovementId,
                SparePartId = sparePart.SparePartId,
                PartNumber = sparePart.PartNumber,
                PartName = sparePart.PartName,
                MovementType = movement.MovementType.ToString(),
                Quantity = movement.Quantity,
                UnitCost = movement.UnitCost,
                Date = movement.Date,
                MaintenanceRecordId = movement.MaintenanceRecordId,
                ReferenceNumber = movement.ReferenceNumber,
                Notes = movement.Notes
            };
        }
    }
}
