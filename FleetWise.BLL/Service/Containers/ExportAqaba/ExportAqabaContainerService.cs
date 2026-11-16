using FleetWise.BLL.DTOs.Containers.ExportAqaba;
using FleetWise.BLL.Interface.Containers.ExportAqaba;
using FleetWise.DAL.Data;
using FleetWise.DAL.Entities.Containers.ExportAqaba;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.Service.Containers.ExportAqaba
{
    public class ExportAqabaContainerService : IExportAqabaContainer
    {
        private readonly FleetWiseDbContext _context;

        public ExportAqabaContainerService(FleetWiseDbContext context)
        {
            _context = context;
        }

        public async Task<ExportAqabaContainerResponseDto?> GetByIdAsync(int id)
        {
            var container = await _context.ExportAqabaContainers
                .AsNoTracking()
                .Include(x => x.LoadingLocation)
                .Include(x => x.DeliveryLocation)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (container == null)
                return null;

            return new ExportAqabaContainerResponseDto
            {
                Id = container.Id,
                BookingId = container.BookingId,
                ContainerNumber = container.ContainerNumber,
                ContainerSize = container.ContainerSize,
                ContainerType = container.ContainerType,
                LoadingLocationId = container.LoadingLocationId,
                LoadingLocationName = container.LoadingLocation?.Name ?? string.Empty,
                RouteStatus = container.RouteStatus,
                WithdrawalDate = container.WithdrawalDate,
                LoadingDate = container.LoadingDate,
                DeliveryLocationId = container.DeliveryLocationId,
                DeliveryLocationName = container.DeliveryLocation?.Name ?? string.Empty,
                WeightInTons = container.WeightInTons,
                Notes = container.Notes
            };
        }

        public async Task<ExportAqabaContainerResponseDto> CreateAsync( int bookingId,CreateExportAqabaContainerDto dto)
        {
            var booking = await _context.ExportAqabaBookings
                .FirstOrDefaultAsync(x => x.Id == bookingId && !x.IsDeleted);

            if (booking == null)
                throw new InvalidOperationException("Booking not found.");

            var containerNumber = dto.ContainerNumber.Trim();

            var exists = await _context.ExportAqabaContainers
                .AnyAsync(x =>
                    x.ContainerNumber == containerNumber &&
                    !x.IsDeleted);

            if (exists)
                throw new InvalidOperationException("Container number already exists.");

            var loadingLocationExists = await _context.DeliveryLocations
                .AnyAsync(x =>
                    x.DeliveryLocationId == dto.LoadingLocationId &&
                    !x.IsDeleted);

            if (!loadingLocationExists)
                throw new InvalidOperationException("Loading location not found.");

            var deliveryLocationExists = await _context.DeliveryLocations
                .AnyAsync(x =>
                    x.DeliveryLocationId == dto.DeliveryLocationId &&
                    !x.IsDeleted);

            if (!deliveryLocationExists)
                throw new InvalidOperationException("Delivery location not found.");

            var container = new ExportAqabaContainer
            {
                BookingId = bookingId,
                ContainerNumber = containerNumber,
                ContainerSize = dto.ContainerSize,
                ContainerType = dto.ContainerType,
                LoadingLocationId = dto.LoadingLocationId,
                RouteStatus = dto.RouteStatus,
                WithdrawalDate = dto.WithdrawalDate,
                LoadingDate = dto.LoadingDate,
                DeliveryLocationId = dto.DeliveryLocationId,
                WeightInTons = dto.WeightInTons,
                Notes = string.IsNullOrWhiteSpace(dto.Notes)
                    ? null
                    : dto.Notes.Trim()
            };

            _context.ExportAqabaContainers.Add(container);

            booking.ContainerCount++;

            await _context.SaveChangesAsync();

            await _context.Entry(container)
                .Reference(x => x.LoadingLocation)
                .LoadAsync();

            await _context.Entry(container)
                .Reference(x => x.DeliveryLocation)
                .LoadAsync();

            return new ExportAqabaContainerResponseDto
            {
                Id = container.Id,
                BookingId = container.BookingId,
                ContainerNumber = container.ContainerNumber,
                ContainerSize = container.ContainerSize,
                ContainerType = container.ContainerType,
                LoadingLocationId = container.LoadingLocationId,
                LoadingLocationName = container.LoadingLocation?.Name ?? string.Empty,
                RouteStatus = container.RouteStatus,
                WithdrawalDate = container.WithdrawalDate,
                LoadingDate = container.LoadingDate,
                DeliveryLocationId = container.DeliveryLocationId,
                DeliveryLocationName = container.DeliveryLocation?.Name ?? string.Empty,
                WeightInTons = container.WeightInTons,
                Notes = container.Notes
            };
        }

        public async Task<bool> UpdateAsync( int id,UpdateExportAqabaContainerDto dto)
        {
            var container = await _context.ExportAqabaContainers
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (container == null)
                return false;

            var containerNumber = dto.ContainerNumber.Trim();

            var exists = await _context.ExportAqabaContainers
                .AnyAsync(x =>
                    x.Id != id &&
                    x.ContainerNumber == containerNumber &&
                    !x.IsDeleted);

            if (exists)
                throw new InvalidOperationException("Container number already exists.");

            var loadingLocationExists = await _context.DeliveryLocations
                .AnyAsync(x =>
                    x.DeliveryLocationId == dto.LoadingLocationId &&
                    !x.IsDeleted);

            if (!loadingLocationExists)
                throw new InvalidOperationException("Loading location not found.");

            var deliveryLocationExists = await _context.DeliveryLocations
                .AnyAsync(x =>
                    x.DeliveryLocationId == dto.DeliveryLocationId &&
                    !x.IsDeleted);

            if (!deliveryLocationExists)
                throw new InvalidOperationException("Delivery location not found.");

            container.ContainerNumber = containerNumber;
            container.ContainerSize = dto.ContainerSize;
            container.ContainerType = dto.ContainerType;
            container.LoadingLocationId = dto.LoadingLocationId;
            container.RouteStatus = dto.RouteStatus;
            container.WithdrawalDate = dto.WithdrawalDate;
            container.LoadingDate = dto.LoadingDate;
            container.DeliveryLocationId = dto.DeliveryLocationId;
            container.WeightInTons = dto.WeightInTons;
            container.Notes = string.IsNullOrWhiteSpace(dto.Notes)
                ? null
                : dto.Notes.Trim();

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var container = await _context.ExportAqabaContainers
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);

            if (container == null)
                return false;

            container.IsDeleted = true;

            var booking = await _context.ExportAqabaBookings
                .FirstOrDefaultAsync(x =>
                    x.Id == container.BookingId &&
                    !x.IsDeleted);

            if (booking != null && booking.ContainerCount > 0)
                booking.ContainerCount--;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
