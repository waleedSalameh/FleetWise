using FleetWise.BLL.DTOs.Maintenance.SpareParts;

namespace FleetWise.BLL.Interface
{
    public interface ISparePartService
    {
        Task<List<SparePartDto>> GetAllAsync();

        Task<SparePartDto?> GetByIdAsync(int id);

        Task<SparePartDto> CreateAsync(CreateSparePartDto dto);

        Task<bool> UpdateAsync(int id, UpdateSparePartDto dto);

        Task<bool> DeleteAsync(int id);
    }
}