using FleetWise.BLL.DTOs.FuelRecords;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.Interface.Fuel
{

    public interface IFuelRecord
    {
        Task<List<FuelRecordDto>> GetAllAsync();

        Task<FuelRecordDto?> GetByIdAsync(int id);

        Task<FuelRecordDto> CreateAsync(CreateFuelRecordDto dto);

        Task<FuelRecordDto> CreateInternalAsync(CreateInternalFuelRecordDto dto);

        Task<bool> UpdateAsync(int id, UpdateFuelRecordDto dto);

        Task<bool> DeleteAsync(int id);
    }
}
