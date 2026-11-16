using FleetWise.BLL.DTOs.Containers.ShippingLines;
using FleetWise.BLL.Interface.Containers;
using FleetWise.DAL.Data;
using FleetWise.DAL.Entities.Containers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.Service.Containers
{
    public class ShippingLineService : IShippingLine
    {
        private readonly FleetWiseDbContext _context;

        public ShippingLineService(FleetWiseDbContext context)
        {
            _context = context;
        }

        // Add new shipping line
        public async Task<ShippingLineResponseDto> CreateAsync(CreateShippingLineDto dto)
        {
            // Check duplicate name
            bool nameExists = await _context.ShippingLines
                .AnyAsync(s => s.Name == dto.Name && !s.IsDeleted);

            if (nameExists)
            {
                throw new Exception("Shipping line name already exists.");
            }

            // Check duplicate code
            bool codeExists = await _context.ShippingLines
                .AnyAsync(s => s.Code == dto.Code && !s.IsDeleted);

            if (codeExists)
            {
                throw new Exception("Shipping line code already exists.");
            }

            var shippingLine = new ShippingLine
            {
                Name = dto.Name,
                Code = dto.Code,
            };

            _context.ShippingLines.Add(shippingLine);

            await _context.SaveChangesAsync();

            return new ShippingLineResponseDto
            {
                ShippingLineId = shippingLine.ShippingLineId,
                Name = shippingLine.Name,
                Code = shippingLine.Code
            };
        }

        // Get all active shipping lines
        public async Task<List<ShippingLineResponseDto>> GetAllAsync()
        {
            return await _context.ShippingLines
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.Name)
                .Select(s => new ShippingLineResponseDto
                {
                    ShippingLineId = s.ShippingLineId,
                    Name = s.Name,
                    Code = s.Code
                })
                .ToListAsync(); ;
        }

        public async Task<bool> UpdateAsync(int id, UpdateShippingLineDto dto)
        {
            var line = await _context.ShippingLines.FirstOrDefaultAsync(x => x.ShippingLineId == id && !x.IsDeleted);
            if (line == null) return false;
            string name = dto.Name.Trim();
            string code = dto.Code.Trim();
            if (name.Length == 0 || code.Length == 0)
                throw new InvalidOperationException("Shipping line name and code are required.");
            if (await _context.ShippingLines.AnyAsync(x => x.ShippingLineId != id && !x.IsDeleted && x.Name == name))
                throw new InvalidOperationException("Shipping line name already exists.");
            if (await _context.ShippingLines.AnyAsync(x => x.ShippingLineId != id && !x.IsDeleted && x.Code == code))
                throw new InvalidOperationException("Shipping line code already exists.");
            line.Name = name;
            line.Code = code;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var line = await _context.ShippingLines.FirstOrDefaultAsync(x => x.ShippingLineId == id && !x.IsDeleted);
            if (line == null) return false;
            if (await _context.IncomingAqabaContainers.AnyAsync(x => x.ShippingLineId == id && !x.IsDeleted))
                throw new InvalidOperationException("Shipping line is used by active containers.");
            line.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
