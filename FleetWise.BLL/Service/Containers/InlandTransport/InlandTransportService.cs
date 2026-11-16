using FleetWise.BLL.DTOs.Containers.InlandTransport;
using FleetWise.BLL.Interface.Containers.InlandTransport;
using FleetWise.DAL.Data;
using FleetWise.DAL.Enums;
using Microsoft.EntityFrameworkCore;

namespace FleetWise.BLL.Service.Containers.InlandTransport
{
    public class InlandTransportService : IInlandTransport
    {
        private readonly FleetWiseDbContext _context;

        public InlandTransportService(FleetWiseDbContext context)
        {
            _context = context;
        }

        public async Task<List<InlandTransportResponseDto>> GetAllAsync()
        {
            return await _context.InlandTransports
                .Where(x => !x.IsDeleted)
                .Include(x => x.Customer)
                .Select(x => new InlandTransportResponseDto
                {
                    Id = x.Id,
                    TripCode = x.TripCode,
                    CustomerId = x.CustomerId,
                    CustomerName = x.Customer.CustomerName,
                    TripStatus = x.TripStatus,
                    CargoType = x.CargoType,
                    Weight = x.Weight,
                    LoadingLocation = x.LoadingLocation,
                    UnloadingLocation = x.UnloadingLocation,
                    Notes = x.Notes,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<InlandTransportResponseDto?> GetByIdAsync(int id)
        {
            return await _context.InlandTransports
                .Where(x => x.Id == id && !x.IsDeleted)
                .Include(x => x.Customer)
                .Select(x => new InlandTransportResponseDto
                {
                    Id = x.Id,
                    TripCode = x.TripCode,
                    CustomerId = x.CustomerId,
                    CustomerName = x.Customer.CustomerName,
                    TripStatus = x.TripStatus,
                    CargoType = x.CargoType,
                    Weight = x.Weight,
                    LoadingLocation = x.LoadingLocation,
                    UnloadingLocation = x.UnloadingLocation,
                    Notes = x.Notes,
                    CreatedAt = x.CreatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<InlandTransportResponseDto> CreateAsync(CreateInlandTransportDto dto)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == dto.CustomerId && !x.IsDeleted);

            if (customer == null)
                throw new Exception("Customer not found or inactive.");

            if (dto.Weight <= 0)
                throw new Exception("Weight must be greater than zero.");

            if (!Enum.IsDefined(typeof(InlandTripStatus), dto.TripStatus))
                throw new Exception("Invalid trip status.");

            if (!Enum.IsDefined(typeof(InternationalCargoType), dto.CargoType))
                throw new Exception("Invalid cargo type.");

            if (string.IsNullOrWhiteSpace(dto.LoadingLocation))
                throw new Exception("Loading location is required.");

            if (string.IsNullOrWhiteSpace(dto.UnloadingLocation))
                throw new Exception("Unloading location is required.");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var transport = new DAL.Entities.Containers.InlandTransport.InlandTransport
            {
                TripCode = $"TEMP-{Guid.NewGuid():N}",
                CustomerId = dto.CustomerId,
                TripStatus = dto.TripStatus,
                CargoType = dto.CargoType,
                Weight = dto.Weight,
                LoadingLocation = dto.LoadingLocation.Trim(),
                UnloadingLocation = dto.UnloadingLocation.Trim(),
                Notes = dto.Notes?.Trim(),
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.InlandTransports.Add(transport);

            await _context.SaveChangesAsync();

            transport.TripCode = $"IT-{transport.Id:D6}";

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return new InlandTransportResponseDto
            {
                Id = transport.Id,
                TripCode = transport.TripCode,
                CustomerId = transport.CustomerId,
                CustomerName = customer.CustomerName,
                TripStatus = transport.TripStatus,
                CargoType = transport.CargoType,
                Weight = transport.Weight,
                LoadingLocation = transport.LoadingLocation,
                UnloadingLocation = transport.UnloadingLocation,
                Notes = transport.Notes,
                CreatedAt = transport.CreatedAt
            };
        }

        public async Task UpdateAsync(int id, UpdateInlandTransportDto dto)
        {
            var transport = await _context.InlandTransports
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (transport == null)
                throw new Exception("Inland transport not found.");

            var customer = await _context.Customers
                .FirstOrDefaultAsync(x => x.Id == dto.CustomerId && !x.IsDeleted);

            if (customer == null)
                throw new Exception("Customer not found or inactive.");

            if (dto.Weight <= 0)
                throw new Exception("Weight must be greater than zero.");

            if (!Enum.IsDefined(typeof(InlandTripStatus), dto.TripStatus))
                throw new Exception("Invalid trip status.");

            if (!Enum.IsDefined(typeof(InternationalCargoType), dto.CargoType))
                throw new Exception("Invalid cargo type.");

            if (string.IsNullOrWhiteSpace(dto.LoadingLocation))
                throw new Exception("Loading location is required.");

            if (string.IsNullOrWhiteSpace(dto.UnloadingLocation))
                throw new Exception("Unloading location is required.");

            transport.CustomerId = dto.CustomerId;
            transport.TripStatus = dto.TripStatus;
            transport.CargoType = dto.CargoType;
            transport.Weight = dto.Weight;
            transport.LoadingLocation = dto.LoadingLocation.Trim();
            transport.UnloadingLocation = dto.UnloadingLocation.Trim();
            transport.Notes = dto.Notes?.Trim();

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var transport = await _context.InlandTransports
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (transport == null)
                throw new Exception("Inland transport not found.");

            transport.IsDeleted = true;

            await _context.SaveChangesAsync();
        }
    }
}