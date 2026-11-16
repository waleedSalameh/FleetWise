using FleetWise.BLL.DTOs.Vehicles.ExternalVehicles;
using FleetWise.BLL.Interface.Vehicles;
using FleetWise.DAL.Data;
using FleetWise.DAL.Entities.Vehicles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;


namespace FleetWise.BLL.Service.Vehicles
{
    public class ExternalVehiclesService : IExternalVehicles
    {
        private readonly FleetWiseDbContext _context;

        public ExternalVehiclesService(FleetWiseDbContext context)
        {
            _context = context;
        }

        public async Task<List<ExternalVehiclesResponseDto>> GetAllAsync()
        {
            return await _context.ExternalVehicles
                .Where(x => !x.IsDeleted)
                .Select(x => new ExternalVehiclesResponseDto
                {
                    Id = x.Id,
                    DriverName = x.DriverName,
                    PlateNumber = x.PlateNumber,
                    PhoneNumber = x.PhoneNumber
                })
                .ToListAsync();
        }

        public async Task<ExternalVehiclesResponseDto?> GetByIdAsync(int id)
        {
            return await _context.ExternalVehicles
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new ExternalVehiclesResponseDto
                {
                    Id = x.Id,
                    DriverName = x.DriverName,
                    PlateNumber = x.PlateNumber,
                    PhoneNumber = x.PhoneNumber
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ExternalVehiclesResponseDto> CreateAsync(CreateExternalVehiclesDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.DriverName))
                throw new Exception("Driver name is required.");

            if (dto.PlateNumber <= 0)
                throw new Exception("Plate number must be greater than zero.");

            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
                throw new Exception("Phone number is required.");

            var driverName = dto.DriverName.Trim();
            var phoneNumber = dto.PhoneNumber.Trim();

            if (await _context.ExternalVehicles.AnyAsync(x => x.PlateNumber == dto.PlateNumber))
                throw new Exception("Plate number already exists.");

            if (await _context.ExternalVehicles.AnyAsync(x => x.PhoneNumber == phoneNumber))
                throw new Exception("Phone number already exists.");

            var vehicle = new ExternalVehicles
            {
                DriverName = driverName,
                PlateNumber = dto.PlateNumber,
                PhoneNumber = phoneNumber,
                IsDeleted = false
            };

            _context.ExternalVehicles.Add(vehicle);

            await _context.SaveChangesAsync();

            return new ExternalVehiclesResponseDto
            {
                Id = vehicle.Id,
                DriverName = vehicle.DriverName,
                PlateNumber = vehicle.PlateNumber,
                PhoneNumber = vehicle.PhoneNumber
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var vehicle = await _context.ExternalVehicles
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (vehicle == null)
                return false;

            vehicle.IsDeleted = true;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
