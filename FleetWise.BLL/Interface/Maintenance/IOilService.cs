using FleetWise.BLL.DTOs.Maintenance.Oils;

namespace FleetWise.BLL.Interface
{
    public interface IOilService
    {
        Task<List<OilDto>> GetAllAsync();

        Task<OilDto?> GetByIdAsync(int id);

        Task<OilDto> CreateAsync(CreateOilDto dto);

        Task<bool> UpdateAsync(int id, UpdateOilDto dto);

        Task<bool> DeleteAsync(int id);
    }
}