using FleetWise.BLL.DTOs.FuelRecords;

namespace FleetWise.BLL.Interface.Fuel;

public interface IFuelReportService
{
    Task<List<MonthlyFuelReportDto>> GetMonthlyAsync(DateOnly start, DateOnly end, int? vehicleId);
    Task<List<FuelTransactionDto>> GetTransactionsAsync(DateTime start, DateTime end, int? vehicleId);
}
