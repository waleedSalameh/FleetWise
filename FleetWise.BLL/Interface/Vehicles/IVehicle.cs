using FleetWise.BLL.DTOs.Vehicles;
using System;
using System.Collections.Generic;
using System.Text;

namespace FleetWise.BLL.Interface.Vehicles
{
    public interface IVehicle
    {
        Task<List<VehicleDto>> GetAllAsync();

        Task<VehicleDto> GetByIdAsync(int id);

        Task<VehicleDto> CreateAsync(CreateVehicleDto dto);

        Task<bool> UpdateAsync(int id, UpdateVehicleDto dto);

        Task<bool> DeleteAsync(int id);
    }
}
