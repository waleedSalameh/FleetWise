using FleetWise.BLL.DTOs.Drivers;
using FleetWise.BLL.DTOs.Vehicles;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.Interface
{
    public interface IDriver
    {
        Task<List<DriverDto>> GetAllAsync();
        Task<DriverDto> GetByIdAsync(int id);
        Task<DriverDto> CreateAsync(CreateDriverDto dto);
        Task<bool> UpdateAsync(int id, UpdateDriverDto dto);
        Task<bool> DeleteAsync(int id);

    }
}
