using FleetWise.BLL.DTOs.Containers.DeliveryLocations;
using FleetWise.BLL.Interface.Containers;
using FleetWise.DAL.Data;
using FleetWise.DAL.Entities.Containers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.Service.Containers
{
    public class DeliveryLocationService : IDeliveryLocation
    {
        private readonly FleetWiseDbContext _context;

        public DeliveryLocationService(FleetWiseDbContext context)
        {
            _context = context;
        }

        // Add new delivery location
        public async Task<DeliveryLocationResponseDto> CreateAsync(
            CreateDeliveryLocationDto dto)
        {
            string name = dto.Name.Trim();

            bool nameExists = await _context.DeliveryLocations
                .AnyAsync(d => d.Name == name && !d.IsDeleted);

            if (nameExists)
                throw new Exception("Delivery location name already exists.");

            var deliveryLocation = new DeliveryLocation
            {
                Name = name
            };

            _context.DeliveryLocations.Add(deliveryLocation);

            await _context.SaveChangesAsync();

            return new DeliveryLocationResponseDto
            {
                DeliveryLocationId = deliveryLocation.DeliveryLocationId,
                Name = deliveryLocation.Name
            };
        }

        // Get all active delivery locations
        public async Task<List<DeliveryLocationResponseDto>> GetAllAsync()
        {
            return await _context.DeliveryLocations
                .Where(d => !d.IsDeleted)
                .OrderBy(d => d.Name)
                .Select(d => new DeliveryLocationResponseDto
                {
                    DeliveryLocationId = d.DeliveryLocationId,
                    Name = d.Name
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateAsync(int id, UpdateDeliveryLocationDto dto)
        {
            var location = await _context.DeliveryLocations.FirstOrDefaultAsync(x => x.DeliveryLocationId == id && !x.IsDeleted);
            if (location == null) return false;
            string name = dto.Name.Trim();
            if (name.Length == 0)
                throw new InvalidOperationException("Delivery location name is required.");
            if (await _context.DeliveryLocations.AnyAsync(x => x.DeliveryLocationId != id && !x.IsDeleted && x.Name == name))
                throw new InvalidOperationException("Delivery location name already exists.");
            location.Name = name;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var location = await _context.DeliveryLocations.FirstOrDefaultAsync(x => x.DeliveryLocationId == id && !x.IsDeleted);
            if (location == null) return false;
            if (await _context.IncomingAqabaContainers.AnyAsync(x => x.DeliveryLocationId == id && !x.IsDeleted) ||
                await _context.ExportAqabaContainers.AnyAsync(x => !x.IsDeleted &&
                    (x.LoadingLocationId == id || x.DeliveryLocationId == id)))
                throw new InvalidOperationException("Delivery location is used by active containers.");
            location.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
