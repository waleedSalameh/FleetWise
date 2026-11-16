using FleetWise.BLL.DTOs.Containers.InternationalExport;
using FleetWise.BLL.Interface.Containers.IInternationalExport;
using FleetWise.DAL.Data;
using FleetWise.DAL.Enums;
using Microsoft.EntityFrameworkCore;

namespace FleetWise.BLL.Service.Containers.InternationalExport
{
    public class InternationalExportService : IInternationalExport
    {
        private readonly FleetWiseDbContext _context;

        public InternationalExportService(FleetWiseDbContext context)
        {
            _context = context;
        }

        public async Task<InternationalExportResponseDto> CreateAsync(CreateInternationalExportDto dto)
        {
            if (IsCargoNumberExists(dto.CargoNumber))
                throw new Exception("Cargo number already exists.");

            if (IsCustomsDeclarationNumberExists(dto.CustomsDeclarationNumber))
                throw new Exception("Customs declaration number already exists.");

            if (dto.Weight <= 0)
                throw new Exception("Weight must be greater than zero.");

            if (dto.Distance <= 0)
                throw new Exception("Distance must be greater than zero.");

            if (dto.TripDuration <= 0)
                throw new Exception("Trip duration must be greater than zero.");

            if (!Enum.IsDefined(typeof(InternationalCargoType), dto.CargoType))
                throw new Exception("Cargo type is invalid.");

            if (!Enum.IsDefined(typeof(InternationalOrderStatus), dto.Status))
                throw new Exception("Status is invalid.");

            if (dto.Status == InternationalOrderStatus.Completed && !dto.IsPaid)
                throw new Exception("An export record cannot be completed unless it is paid.");

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == dto.CustomerId && !c.IsDeleted);

            if (customer == null)
                throw new Exception("Customer not found.");

            var export = new FleetWise.DAL.Entities.Containers.InternationalExport.InternationalExport
            {
                CargoNumber = dto.CargoNumber,
                CustomerId = dto.CustomerId,
                Country = dto.Country,
                CargoType = dto.CargoType,
                LoadingLocation = dto.LoadingLocation,
                FactoryName = dto.FactoryName,
                Weight = dto.Weight,
                Notes = dto.Notes,
                CustomsDeclarationNumber = dto.CustomsDeclarationNumber,
                TripDuration = dto.TripDuration,
                UnloadingLocation = dto.UnloadingLocation,
                Distance = dto.Distance,
                Status = dto.Status,
                IsPaid = dto.IsPaid
            };

            _context.InternationalExports.Add(export);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(export.Id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var export = await _context.InternationalExports
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

            if (export == null)
                return false;

            export.IsDeleted = true;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<InternationalExportResponseDto>> GetAllAsync()
        {
            var exports = await _context.InternationalExports
                .Where(e => !e.IsDeleted)
                .Include(e => e.Customer)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            return exports.Select(export => new InternationalExportResponseDto
            {
                Id = export.Id,
                CargoNumber = export.CargoNumber,
                CustomerId = export.CustomerId,
                CustomerName = export.Customer.CustomerName,
                CustomerPhoneNumber = export.Customer.PhoneNumber,
                Country = export.Country,
                CargoType = export.CargoType,
                LoadingLocation = export.LoadingLocation,
                FactoryName = export.FactoryName,
                Weight = export.Weight,
                Notes = export.Notes,
                CustomsDeclarationNumber = export.CustomsDeclarationNumber,
                TripDuration = export.TripDuration,
                UnloadingLocation = export.UnloadingLocation,
                Distance = export.Distance,
                Status = export.Status,
                IsPaid = export.IsPaid,
                CreatedAt = export.CreatedAt
            }).ToList();
        }

        public async Task<InternationalExportResponseDto> GetByIdAsync(int id)
        {
            var export = await _context.InternationalExports
                .Include(e => e.Customer)
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

            if (export == null)
                return null;

            return new InternationalExportResponseDto
            {
                Id = export.Id,
                CargoNumber = export.CargoNumber,
                CustomerId = export.CustomerId,
                CustomerName = export.Customer.CustomerName,
                CustomerPhoneNumber = export.Customer.PhoneNumber,
                Country = export.Country,
                CargoType = export.CargoType,
                LoadingLocation = export.LoadingLocation,
                FactoryName = export.FactoryName,
                Weight = export.Weight,
                Notes = export.Notes,
                CustomsDeclarationNumber = export.CustomsDeclarationNumber,
                TripDuration = export.TripDuration,
                UnloadingLocation = export.UnloadingLocation,
                Distance = export.Distance,
                Status = export.Status,
                IsPaid = export.IsPaid,
                CreatedAt = export.CreatedAt
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateInternationalExportDto dto)
        {
            var export = await _context.InternationalExports
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

            if (export == null)
                return false;

            if (IsCargoNumberExists(dto.CargoNumber, id))
                throw new Exception("Cargo number already exists.");

            if (IsCustomsDeclarationNumberExists(dto.CustomsDeclarationNumber, id))
                throw new Exception("Customs declaration number already exists.");

            if (dto.Weight <= 0)
                throw new Exception("Weight must be greater than zero.");

            if (dto.Distance <= 0)
                throw new Exception("Distance must be greater than zero.");

            if (dto.TripDuration <= 0)
                throw new Exception("Trip duration must be greater than zero.");

            if (!Enum.IsDefined(typeof(InternationalCargoType), dto.CargoType))
                throw new Exception("Cargo type is invalid.");

            if (!Enum.IsDefined(typeof(InternationalOrderStatus), dto.Status))
                throw new Exception("Status is invalid.");

            if (dto.Status == InternationalOrderStatus.Completed && !dto.IsPaid)
                throw new Exception("An export record cannot be completed unless it is paid.");

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == dto.CustomerId && !c.IsDeleted);

            if (customer == null)
                throw new Exception("Customer not found.");

            export.CargoNumber = dto.CargoNumber;
            export.CustomerId = dto.CustomerId;
            export.Country = dto.Country;
            export.CargoType = dto.CargoType;
            export.LoadingLocation = dto.LoadingLocation;
            export.FactoryName = dto.FactoryName;
            export.Weight = dto.Weight;
            export.Notes = dto.Notes;
            export.CustomsDeclarationNumber = dto.CustomsDeclarationNumber;
            export.TripDuration = dto.TripDuration;
            export.UnloadingLocation = dto.UnloadingLocation;
            export.Distance = dto.Distance;
            export.Status = dto.Status;
            export.IsPaid = dto.IsPaid;

            await _context.SaveChangesAsync();

            return true;
        }

        private bool IsCargoNumberExists(string cargoNumber, int? exportId = null)
        {
            var query = _context.InternationalExports
                .Where(e => e.CargoNumber == cargoNumber && !e.IsDeleted);

            if (exportId.HasValue)
                query = query.Where(e => e.Id != exportId.Value);

            return query.Any();
        }

        private bool IsCustomsDeclarationNumberExists(string declarationNumber, int? exportId = null)
        {
            var query = _context.InternationalExports
                .Where(e => e.CustomsDeclarationNumber == declarationNumber && !e.IsDeleted);

            if (exportId.HasValue)
                query = query.Where(e => e.Id != exportId.Value);

            return query.Any();
        }
    }
}
