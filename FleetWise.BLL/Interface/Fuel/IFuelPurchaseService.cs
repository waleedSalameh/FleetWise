using FleetWise.BLL.DTOs.FuelRecords;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.Interface.Fuel
{
    public interface IFuelPurchaseService
    {
        Task<int> CreateAsync(CreateFuelPurchaseDto dto);
        Task<List<FuelPurchaseDto>> GetAllAsync();
        Task<FuelInventorySummaryDto> GetSummaryAsync();
    }
}
