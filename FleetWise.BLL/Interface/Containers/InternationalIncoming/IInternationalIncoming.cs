using FleetWise.BLL.DTOs.Containers.InternationalIncoming;

namespace FleetWise.BLL.Interface.Containers.InternationalIncoming
{
    public interface IInternationalIncoming
    {
        Task<List<InternationalIncomingResponseDto>> GetAllAsync();

        Task<InternationalIncomingResponseDto?> GetByIdAsync(int id);

        Task<InternationalIncomingResponseDto> CreateAsync(CreateInternationalIncomingDto dto);

        Task<bool> UpdateAsync(int id, UpdateInternationalIncomingDto dto);

        Task<bool> DeleteAsync(int id);
    }
}
