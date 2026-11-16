
using FleetWise.BLL.DTOs.Drivers;
using FleetWise.BLL.Interface;
using FleetWise.DAL.Data;
using FleetWise.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace FleetWise.BLL.Service
{
    public class DriverService : IDriver
    {
        private readonly FleetWiseDbContext _context;

        public DriverService(FleetWiseDbContext context)
        {
            _context = context;
        }

        public async Task<DriverDto> CreateAsync(CreateDriverDto dto)
        {
            if (await IsLicenseNumberExistsAsync(dto.LicenseNumber))
            {
                throw new InvalidOperationException(
                    "License number already exists.");
            }

            if (dto.LicenseExpiryDate < DateOnly.FromDateTime(DateTime.Today))
            {
                throw new InvalidOperationException(
                    "The driver's license has expired.");
            }

            var driver = new Driver
            {
                FullName = dto.FullName,
                LicenseExpiryDate = dto.LicenseExpiryDate,
                LicenseNumber = dto.LicenseNumber,
                Phone = dto.Phone,
                Status = (DriverStatus)dto.Status,
                IsDeleted = false
            };

            _context.Drivers.Add(driver);

            await _context.SaveChangesAsync();

            return await GetByIdAsync(driver.DriverId);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var driver = await _context.Drivers
                .FirstOrDefaultAsync(d =>
                    d.DriverId == id &&
                    !d.IsDeleted);

            if (driver == null)
                return false;

            if (driver.Status == DriverStatus.Active)
            {
                throw new InvalidOperationException(
                    "The driver is active and cannot be deleted.");
            }

            driver.IsDeleted = true;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<DriverDto>> GetAllAsync()
        {
            return await _context.Drivers
                .Where(d => !d.IsDeleted)
                .Select(driver => new DriverDto
                {
                    DriverId = driver.DriverId,
                    FullName = driver.FullName,
                    LicenseExpiryDate = driver.LicenseExpiryDate,
                    LicenseNumber = driver.LicenseNumber,
                    Phone = driver.Phone,
                    Status = (int)driver.Status,
                    IsDeleted = driver.IsDeleted
                })
                .ToListAsync();
        }

        public async Task<DriverDto> GetByIdAsync(int id)
        {
            var driver = await _context.Drivers
                .FirstOrDefaultAsync(d =>
                    d.DriverId == id &&
                    !d.IsDeleted);

            if (driver == null)
                return null;

            return new DriverDto
            {
                DriverId = driver.DriverId,
                FullName = driver.FullName,
                LicenseExpiryDate = driver.LicenseExpiryDate,
                LicenseNumber = driver.LicenseNumber,
                Phone = driver.Phone,
                Status = (int)driver.Status,
                IsDeleted = driver.IsDeleted
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateDriverDto dto)
        {
            var driver = await _context.Drivers
                .FirstOrDefaultAsync(d =>
                    d.DriverId == id &&
                    !d.IsDeleted);

            if (driver == null)
                return false;

            if (await IsLicenseNumberExistsAsync(dto.LicenseNumber, id))
            {
                throw new InvalidOperationException(
                    "License number already exists.");
            }

            if (dto.LicenseExpiryDate < DateOnly.FromDateTime(DateTime.Today))
            {
                throw new InvalidOperationException(
                    "The driver's license has expired.");
            }

            driver.FullName = dto.FullName;
            driver.LicenseExpiryDate = dto.LicenseExpiryDate;
            driver.LicenseNumber = dto.LicenseNumber;
            driver.Phone = dto.Phone;
            driver.Status = (DriverStatus)dto.Status;

            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<bool> IsLicenseNumberExistsAsync(
            string licenseNumber,
            int? driverId = null)
        {
            var query = _context.Drivers
                .Where(d =>
                    d.LicenseNumber == licenseNumber &&
                    !d.IsDeleted);

            if (driverId.HasValue)
            {
                query = query.Where(d =>
                    d.DriverId != driverId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
