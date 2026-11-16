using FleetWise.BLL.DTOs.FuelRecords;

namespace FleetWise.BLL.Interface.Fuel;

public interface IVehicleFuelMeasurementService
{
    Task<FuelMeasurementDto> CreateAsync(CreateFuelMeasurementDto dto);
    Task<FuelMeasurementDto> ApproveAsync(long id, string approvedBy);
    Task<List<FuelMeasurementDto>> GetHistoryAsync(int vehicleId);
}
