using FleetWise.BLL.DTOs.Maintenance.SpareParts;
using FleetWise.BLL.Interface;
using FleetWise.DAL.Data;
using FleetWise.DAL.Entities.Maintenance;
using Microsoft.EntityFrameworkCore;

namespace FleetWise.BLL.Services.Maintenance
{
    public class SparePartService : ISparePartService
    {
        private readonly FleetWiseDbContext _context;

        public SparePartService(FleetWiseDbContext context)
        {
            _context = context;
        }

        // Get All Spare Parts
        public async Task<List<SparePartDto>> GetAllAsync()
        {
            return await _context.SpareParts
                .Select(x => new SparePartDto
                {
                    SparePartId = x.SparePartId,
                    PartNumber = x.PartNumber,
                    PartName = x.PartName,
                    Brand = x.Brand,
                    Description = x.Description,
                    Unit = x.Unit,
                    PurchasePrice = x.PurchasePrice,
                    MinimumQuantity = x.MinimumQuantity
                })
                .ToListAsync();
        }

        // Get Spare Part By Id
        public async Task<SparePartDto?> GetByIdAsync(int id)
        {
            return await _context.SpareParts
                .Where(x => x.SparePartId == id)
                .Select(x => new SparePartDto
                {
                    SparePartId = x.SparePartId,
                    PartNumber = x.PartNumber,
                    PartName = x.PartName,
                    Brand = x.Brand,
                    Description = x.Description,
                    Unit = x.Unit,
                    PurchasePrice = x.PurchasePrice,
                    MinimumQuantity = x.MinimumQuantity
                })
                .FirstOrDefaultAsync();
        }

        // Create Spare Part
        public async Task<SparePartDto> CreateAsync(CreateSparePartDto dto)
        {
            var exists = await _context.SpareParts
                .AnyAsync(x => x.PartNumber == dto.PartNumber);

            if (exists)
                throw new Exception("Part number already exists.");

            var sparePart = new SparePart
            {
                PartNumber = dto.PartNumber,
                PartName = dto.PartName,
                Brand = dto.Brand,
                Description = dto.Description,
                Unit = dto.Unit,
                PurchasePrice = dto.PurchasePrice,
                MinimumQuantity = dto.MinimumQuantity
            };

            _context.SpareParts.Add(sparePart);

            await _context.SaveChangesAsync();

            return new SparePartDto
            {
                SparePartId = sparePart.SparePartId,
                PartNumber = sparePart.PartNumber,
                PartName = sparePart.PartName,
                Brand = sparePart.Brand,
                Description = sparePart.Description,
                Unit = sparePart.Unit,
                PurchasePrice = sparePart.PurchasePrice,
                MinimumQuantity = sparePart.MinimumQuantity
            };
        }

        // Update Spare Part
        public async Task<bool> UpdateAsync(int id, UpdateSparePartDto dto)
        {
            var sparePart = await _context.SpareParts.FindAsync(id);

            if (sparePart == null)
                return false;

            var exists = await _context.SpareParts
                .AnyAsync(x => x.PartNumber == dto.PartNumber
                            && x.SparePartId != id);

            if (exists)
                throw new Exception("Part number already exists.");

            sparePart.PartNumber = dto.PartNumber;
            sparePart.PartName = dto.PartName;
            sparePart.Brand = dto.Brand;
            sparePart.Description = dto.Description;
            sparePart.Unit = dto.Unit;
            sparePart.PurchasePrice = dto.PurchasePrice;
            sparePart.MinimumQuantity = dto.MinimumQuantity;

            await _context.SaveChangesAsync();

            return true;
        }

        // Soft Delete Spare Part
        public async Task<bool> DeleteAsync(int id)
        {
            var sparePart = await _context.SpareParts
                .FirstOrDefaultAsync(x => x.SparePartId == id);

            if (sparePart == null)
                return false;

            var hasMovements = await _context.SparePartMovements
                .AnyAsync(x => x.SparePartId == id);

            if (hasMovements)
                return false;

            sparePart.IsDeleted = true;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}