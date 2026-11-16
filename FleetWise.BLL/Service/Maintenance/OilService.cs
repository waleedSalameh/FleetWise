using FleetWise.BLL.DTOs.Maintenance.Oils;
using FleetWise.BLL.Interface;
using FleetWise.DAL.Data;
using FleetWise.DAL.Entities.Maintenance;
using FleetWise.DAL.Entities.Maintenance.FleetWise.DAL.Entities.Maintenance;
using Microsoft.EntityFrameworkCore;

namespace FleetWise.BLL.Services.Maintenance
{
    public class OilService : IOilService
    {
        private readonly FleetWiseDbContext _context;

        public OilService(FleetWiseDbContext context)
        {
            _context = context;
        }

        // Get All Oils
        public async Task<List<OilDto>> GetAllAsync()
        {
            return await _context.Oils
                .Select(x => new OilDto
                {
                    OilId = x.OilId,
                    OilName = x.OilName,
                    Brand = x.Brand,
                    Viscosity = x.Viscosity,
                    Unit = x.Unit,
                    PurchasePrice = x.PurchasePrice,
                    MinimumQuantity = x.MinimumQuantity
                })
                .ToListAsync();
        }

        // Get Oil By Id
        public async Task<OilDto?> GetByIdAsync(int id)
        {
            return await _context.Oils
                .Where(x => x.OilId == id)
                .Select(x => new OilDto
                {
                    OilId = x.OilId,
                    OilName = x.OilName,
                    Brand = x.Brand,
                    Viscosity = x.Viscosity,
                    Unit = x.Unit,
                    PurchasePrice = x.PurchasePrice,
                    MinimumQuantity = x.MinimumQuantity
                })
                .FirstOrDefaultAsync();
        }

        // Create Oil
        public async Task<OilDto> CreateAsync(CreateOilDto dto)
        {
            var oil = new Oil
            {
                OilName = dto.OilName,
                Brand = dto.Brand,
                Viscosity = dto.Viscosity,
                Unit = dto.Unit,
                PurchasePrice = dto.PurchasePrice,
                MinimumQuantity = dto.MinimumQuantity
            };

            _context.Oils.Add(oil);

            await _context.SaveChangesAsync();

            return new OilDto
            {
                OilId = oil.OilId,
                OilName = oil.OilName,
                Brand = oil.Brand,
                Viscosity = oil.Viscosity,
                Unit = oil.Unit,
                PurchasePrice = oil.PurchasePrice,
                MinimumQuantity = oil.MinimumQuantity
            };
        }

        // Update Oil
        public async Task<bool> UpdateAsync(int id, UpdateOilDto dto)
        {
            var oil = await _context.Oils.FindAsync(id);

            if (oil == null)
                return false;

            oil.OilName = dto.OilName;
            oil.Brand = dto.Brand;
            oil.Viscosity = dto.Viscosity;
            oil.Unit = dto.Unit;
            oil.PurchasePrice = dto.PurchasePrice;
            oil.MinimumQuantity = dto.MinimumQuantity;

            await _context.SaveChangesAsync();

            return true;
        }

        // Soft Delete Oil
        public async Task<bool> DeleteAsync(int id)
        {
            var oil = await _context.Oils
                .FirstOrDefaultAsync(x => x.OilId == id);

            if (oil == null)
                return false;

            var hasMovements = await _context.OilStockMovements
                .AnyAsync(x => x.OilId == id);

            if (hasMovements)
                return false;

            oil.IsDeleted = true;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}