using FleetWise.BLL.DTOs.Containers.ExportAqaba;
using FleetWise.BLL.Interface.Containers.ExportAqaba;
using FleetWise.DAL.Data;
using FleetWise.DAL.Entities.Containers.ExportAqaba;
using FleetWise.DAL.Entities.Customers;
using Microsoft.EntityFrameworkCore;

namespace FleetWise.BLL.Service.Containers.ExportAqaba
{
    public class ExportAqabaBookingService : IExportAqabaBooking
    {
        private readonly FleetWiseDbContext _context;

        public ExportAqabaBookingService(FleetWiseDbContext context)
        {
            _context = context;
        }

        public async Task<List<ExportAqabaBookingResponseDto>> GetAllAsync()
        {
            var bookings = await _context.ExportAqabaBookings
                .AsNoTracking()
                .Where(booking => !booking.IsDeleted)
                .Include(booking => booking.Customer)
                .Include(booking => booking.Containers.Where(container => !container.IsDeleted))
                    .ThenInclude(container => container.LoadingLocation)
                .Include(booking => booking.Containers.Where(container => !container.IsDeleted))
                    .ThenInclude(container => container.DeliveryLocation)
                .OrderByDescending(booking => booking.CreatedAt)
                .ToListAsync();

            return bookings.Select(booking => new ExportAqabaBookingResponseDto
            {
                Id = booking.Id,
                BookingNumber = booking.BookingNumber,
                CustomerId = booking.CustomerId,
                CustomerName = booking.Customer.CustomerName,
                CustomerPhoneNumber = booking.Customer.PhoneNumber,
                ContainerCount = booking.Containers.Count(container => !container.IsDeleted),
                Status = booking.Status,
                CreatedAt = booking.CreatedAt,
                Containers = booking.Containers
                    .Where(container => !container.IsDeleted)
                    .Select(container => new ExportAqabaContainerResponseDto
                    {
                        Id = container.Id,
                        BookingId = container.BookingId,
                        ContainerNumber = container.ContainerNumber,
                        ContainerSize = container.ContainerSize,
                        ContainerType = container.ContainerType,
                        LoadingLocationId = container.LoadingLocationId,
                        LoadingLocationName = container.LoadingLocation.Name,
                        RouteStatus = container.RouteStatus,
                        WithdrawalDate = container.WithdrawalDate,
                        LoadingDate = container.LoadingDate,
                        DeliveryLocationId = container.DeliveryLocationId,
                        DeliveryLocationName = container.DeliveryLocation.Name,
                        WeightInTons = container.WeightInTons,
                        Notes = container.Notes
                    }).ToList()
            }).ToList();
        }

        public async Task<ExportAqabaBookingResponseDto?> GetByIdAsync(int id)
        {
            var booking = await _context.ExportAqabaBookings
                .AsNoTracking()
                .Where(booking => booking.Id == id && !booking.IsDeleted)
                .Include(booking => booking.Customer)
                .Include(booking => booking.Containers.Where(container => !container.IsDeleted))
                    .ThenInclude(container => container.LoadingLocation)
                .Include(booking => booking.Containers.Where(container => !container.IsDeleted))
                    .ThenInclude(container => container.DeliveryLocation)
                .FirstOrDefaultAsync();

            if (booking == null)
                return null;

            return new ExportAqabaBookingResponseDto
            {
                Id = booking.Id,
                BookingNumber = booking.BookingNumber,
                CustomerId = booking.CustomerId,
                CustomerName = booking.Customer.CustomerName,
                CustomerPhoneNumber = booking.Customer.PhoneNumber,
                ContainerCount = booking.Containers.Count(container => !container.IsDeleted),
                Status = booking.Status,
                CreatedAt = booking.CreatedAt,
                Containers = booking.Containers
                    .Where(container => !container.IsDeleted)
                    .Select(container => new ExportAqabaContainerResponseDto
                    {
                        Id = container.Id,
                        BookingId = container.BookingId,
                        ContainerNumber = container.ContainerNumber,
                        ContainerSize = container.ContainerSize,
                        ContainerType = container.ContainerType,
                        LoadingLocationId = container.LoadingLocationId,
                        LoadingLocationName = container.LoadingLocation.Name,
                        RouteStatus = container.RouteStatus,
                        WithdrawalDate = container.WithdrawalDate,
                        LoadingDate = container.LoadingDate,
                        DeliveryLocationId = container.DeliveryLocationId,
                        DeliveryLocationName = container.DeliveryLocation.Name,
                        WeightInTons = container.WeightInTons,
                        Notes = container.Notes
                    }).ToList()
            };
        }

        public async Task<ExportAqabaBookingResponseDto> CreateAsync(CreateExportAqabaBookingDto dto)
        {
            string bookingNumber = dto.BookingNumber.Trim();

            bool bookingExists = await _context.ExportAqabaBookings
                .AnyAsync(booking => booking.BookingNumber == bookingNumber && !booking.IsDeleted);

            if (bookingExists)
                throw new InvalidOperationException("Booking number already exists.");

            var customer = await _context.Set<Customer>()
                .FirstOrDefaultAsync(customer => customer.Id == dto.CustomerId && !customer.IsDeleted);

            if (customer == null)
                throw new InvalidOperationException("An active customer with the specified ID was not found.");

            if (dto.Containers == null || !dto.Containers.Any())
                throw new InvalidOperationException("At least one container is required.");

            var containerNumbers = dto.Containers
                .Select(container => container.ContainerNumber.Trim())
                .ToList();

            if (containerNumbers.Distinct(StringComparer.OrdinalIgnoreCase).Count() != containerNumbers.Count)
                throw new InvalidOperationException("Duplicate container numbers in the booking.");

            foreach (var containerNumber in containerNumbers)
            {
                bool containerExists = await _context.ExportAqabaContainers
                    .AnyAsync(container => container.ContainerNumber == containerNumber && !container.IsDeleted);

                if (containerExists)
                    throw new InvalidOperationException($"Container number {containerNumber} already exists.");
            }

            foreach (var container in dto.Containers)
            {
                bool loadingLocationExists = await _context.DeliveryLocations
                    .AnyAsync(location => location.DeliveryLocationId == container.LoadingLocationId && !location.IsDeleted);

                if (!loadingLocationExists)
                    throw new InvalidOperationException($"Loading location with ID {container.LoadingLocationId} was not found.");

                bool deliveryLocationExists = await _context.DeliveryLocations
                    .AnyAsync(location => location.DeliveryLocationId == container.DeliveryLocationId && !location.IsDeleted);

                if (!deliveryLocationExists)
                    throw new InvalidOperationException($"Delivery location with ID {container.DeliveryLocationId} was not found.");
            }

            var booking = new ExportAqabaBooking
            {
                BookingNumber = bookingNumber,
                CustomerId = customer.Id,
                Customer = customer,
                CustomerName = customer.CustomerName,
                Status = dto.Status,
                ContainerCount = dto.Containers.Count,
                Containers = dto.Containers.Select(container => new ExportAqabaContainer
                {
                    ContainerNumber = container.ContainerNumber.Trim(),
                    ContainerSize = container.ContainerSize,
                    ContainerType = container.ContainerType,
                    LoadingLocationId = container.LoadingLocationId,
                    RouteStatus = container.RouteStatus,
                    WithdrawalDate = container.WithdrawalDate,
                    LoadingDate = container.LoadingDate,
                    DeliveryLocationId = container.DeliveryLocationId,
                    WeightInTons = container.WeightInTons,
                    Notes = string.IsNullOrWhiteSpace(container.Notes) ? null : container.Notes.Trim()
                }).ToList()
            };

            _context.ExportAqabaBookings.Add(booking);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(booking.Id)
                ?? throw new InvalidOperationException("Failed to retrieve the created booking.");
        }

        public async Task<bool> UpdateAsync(int id, UpdateExportAqabaBookingDto dto)
        {
            var booking = await _context.ExportAqabaBookings
                .FirstOrDefaultAsync(booking => booking.Id == id && !booking.IsDeleted);

            if (booking == null)
                return false;

            var customer = await _context.Set<Customer>()
                .FirstOrDefaultAsync(customer => customer.Id == dto.CustomerId && !customer.IsDeleted);

            if (customer == null)
                throw new InvalidOperationException("An active customer with the specified ID was not found.");

            string bookingNumber = dto.BookingNumber.Trim();

            bool bookingNumberExists = await _context.ExportAqabaBookings
                .AnyAsync(existingBooking =>
                    existingBooking.Id != id &&
                    existingBooking.BookingNumber == bookingNumber &&
                    !existingBooking.IsDeleted);

            if (bookingNumberExists)
                throw new InvalidOperationException("Booking number already exists.");

            booking.BookingNumber = bookingNumber;
            booking.CustomerId = customer.Id;
            booking.Customer = customer;
            booking.CustomerName = customer.CustomerName;
            booking.Status = dto.Status;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var booking = await _context.ExportAqabaBookings
                .Include(booking => booking.Containers)
                .FirstOrDefaultAsync(booking => booking.Id == id && !booking.IsDeleted);

            if (booking == null)
                return false;

            booking.IsDeleted = true;

            foreach (var container in booking.Containers)
                container.IsDeleted = true;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
