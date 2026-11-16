using FleetWise.BLL.DTOs.Containers.ExportAqaba;
using FleetWise.BLL.Interface.Containers.IncomingAqaba;
using FleetWise.DAL.Data;
using FleetWise.DAL.Entities.Containers.IncomingAqaba;
using FleetWise.DAL.Entities.Customers;
using Microsoft.EntityFrameworkCore;

namespace FleetWise.BLL.Service.Containers.IncomingAqaba
{
    public class IncomingAqabaBillService : IIncomingAqabaBill
    {
        private readonly FleetWiseDbContext _context;

        public IncomingAqabaBillService(FleetWiseDbContext context)
        {
            _context = context;
        }

        public async Task<IncomingAqabaBillResponseDto> CreateAsync(CreateIncomingAqabaBillDto dto)
        {
            string billNumber = dto.BillNumber.Trim();

            bool billExists = await _context.IncomingAqabaBills
                .AnyAsync(bill => bill.BillNumber == billNumber && !bill.IsDeleted);

            if (billExists)
                throw new InvalidOperationException("Bill number already exists.");

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
                throw new InvalidOperationException("Duplicate container numbers in the bill.");

            foreach (var containerNumber in containerNumbers)
            {
                bool containerExists = await _context.IncomingAqabaContainers
                    .AnyAsync(container => container.ContainerNumber == containerNumber && !container.IsDeleted);

                if (containerExists)
                    throw new InvalidOperationException($"Container number {containerNumber} already exists.");
            }

            foreach (var container in dto.Containers)
            {
                bool shippingLineExists = await _context.ShippingLines
                    .AnyAsync(line => line.ShippingLineId == container.ShippingLineId && !line.IsDeleted);

                if (!shippingLineExists)
                    throw new InvalidOperationException($"Shipping line with ID {container.ShippingLineId} was not found.");

                bool deliveryLocationExists = await _context.DeliveryLocations
                    .AnyAsync(location => location.DeliveryLocationId == container.DeliveryLocationId && !location.IsDeleted);

                if (!deliveryLocationExists)
                    throw new InvalidOperationException($"Delivery location with ID {container.DeliveryLocationId} was not found.");
            }

            var bill = new IncomingAqabaBill
            {
                BillNumber = billNumber,
                CustomerId = customer.Id,
                Customer = customer,
                CustomerName = customer.CustomerName,
                Status = dto.Status,
                ContainerCount = dto.Containers.Count,
                Containers = dto.Containers.Select(container => new IncomingAqabaContainer
                {
                    ContainerNumber = container.ContainerNumber.Trim(),
                    ContainerType = container.ContainerType,
                    ContainerSize = container.ContainerSize,
                    ShippingLineId = container.ShippingLineId,
                    DeliveryLocationId = container.DeliveryLocationId,
                    WeightInTons = container.WeightInTons,
                    VesselArrivalDate = container.VesselArrivalDate,
                    PortFreeTimeEndDate = container.PortFreeTimeEndDate,
                    EmptyReturnFreeTimeEndDate = container.EmptyReturnFreeTimeEndDate,
                    ContainerLoadingDate = container.ContainerLoadingDate,
                    Notes = container.Notes,
                    Status = container.Status
                }).ToList()
            };

            _context.IncomingAqabaBills.Add(bill);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(bill.Id)
                ?? throw new InvalidOperationException("Failed to retrieve the created bill.");
        }

        public async Task<List<IncomingAqabaBillResponseDto>> GetAllAsync()
        {
            var bills = await _context.IncomingAqabaBills
                .AsNoTracking()
                .Where(bill => !bill.IsDeleted)
                .Include(bill => bill.Customer)
                .Include(bill => bill.Containers.Where(container => !container.IsDeleted))
                    .ThenInclude(container => container.ShippingLine)
                .Include(bill => bill.Containers.Where(container => !container.IsDeleted))
                    .ThenInclude(container => container.DeliveryLocation)
                .OrderByDescending(bill => bill.CreatedAt)
                .ToListAsync();

            return bills.Select(bill => new IncomingAqabaBillResponseDto
            {
                Id = bill.Id,
                BillNumber = bill.BillNumber,
                CustomerId = bill.CustomerId,
                CustomerName = bill.Customer.CustomerName,
                CustomerPhoneNumber = bill.Customer.PhoneNumber,
                ContainerCount = bill.Containers.Count(container => !container.IsDeleted),
                Status = bill.Status,
                CreatedAt = bill.CreatedAt,
                Containers = bill.Containers
                    .Where(container => !container.IsDeleted)
                    .Select(container => new IncomingAqabaContainerResponseDto
                    {
                        Id = container.Id,
                        ContainerNumber = container.ContainerNumber,
                        ContainerType = container.ContainerType,
                        ContainerSize = container.ContainerSize,
                        ShippingLineId = container.ShippingLineId,
                        ShippingLineName = container.ShippingLine.Name,
                        DeliveryLocationId = container.DeliveryLocationId,
                        DeliveryLocationName = container.DeliveryLocation.Name,
                        WeightInTons = container.WeightInTons,
                        VesselArrivalDate = container.VesselArrivalDate,
                        PortFreeTimeEndDate = container.PortFreeTimeEndDate,
                        EmptyReturnFreeTimeEndDate = container.EmptyReturnFreeTimeEndDate,
                        ContainerLoadingDate = container.ContainerLoadingDate,
                        Notes = container.Notes,
                        Status = container.Status
                    }).ToList()
            }).ToList();
        }

        public async Task<IncomingAqabaBillResponseDto?> GetByIdAsync(int id)
        {
            var bill = await _context.IncomingAqabaBills
                .AsNoTracking()
                .Where(bill => bill.Id == id && !bill.IsDeleted)
                .Include(bill => bill.Customer)
                .Include(bill => bill.Containers.Where(container => !container.IsDeleted))
                    .ThenInclude(container => container.ShippingLine)
                .Include(bill => bill.Containers.Where(container => !container.IsDeleted))
                    .ThenInclude(container => container.DeliveryLocation)
                .FirstOrDefaultAsync();

            if (bill == null)
                return null;

            return new IncomingAqabaBillResponseDto
            {
                Id = bill.Id,
                BillNumber = bill.BillNumber,
                CustomerId = bill.CustomerId,
                CustomerName = bill.Customer.CustomerName,
                CustomerPhoneNumber = bill.Customer.PhoneNumber,
                ContainerCount = bill.Containers.Count(container => !container.IsDeleted),
                Status = bill.Status,
                CreatedAt = bill.CreatedAt,
                Containers = bill.Containers
                    .Where(container => !container.IsDeleted)
                    .Select(container => new IncomingAqabaContainerResponseDto
                    {
                        Id = container.Id,
                        ContainerNumber = container.ContainerNumber,
                        ContainerType = container.ContainerType,
                        ContainerSize = container.ContainerSize,
                        ShippingLineId = container.ShippingLineId,
                        ShippingLineName = container.ShippingLine.Name,
                        DeliveryLocationId = container.DeliveryLocationId,
                        DeliveryLocationName = container.DeliveryLocation.Name,
                        WeightInTons = container.WeightInTons,
                        VesselArrivalDate = container.VesselArrivalDate,
                        PortFreeTimeEndDate = container.PortFreeTimeEndDate,
                        EmptyReturnFreeTimeEndDate = container.EmptyReturnFreeTimeEndDate,
                        ContainerLoadingDate = container.ContainerLoadingDate,
                        Notes = container.Notes,
                        Status = container.Status
                    }).ToList()
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateIncomingAqabaBillDto dto)
        {
            var bill = await _context.IncomingAqabaBills
                .FirstOrDefaultAsync(bill => bill.Id == id && !bill.IsDeleted);

            if (bill == null)
                return false;

            string billNumber = dto.BillNumber.Trim();

            bool billExists = await _context.IncomingAqabaBills
                .AnyAsync(existingBill => existingBill.Id != id && existingBill.BillNumber == billNumber && !existingBill.IsDeleted);

            if (billExists)
                throw new InvalidOperationException("Bill number already exists.");

            var customer = await _context.Set<Customer>()
                .FirstOrDefaultAsync(customer => customer.Id == dto.CustomerId && !customer.IsDeleted);

            if (customer == null)
                throw new InvalidOperationException("An active customer with the specified ID was not found.");

            bill.BillNumber = billNumber;
            bill.CustomerId = customer.Id;
            bill.Customer = customer;
            bill.CustomerName = customer.CustomerName;
            bill.Status = dto.Status;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var bill = await _context.IncomingAqabaBills
                .Include(bill => bill.Containers)
                .FirstOrDefaultAsync(bill => bill.Id == id && !bill.IsDeleted);

            if (bill == null)
                return false;

            bill.IsDeleted = true;

            foreach (var container in bill.Containers)
                container.IsDeleted = true;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
