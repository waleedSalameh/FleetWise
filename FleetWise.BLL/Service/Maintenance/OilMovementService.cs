using FleetWise.BLL.DTOs.Maintenance.OilMovements;
using FleetWise.BLL.Interface;
using FleetWise.DAL.Data;
using FleetWise.DAL.Entities.Maintenance;
using FleetWise.DAL.Enums;
using FleetWise.BLL.Service.Concurrency;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FleetWise.BLL.Services.Maintenance
{
    public class OilMovementService : IOilMovementService
    {
        private readonly FleetWiseDbContext _context;

        public OilMovementService(FleetWiseDbContext context)
        {
            _context = context;
        }

        // Get All Movements
        public async Task<List<OilMovementDto>> GetAllAsync()
        {
            return await _context.OilStockMovements
                .Select(x => new OilMovementDto
                {
                    OilStockMovementId = x.OilStockMovementId,
                    OilId = x.OilId,
                    OilName = x.Oil.OilName,
                    MovementType = x.MovementType.ToString(),
                    Quantity = x.Quantity,
                    UnitCost = x.UnitCost,
                    Date = x.Date,
                    MaintenanceRecordId = x.MaintenanceRecordId,
                    ReferenceNumber = x.ReferenceNumber,
                    Notes = x.Notes
                })
                .OrderByDescending(x => x.Date)
                .ToListAsync();
        }

        // Get Movements By Oil Id
        public async Task<List<OilMovementDto>> GetByOilIdAsync(int oilId)
        {
            return await _context.OilStockMovements
                .Where(x => x.OilId == oilId)
                .Select(x => new OilMovementDto
                {
                    OilStockMovementId = x.OilStockMovementId,
                    OilId = x.OilId,
                    OilName = x.Oil.OilName,
                    MovementType = x.MovementType.ToString(),
                    Quantity = x.Quantity,
                    UnitCost = x.UnitCost,
                    Date = x.Date,
                    MaintenanceRecordId = x.MaintenanceRecordId,
                    ReferenceNumber = x.ReferenceNumber,
                    Notes = x.Notes
                })
                .OrderByDescending(x => x.Date)
                .ToListAsync();
        }

        // Get Movement By Id
        public async Task<OilMovementDto?> GetByIdAsync(int id)
        {
            return await _context.OilStockMovements
                .Where(x => x.OilStockMovementId == id)
                .Select(x => new OilMovementDto
                {
                    OilStockMovementId = x.OilStockMovementId,
                    OilId = x.OilId,
                    OilName = x.Oil.OilName,
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
        public async Task<decimal> GetCurrentStockAsync(int oilId)
        {
            var movements = await _context.OilStockMovements
                .Where(x => x.OilId == oilId)
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
        public async Task<OilMovementDto> CreateAsync(CreateOilMovementDto dto)
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
                _context, $"FleetWise:Inventory:Oil:{dto.OilId}");

            var oil = await _context.Oils
                .FirstOrDefaultAsync(x => x.OilId == dto.OilId);

            if (oil == null)
                throw new Exception("Oil not found.");

            var currentStock = await GetCurrentStockAsync(dto.OilId);

            if (dto.MovementType == InventoryMovementType.Issue
                && dto.Quantity > currentStock)
            {
                throw new Exception("Insufficient oil stock.");
            }

            var movement = new OilStockMovement
            {
                OilId = dto.OilId,
                MovementType = dto.MovementType,
                Quantity = dto.Quantity,
                UnitCost = dto.UnitCost,
                Date = dto.Date,
                MaintenanceRecordId = dto.MaintenanceRecordId,
                ReferenceNumber = dto.ReferenceNumber,
                Notes = dto.Notes
            };

            _context.OilStockMovements.Add(movement);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new OilMovementDto
            {
                OilStockMovementId = movement.OilStockMovementId,
                OilId = movement.OilId,
                OilName = oil.OilName,
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
