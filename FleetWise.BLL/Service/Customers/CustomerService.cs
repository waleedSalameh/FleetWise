using FleetWise.BLL.DTOs.Customers;
using FleetWise.BLL.Interface.Customers;
using FleetWise.DAL.Data;
using FleetWise.DAL.Entities.Customers;
using Microsoft.EntityFrameworkCore;

namespace FleetWise.BLL.Service.Customers
{
    public class CustomerService : ICustomer
    {
        private readonly FleetWiseDbContext _context;

        public CustomerService(FleetWiseDbContext context)
        {
            _context = context;
        }

        public async Task<List<CustomerResponseDto>> GetAllAsync()
        {
            return await _context.Set<Customer>()
                .AsNoTracking()
                .Where(customer => !customer.IsDeleted)
                .OrderBy(customer => customer.CustomerName)
                .Select(customer => new CustomerResponseDto
                {
                    Id = customer.Id,
                    CustomerName = customer.CustomerName,
                    PhoneNumber = customer.PhoneNumber
                })
                .ToListAsync();
        }

        public async Task<CustomerResponseDto?> GetByIdAsync(int id)
        {
            return await _context.Set<Customer>()
                .AsNoTracking()
                .Where(customer => customer.Id == id && !customer.IsDeleted)
                .Select(customer => new CustomerResponseDto
                {
                    Id = customer.Id,
                    CustomerName = customer.CustomerName,
                    PhoneNumber = customer.PhoneNumber
                })
                .FirstOrDefaultAsync();
        }

        public async Task<CustomerResponseDto> CreateAsync(CreateCustomerDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CustomerName))
                throw new InvalidOperationException("Customer name is required.");

            string customerName = dto.CustomerName.Trim();
            string phoneNumber = new(dto.PhoneNumber.Where(char.IsDigit).ToArray());

            if (phoneNumber.Length is < 7 or > 15)
                throw new InvalidOperationException("Phone number must contain between 7 and 15 digits.");

            bool phoneNumberExists = await _context.Set<Customer>()
                .AnyAsync(customer => !customer.IsDeleted && customer.PhoneNumber == phoneNumber);

            if (phoneNumberExists)
                throw new InvalidOperationException("A customer with this phone number already exists.");

            var customer = new Customer
            {
                CustomerName = customerName,
                PhoneNumber = phoneNumber
            };

            _context.Set<Customer>().Add(customer);
            await _context.SaveChangesAsync();

            return new CustomerResponseDto
            {
                Id = customer.Id,
                CustomerName = customer.CustomerName,
                PhoneNumber = customer.PhoneNumber
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateCustomerDto dto)
        {
            var customer = await _context.Set<Customer>()
                .FirstOrDefaultAsync(customer => customer.Id == id && !customer.IsDeleted);

            if (customer == null)
                return false;

            if (string.IsNullOrWhiteSpace(dto.CustomerName))
                throw new InvalidOperationException("Customer name is required.");

            string customerName = dto.CustomerName.Trim();
            string phoneNumber = new(dto.PhoneNumber.Where(char.IsDigit).ToArray());

            if (phoneNumber.Length is < 7 or > 15)
                throw new InvalidOperationException("Phone number must contain between 7 and 15 digits.");

            bool phoneNumberExists = await _context.Set<Customer>()
                .AnyAsync(existingCustomer =>
                    existingCustomer.Id != id &&
                    !existingCustomer.IsDeleted &&
                    existingCustomer.PhoneNumber == phoneNumber);

            if (phoneNumberExists)
                throw new InvalidOperationException("A customer with this phone number already exists.");

            customer.CustomerName = customerName;
            customer.PhoneNumber = phoneNumber;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var customer = await _context.Set<Customer>()
                .FirstOrDefaultAsync(customer => customer.Id == id && !customer.IsDeleted);

            if (customer == null)
                return false;

            bool hasActiveIncomingBills = await _context.IncomingAqabaBills
                .AnyAsync(bill => !bill.IsDeleted && bill.CustomerName == customer.CustomerName);

            bool hasActiveExportBookings = await _context.ExportAqabaBookings
                .AnyAsync(booking => !booking.IsDeleted && booking.CustomerName == customer.CustomerName);

            if (hasActiveIncomingBills || hasActiveExportBookings)
                throw new InvalidOperationException("Customer cannot be deleted because active records are linked to this customer.");

            customer.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
