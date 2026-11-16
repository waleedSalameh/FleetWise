using FleetWise.BLL.DTOs.Customers;

namespace FleetWise.BLL.Interface.Customers
{
    public interface ICustomer
    {
        Task<List<CustomerResponseDto>> GetAllAsync();

        Task<CustomerResponseDto?> GetByIdAsync(int id);

        Task<CustomerResponseDto> CreateAsync(CreateCustomerDto dto);

        Task<bool> UpdateAsync(int id, UpdateCustomerDto dto);

        Task<bool> DeleteAsync(int id);
    }
}
