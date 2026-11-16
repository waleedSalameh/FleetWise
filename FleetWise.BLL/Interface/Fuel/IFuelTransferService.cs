using FleetWise.BLL.DTOs.FuelRecords;

namespace FleetWise.BLL.Interface.Fuel;

public interface IFuelTransferService
{
    Task<FuelTransferDto> CreateAsync(CreateFuelTransferDto dto);
    Task<List<FuelTransferDto>> GetAllAsync();
    Task<FuelTransferDto?> GetByIdAsync(int id);
    Task<FuelTransferDto> ReverseAsync(int id, string notes);
}
