using FleetWise.BLL.DTOs.Containers.InternationalIncoming;
using FleetWise.BLL.Interface.Containers.InternationalIncoming;
using FleetWise.DAL.Data;
using FleetWise.DAL.Entities.Containers.InternationalIncoming;
using FleetWise.DAL.Enums;
using Microsoft.EntityFrameworkCore;

namespace FleetWise.BLL.Service.Containers.InternationalIncoming
{
    public class InternationalIncomingService : IInternationalIncoming
    {
        private readonly FleetWiseDbContext _context;

        public InternationalIncomingService(FleetWiseDbContext context)
        {
            _context = context;
        }

        public async Task<InternationalIncomingResponseDto> CreateAsync(CreateInternationalIncomingDto dto)
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
                throw new Exception("An incoming record cannot be completed unless it is paid.");

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == dto.CustomerId && !c.IsDeleted);

            if (customer == null)
                throw new Exception("Customer not found.");

            var incoming = new FleetWise.DAL.Entities.Containers.InternationalIncoming.InternationalIncoming
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

            _context.InternationalIncomings.Add(incoming);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(incoming.Id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var incoming = await _context.InternationalIncomings
                .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);

            if (incoming == null)
                return false;

            incoming.IsDeleted = true;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<InternationalIncomingResponseDto>> GetAllAsync()
        {
            var incomings = await _context.InternationalIncomings
                .Where(i => !i.IsDeleted)
                .Include(i => i.Customer)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            return incomings.Select(incoming => new InternationalIncomingResponseDto
            {
                Id = incoming.Id,
                CargoNumber = incoming.CargoNumber,
                CustomerId = incoming.CustomerId,
                CustomerName = incoming.Customer.CustomerName,
                CustomerPhoneNumber = incoming.Customer.PhoneNumber,
                Country = incoming.Country,
                CargoType = incoming.CargoType,
                LoadingLocation = incoming.LoadingLocation,
                FactoryName = incoming.FactoryName,
                Weight = incoming.Weight,
                Notes = incoming.Notes,
                CustomsDeclarationNumber = incoming.CustomsDeclarationNumber,
                TripDuration = incoming.TripDuration,
                UnloadingLocation = incoming.UnloadingLocation,
                Distance = incoming.Distance,
                Status = incoming.Status,
                IsPaid = incoming.IsPaid,
                CreatedAt = incoming.CreatedAt
            }).ToList();
        }

        public async Task<InternationalIncomingResponseDto> GetByIdAsync(int id)
        {
            var incoming = await _context.InternationalIncomings
                .Include(i => i.Customer)
                .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);

            if (incoming == null)
                return null;

            return new InternationalIncomingResponseDto
            {
                Id = incoming.Id,
                CargoNumber = incoming.CargoNumber,
                CustomerId = incoming.CustomerId,
                CustomerName = incoming.Customer.CustomerName,
                CustomerPhoneNumber = incoming.Customer.PhoneNumber,
                Country = incoming.Country,
                CargoType = incoming.CargoType,
                LoadingLocation = incoming.LoadingLocation,
                FactoryName = incoming.FactoryName,
                Weight = incoming.Weight,
                Notes = incoming.Notes,
                CustomsDeclarationNumber = incoming.CustomsDeclarationNumber,
                TripDuration = incoming.TripDuration,
                UnloadingLocation = incoming.UnloadingLocation,
                Distance = incoming.Distance,
                Status = incoming.Status,
                IsPaid = incoming.IsPaid,
                CreatedAt = incoming.CreatedAt
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateInternationalIncomingDto dto)
        {
            var incoming = await _context.InternationalIncomings
                .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);

            if (incoming == null)
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
                throw new Exception("An incoming record cannot be completed unless it is paid.");

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == dto.CustomerId && !c.IsDeleted);

            if (customer == null)
                throw new Exception("Customer not found.");

            incoming.CargoNumber = dto.CargoNumber;
            incoming.CustomerId = dto.CustomerId;
            incoming.Country = dto.Country;
            incoming.CargoType = dto.CargoType;
            incoming.LoadingLocation = dto.LoadingLocation;
            incoming.FactoryName = dto.FactoryName;
            incoming.Weight = dto.Weight;
            incoming.Notes = dto.Notes;
            incoming.CustomsDeclarationNumber = dto.CustomsDeclarationNumber;
            incoming.TripDuration = dto.TripDuration;
            incoming.UnloadingLocation = dto.UnloadingLocation;
            incoming.Distance = dto.Distance;
            incoming.Status = dto.Status;
            incoming.IsPaid = dto.IsPaid;

            await _context.SaveChangesAsync();

            return true;
        }

        private bool IsCargoNumberExists(string cargoNumber, int? incomingId = null)
        {
            var query = _context.InternationalIncomings
                .Where(i => i.CargoNumber == cargoNumber && !i.IsDeleted);

            if (incomingId.HasValue)
                query = query.Where(i => i.Id != incomingId.Value);

            return query.Any();
        }

        private bool IsCustomsDeclarationNumberExists(string declarationNumber, int? incomingId = null)
        {
            var query = _context.InternationalIncomings
                .Where(i => i.CustomsDeclarationNumber == declarationNumber && !i.IsDeleted);

            if (incomingId.HasValue)
                query = query.Where(i => i.Id != incomingId.Value);

            return query.Any();
        }
    }
}
