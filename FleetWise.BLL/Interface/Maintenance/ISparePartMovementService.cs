using FleetWise.BLL.DTOs.Maintenance.SparePartMovements;

namespace FleetWise.BLL.Interface
{
    public interface ISparePartMovementService
    {
        Task<List<SparePartMovementDto>> GetAllAsync();

        Task<List<SparePartMovementDto>> GetBySparePartIdAsync(int sparePartId);

        Task<SparePartMovementDto?> GetByIdAsync(int id);

        Task<SparePartMovementDto> CreateAsync(CreateSparePartMovementDto dto);

        Task<decimal> GetCurrentStockAsync(int sparePartId);
    }
}