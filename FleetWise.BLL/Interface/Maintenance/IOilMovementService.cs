using FleetWise.BLL.DTOs.Maintenance.OilMovements;

namespace FleetWise.BLL.Interface
{
    public interface IOilMovementService
    {
        Task<List<OilMovementDto>> GetAllAsync();

        Task<List<OilMovementDto>> GetByOilIdAsync(int oilId);

        Task<OilMovementDto?> GetByIdAsync(int id);

        Task<OilMovementDto> CreateAsync(CreateOilMovementDto dto);

        Task<decimal> GetCurrentStockAsync(int oilId);
    }
}