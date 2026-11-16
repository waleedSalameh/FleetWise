
using FleetWise.BLL.DTOs.Vehicles.Trailer;
using FleetWise.BLL.Interface.Vehicles;
using FleetWise.DAL.Data;
using FleetWise.DAL.Entities.Vehicles;
using Microsoft.EntityFrameworkCore;

namespace FleetWise.BLL.Service.Vehicles
{
    public class TrailerService : ITrailer
    {
        private readonly FleetWiseDbContext _context;

        public TrailerService(FleetWiseDbContext context)
        {
            _context = context;
        }

        public async Task<TrailerResponseDto> CreateAsync(CreateTrailerDto dto)
        {
            if (await IsPlateNumberExistsAsync(dto.PlateNumber))
            {
                throw new InvalidOperationException("Plate number already exists.");
            }

            if (dto.AxleCount <= 0)
            {
                throw new ArgumentException("Axle count must be greater than zero.");
            }

            if (dto.MaxLoad <= 0)
            {
                throw new ArgumentException("Maximum load must be greater than zero.");
            }

            if (!Enum.IsDefined(dto.TrailerType))
            {
                throw new ArgumentException("Invalid trailer type.");
            }

            if (dto.LicenseExpiryDate == default)
            {
                throw new ArgumentException("License expiry date is required.");
            }

            var trailer = new Trailer
            {
                PlateNumber = dto.PlateNumber,
                AxleCount = dto.AxleCount,
                MaxLoad = dto.MaxLoad,
                TrailerType = dto.TrailerType,
                LicenseExpiryDate = dto.LicenseExpiryDate,
                IsDeleted = false
            };

            _context.Trailers.Add(trailer);

            await _context.SaveChangesAsync();

            return await GetByIdAsync(trailer.Id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var trailer = await _context.Trailers
                .FirstOrDefaultAsync(t =>
                    t.Id == id &&
                    !t.IsDeleted);

            if (trailer == null)
                return false;

            if (await IsTrailerAssignmentAsync(id))
            {
                throw new InvalidOperationException(
                    "Cannot delete trailer because it has an assignment.");
            }

            trailer.IsDeleted = true;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<TrailerResponseDto>> GetAllAsync()
        {
            return await _context.Trailers
                .Where(t => !t.IsDeleted)
                .Select(trailer => new TrailerResponseDto
                {
                    Id = trailer.Id,
                    PlateNumber = trailer.PlateNumber,
                    AxleCount = trailer.AxleCount,
                    MaxLoad = trailer.MaxLoad,
                    TrailerType = trailer.TrailerType,
                    LicenseExpiryDate = trailer.LicenseExpiryDate
                })
                .ToListAsync();
        }

        public async Task<TrailerResponseDto> GetByIdAsync(int id)
        {
            var trailer = await _context.Trailers
                .FirstOrDefaultAsync(t =>
                    t.Id == id &&
                    !t.IsDeleted);

            if (trailer == null)
                return null;

            return new TrailerResponseDto
            {
                Id = trailer.Id,
                PlateNumber = trailer.PlateNumber,
                AxleCount = trailer.AxleCount,
                MaxLoad = trailer.MaxLoad,
                TrailerType = trailer.TrailerType,
                LicenseExpiryDate = trailer.LicenseExpiryDate
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateTrailerDto dto)
        {
            var trailer = await _context.Trailers
                .FirstOrDefaultAsync(t =>
                    t.Id == id &&
                    !t.IsDeleted);

            if (trailer == null)
                return false;

            if (await IsPlateNumberExistsAsync(dto.PlateNumber, id))
            {
                throw new InvalidOperationException("Plate number already exists.");
            }

            if (dto.AxleCount <= 0)
            {
                throw new ArgumentException("Axle count must be greater than zero.");
            }

            if (dto.MaxLoad <= 0)
            {
                throw new ArgumentException("Maximum load must be greater than zero.");
            }

            if (!Enum.IsDefined(dto.TrailerType))
            {
                throw new ArgumentException("Invalid trailer type.");
            }

            if (dto.LicenseExpiryDate == default)
            {
                throw new ArgumentException("License expiry date is required.");
            }

            trailer.PlateNumber = dto.PlateNumber;
            trailer.AxleCount = dto.AxleCount;
            trailer.MaxLoad = dto.MaxLoad;
            trailer.TrailerType = dto.TrailerType;
            trailer.LicenseExpiryDate = dto.LicenseExpiryDate;

            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<bool> IsPlateNumberExistsAsync(int plateNumber,int? trailerId = null)
        {
            var query = _context.Trailers
                .Where(t => t.PlateNumber == plateNumber);

            if (trailerId.HasValue)
            {
                query = query.Where(t => t.Id != trailerId.Value);
            }

            return await query.AnyAsync();
        }

        private async Task<bool> IsTrailerAssignmentAsync(int id)
        {
            return await _context.VehicleAssignments
                .AnyAsync(a =>
                    a.TrailerId == id &&
                    !a.IsDeleted);
        }
    }
}
