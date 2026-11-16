using FleetWise.BLL.DTOs.Containers.InlandTransport;

namespace FleetWise.BLL.Interface.Containers.InlandTransport
{
    public interface IInlandTransport
    {
        Task<List<InlandTransportResponseDto>> GetAllAsync();

        Task<InlandTransportResponseDto?> GetByIdAsync(int id);

        Task<InlandTransportResponseDto> CreateAsync(CreateInlandTransportDto dto);

        Task UpdateAsync(int id, UpdateInlandTransportDto dto);

        Task DeleteAsync(int id);
    }
}