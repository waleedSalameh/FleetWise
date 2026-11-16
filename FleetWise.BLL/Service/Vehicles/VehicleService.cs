using FleetWise.BLL.DTOs.Vehicles;
using FleetWise.BLL.Interface.Vehicles;
using FleetWise.DAL.Data;
using FleetWise.DAL.Entities.Vehicles;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.Service.Vehicles
{
    public class VehicleService : IVehicle
    {
        private readonly FleetWiseDbContext _context;
        public VehicleService (FleetWiseDbContext context)
        {
            _context = context;
        }

        public async Task<VehicleDto> CreateAsync(CreateVehicleDto dto)
        {
            if (IsPlateNumberExists(dto.PlateNumber))
            {
                throw new Exception("Plate number already exists.");
            }
            if (dto.Year > DateTime.Today.Year)
            {
                throw new Exception("Registration year cannot be later than the current year.");
            }
            var vehicle = new Vehicle
            {
                PlateNumber = dto.PlateNumber,
                Make = dto.Make,
                Model = dto.Model,
                Year = dto.Year,
                CurrentOdometer = dto.CurrentOdometer,
                FuelTankCapacity = dto.FuelTankCapacity,
                Status = (VehicleStatus)dto.Status
            };
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(vehicle.VehicleId);

        }

        public async Task<bool> DeleteAsync(int id)
        {
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.VehicleId == id && !v.IsDeleted);

            if (vehicle == null)
                return false;

            var hasAssignments = await _context.VehicleAssignments.AnyAsync(a => a.VehicleId == id);
            var hasMaintenance = await _context.MaintenanceRecords.AnyAsync(m => m.VehicleId == id);
            var hasFuelRecords = await _context.FuelRecords.AnyAsync(f => f.VehicleId == id);
            if (hasAssignments || hasMaintenance || hasFuelRecords)
                throw new InvalidOperationException(
                    "Cannot delete a vehicle while it has assignments, maintenance, or fuel records.");

            vehicle.IsDeleted = true;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<VehicleDto>> GetAllAsync()
        {
            return await _context.Vehicles
                .Where(vehicle => !vehicle.IsDeleted)
                .Select(vehicle => new VehicleDto
                {
                    VehicleId = vehicle.VehicleId,
                    PlateNumber = vehicle.PlateNumber,
                    Make = vehicle.Make,
                    Model = vehicle.Model,
                    Year = vehicle.Year,
                    CurrentOdometer = vehicle.CurrentOdometer,
                    FuelTankCapacity = vehicle.FuelTankCapacity,
                    Status = (int)vehicle.Status,
                    IsDeleted = vehicle.IsDeleted
                })
                .ToListAsync();
        }

        public async Task<VehicleDto> GetByIdAsync(int id)
        {
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.VehicleId == id && !v.IsDeleted);

            if (vehicle == null)
                return null!;

            return new VehicleDto
            {
                VehicleId = vehicle.VehicleId,
                PlateNumber = vehicle.PlateNumber,
                Make = vehicle.Make,
                Model = vehicle.Model,
                Year = vehicle.Year,
                CurrentOdometer = vehicle.CurrentOdometer,
                FuelTankCapacity = vehicle.FuelTankCapacity,
                Status = (int)vehicle.Status,
                IsDeleted = vehicle.IsDeleted
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateVehicleDto dto)
        {
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.VehicleId == id && !v.IsDeleted);
            if (IsPlateNumberExists(dto.PlateNumber, id))
            {
                throw new Exception("Plate number already exists.");
            }

            if (vehicle == null)
                return false;

            vehicle.PlateNumber = dto.PlateNumber;
            vehicle.Make = dto.Make;
            vehicle.Model = dto.Model;
            vehicle.Year = dto.Year;
            vehicle.CurrentOdometer = dto.CurrentOdometer;
            vehicle.FuelTankCapacity = dto.FuelTankCapacity;
            vehicle.Status = (VehicleStatus)dto.Status;

            await _context.SaveChangesAsync();

            return true;
        }
        private bool IsPlateNumberExists(int plateNumber, int? vehicleId = null)
        {
            var query = _context.Vehicles
                .Where(v => v.PlateNumber == plateNumber && !v.IsDeleted);

            if (vehicleId.HasValue)
            {
                query = query.Where(v => v.VehicleId != vehicleId.Value);
            }

            return query.Any();
        }



    }
}
